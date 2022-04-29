using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class PlayerData : MonoBehaviour
{
    private PlayerGrouping pGroup;
    private BoardGameManager bgm;
    private int playerNumber;

    private GameObject player1;
    private int p1Stars;
    private int p1MovePoints;

    private GameObject player2;
    private int p2Stars;
    private int p2MovePoints;

    private GameObject player3;
    private int p3Stars;
    private int p3MovePoints;

    private GameObject player4;
    private int p4Stars;
    private int p4MovePoints;

    // Start is called before the first frame update
    void Start()
    {
        pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();
        playerNumber = 0;

        gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(readData);
    }

    void Update()
    {
        if (player1 != null)
            player1.transform.Find("playerPoints").GetComponent<Text>().text = "Stars: " + p1Stars + "\nMove Pts: " + p1MovePoints;
        if (player2 != null)
            player2.transform.Find("playerPoints").GetComponent<Text>().text = "Stars: " + p2Stars + "\nMove Pts: " + p2MovePoints;
        if (player3 != null)
            player3.transform.Find("playerPoints").GetComponent<Text>().text = "Stars: " + p3Stars + "\nMove Pts: " + p3MovePoints;
        if (player4 != null)
            player4.transform.Find("playerPoints").GetComponent<Text>().text = "Stars: " + p4Stars + "\nMove Pts: " + p4MovePoints;
    }

    public void readData(string _id, float[] _f)
    {
        // UPDATE THE APPROPRIATE PLAYERPANEL VARIABLES
        switch (_f[0])
        {
            case 1:
                p1Stars = (int)_f[1];
                p1MovePoints = (int)_f[2];
                break;

            case 2:
                p2Stars = (int)_f[1];
                p2MovePoints = (int)_f[2];
                break;

            case 3:
                p3Stars = (int)_f[1];
                p3MovePoints = (int)_f[2];
                break;

            case 4:
                p4Stars = (int)_f[1];
                p4MovePoints = (int)_f[2];
                break;
        }
    }

    public void sendData()
    {
        if (player1 == null)
            player1 = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("PlayerPanels").Find("player1").gameObject;
        if (player2 == null)
            player2 = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("PlayerPanels").Find("player2").gameObject;
        if (player3 == null)
            player3 = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("PlayerPanels").Find("player3").gameObject;
        if (player4 == null)
            player4 = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("PlayerPanels").Find("player4").gameObject;

        for (int i = 1; i <= pGroup.m_playerGroups[bgm.getPlayerGroup() - 1].Count; i++)
        {
            if (pGroup.m_playerGroups[bgm.getPlayerGroup() - 1][i - 1] == GameLiftManager.GetInstance().m_PeerId)
            {
                playerNumber = i;
            }
        }
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[3];
            sendValue[0] = playerNumber;
            sendValue[1] = DiceRoll.starCount;
            sendValue[2] = DiceRoll.movePoints;

            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }
}
