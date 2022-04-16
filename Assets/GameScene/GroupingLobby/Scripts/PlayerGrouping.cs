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

namespace ASL
{
    public class PlayerGrouping : MonoBehaviour
    {
        private GameObject playerListText;
        public GameObject groupNameText;
        public GameObject playerNameBox;
        public GameObject blankImage;
        public GameObject playersGrid;
        public GameObject groupNumList;
        public GameObject StartButton;
        public GameObject UploadButton;
        private int playerCount = 0; //not including teacher
        private int groupCount = 0;
        private int blankCount = 0;
        private int groupLimit = 4;
        private int hostPlayerID;
        public string gameScene = "studentgame";

        // Start is called before the first frame update
        void Start()
        {
            playerListText = GameObject.Find("PlayerListText");
            playerListText.GetComponent<UnityEngine.UI.Text>().text = "Player List (" + (GameLiftManager.GetInstance().m_Players.Count - 1) + ")";
            populatePlayers();
            if (GameLiftManager.GetInstance().m_PeerId == hostPlayerID)
            {
                StartButton.SetActive(true);
                UploadButton.SetActive(true);
            } else
            {
                GameObject.Find("CancelButton").SetActive(true);
                GameObject.Find("AddBlankButton").SetActive(false);
                GameObject.Find("UploadButton").SetActive(false);
                GameObject.Find("StartButton").SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void populatePlayers()
        {
            var players = GameLiftManager.GetInstance().m_Players;
            hostPlayerID = Int32.MaxValue;
            foreach (var player in players)
            {
                if (player.Key < hostPlayerID)
                {
                    hostPlayerID = player.Key;
                }
            }
            foreach (var player in players)
            {
                if (player.Key != hostPlayerID)
                    addPlayer(player.Value);
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

        private void addPlayer(string name)
        {
            if ((playerCount++) % groupLimit == 0)
            {
                addGroup();
            }
            GameObject newPlayer = GameObject.Instantiate(playerNameBox);
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
            //if (playerCount < 1)
            //{
            //    //err
            //}
            //int playersInGroup = 0;
            //for (int i = 0; i < groupCount + playerCount; i++)
            //{
            //    GameObject item = listContentBox.transform.GetChild(i).gameObject;
            //    if (item.name[0] == 'g')
            //    {
            //        if (playersInGroup > 4)
            //        {
            //            groupCountTestText.GetComponent<UnityEngine.UI.Text>().text = "More than 4 players in one or more group";
            //            return;
            //        }
            //        else
            //        {
            //            playersInGroup = 0;
            //            groupCountTestText.GetComponent<UnityEngine.UI.Text>().text = "All groups have 4 or less players";
            //        }
            //    }
            //    else
            //    {
            //        playersInGroup++;
            //    }
            //}
            //if (GameLiftManager.GetInstance().m_PeerId == hostPlayerID)
            //{
            //    SceneManager.LoadScene(teacherScene);
            //} else
            //{
            SceneManager.LoadScene(gameScene);

        }

        public void cancel()
        {
            GameLiftManager.GetInstance().DisconnectFromServer();
            SceneManager.LoadScene("ASL_LobbyScene");
        }
    }
}