//Used for help debug GameLift packet issues and other misc. GameLift potential problems.
#define ASL_DEBUG
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Aws.GameLift.Realtime.Event;
using Aws.GameLift.Realtime;
using Aws.GameLift.Realtime.Command;
using Aws.GameLift.Realtime.Types;
using ASL;
using UnityEngine.UI;

public class PlayerGrouping : MonoBehaviour
{
    private GameObject playerListText;
    public GameObject groupNameText;
    public GameObject playerNameBox;
    public GameObject blankImage;
    public GameObject playersGrid;
    public GameObject groupNumList;
    private int playerCount = 0; //not including teacher
    private int groupCount = 0;
    private int blankCount = 0;
    public int groupLimit = 4;
    public string gameScene = "studentgame";
    private GameObject groupLobby;

    // Start is called before the first frame update
    void Start()
    {
        groupLobby = GameObject.Find("GroupingLobby");
        playerListText = GameObject.Find("PlayerListText");
        playerListText.GetComponent<UnityEngine.UI.Text>().text = "Player List (" + (GameLiftManager.GetInstance().m_Players.Count - 1) + ")";
        populatePlayers();
        if (GameLiftManager.GetInstance().m_PeerId != 1)
        {
            GameObject.Find("CancelButton").SetActive(true);
            GameObject.Find("AddBlankButton").SetActive(false);
            GameObject.Find("UploadButton").SetActive(false);
            GameObject.Find("StartButton").SetActive(false);
            StartCoroutine(startPlayerGame());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void populatePlayers()
    {
        var players = GameLiftManager.GetInstance().m_Players;
        foreach (var player in players)
        {
            if (player.Key != 1)
            {
                addPlayer(player.Value, player.Key);
            }
        }
    }

    public void addBlank()
    {
        if (((blankCount++) + playerCount) % groupLimit == 0)
        {
            addGroup();
        }
        GameObject newBlank = GameObject.Instantiate(blankImage);
        newBlank.SetActive(true);
        newBlank.transform.SetParent(playersGrid.transform);
    }

    private void addPlayer(string name, int id)
    {
        if ((playerCount++) % groupLimit == 0)
        {
            addGroup();
        }
        GameObject newPlayer = GameObject.Instantiate(playerNameBox);
        newPlayer.name = "" + id;
        newPlayer.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = name;
        newPlayer.SetActive(true);
        newPlayer.transform.SetParent(playersGrid.transform);
    }

    private void addGroup()
    {
        Debug.Log("Add group");
        GameObject newGroup = GameObject.Instantiate(groupNameText);
        newGroup.GetComponent<UnityEngine.UI.Text>().text = "" + (++groupCount);
        newGroup.SetActive(true);
        newGroup.transform.SetParent(groupNumList.transform);

    }
    /*Start Game
        * Need at least 1 person other than host
        * correct group size
        * 
        */
    public void startGame()
    {
        SceneManager.LoadScene(gameScene);
    }

    private IEnumerator startPlayerGame()
    {
        while(groupLobby.tag != "Finish")
        {
            yield return null;
        }
        BoardGameManager.GetInstance().playerStartGame();
    }

    public void cancel()
    {
        GameLiftManager.GetInstance().DisconnectFromServer();
        SceneManager.LoadScene("ASL_LobbyScene");
    }
}