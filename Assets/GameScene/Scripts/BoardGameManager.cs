//Used for help debug GameLift packet issues and other misc. GameLift potential problems.
#define ASL_DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aws.GameLift.Realtime.Event;
using Aws.GameLift.Realtime;
using Aws.GameLift.Realtime.Command;
using Aws.GameLift.Realtime.Types;
using ASL;

public class BoardGameManager : MonoBehaviour
{
    // The singleton instance for this class
    private static BoardGameManager m_Instance;
    /// <summary>
    /// Dictionary containing the all players where key=peerid, value=group#
    /// </summary>
    public Dictionary<int, int> m_Players = new Dictionary<int, int>();

    private GameObject groupLobby;
    private GameObject groupLobbyCanvas;
    private GameObject studentUI;
    private GameObject teacherUI;
    private PlayerGrouping playerGrouping;
    private GameObject camLight;
    [SerializeField] private int currId;
    [SerializeField] private int m_groupWorldSpacing = 75;


    // Start is called before the first frame update
    void Start()
    {
        m_Instance = this;
        groupLobby = GameObject.Find("GroupingLobby");
        groupLobbyCanvas = groupLobby.transform.Find("Canvas").gameObject;
        studentUI = GameObject.Find("StudentUI");
        teacherUI = GameObject.Find("TeacherUI");
        playerGrouping = this.gameObject.GetComponent<PlayerGrouping>();
        camLight = GameObject.Find("camLight");
        studentUI.SetActive(false);
        teacherUI.SetActive(false);
        currId = GameLiftManager.GetInstance().m_PeerId;

    }

    // Update is called once per frame
    void Update()
    {

    }
    //Called by host
    public void startGame()
    {
        Debug.Log("host startGame");
        studentUI.SetActive(true);
        setupGroupWorlds();
        //Claim the object
        groupLobby.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
        {
            //Send and then set (once received - NOT here) the tag
            groupLobby.GetComponent<ASL.ASLObject>().SendAndSetTag("Finish");
        });
        studentUI.SetActive(false);
        groupLobbyCanvas.SetActive(false);
        teacherUI.SetActive(true);
    }

    private int getPlayerGroup()
    {
        int disableOffset = 2;
        for (int i = disableOffset; i < playerGrouping.playersGrid.transform.childCount; i++)
        {
            GameObject player = playerGrouping.playersGrid.transform.GetChild(i).gameObject;
            if (Int16.Parse(player.name) == GameLiftManager.GetInstance().m_PeerId)
                return (i - disableOffset) / playerGrouping.groupLimit + 1;
        }
        return -1;
    }

    public void playerStartGame()
    {
        Debug.Log("playerStartGame");
        groupLobbyCanvas.SetActive(false);
        int id = GameLiftManager.GetInstance().m_PeerId;
        if (id != 1)
        {
            studentUI.SetActive(true);
            int groupNum = getPlayerGroup();
            for (int i = 0; i < studentUI.transform.childCount; i++)
            {
                //if (i == m_Players[id] - 1) //group player is in
                if (i == groupNum - 1)
                {
                    studentUI.transform.GetChild(i).gameObject.SetActive(true);
                    studentUI.transform.GetChild(i).Find("CamLight").gameObject.SetActive(true);
                    camLight.SetActive(false);
                } else
                {
                    studentUI.transform.GetChild(i).gameObject.SetActive(false);
                }
                
            }
        }
    }


    private void setupGroupWorlds()
    {
        Debug.Log("setupGroupWorlds");
        int disableOffset = 2;
        int numOfGroups = (playerGrouping.playersGrid.transform.childCount - disableOffset) / playerGrouping.groupLimit + 1;
        for (int i = 1; i <= numOfGroups; i++)
        {
            createGroupWorld(i);
        }

        //for (int i = disableOffset; i < playerGrouping.playersGrid.transform.childCount; i++)
        //{
        //    GameObject player = playerGrouping.playersGrid.transform.GetChild(i).gameObject;
        //    int groupNum = (i - disableOffset) / playerGrouping.groupLimit + 1;
        //    int playerNum = (i - disableOffset) % playerGrouping.groupLimit; //0-3
        //    Debug.Log("sutdentUI childCount: " + studentUI.transform.childCount + "/ngroupNum: " + groupNum);
        //    if (studentUI.transform.childCount < groupNum)
        //    {
        //        createGroupWorld(groupNum);
        //    }
        //    else
        //    {
        //        setupPlayerPanel(groupNum, playerNum, player);
        //    }
        //}
    }

    public GameObject getGroupWorld(int groupNum)
    {
        if (groupNum < 1 || groupNum >= studentUI.transform.childCount)
            return null;
        return studentUI.transform.GetChild(groupNum - 1).gameObject;
    }

    public GameObject getPlayerPanel(int groupNum)
    {
        GameObject groupWorld = getGroupWorld(groupNum);
        if (groupWorld == null)
            return null;
        return groupWorld.transform.Find("Canvas").Find("PlayerPanels").gameObject;
    }

    private void createGroupWorld(int groupNum)
    {
        Debug.Log("createGroupWorld");
        ASLHelper.InstantiateASLObject("GroupWorld", new Vector3(studentUI.transform.position.x + m_groupWorldSpacing * (groupNum - 1), studentUI.transform.position.y,
            studentUI.transform.position.z), Quaternion.identity, studentUI.GetComponent<ASLObject>().m_Id, "", WhatToDoWithMyOtherGameObjectNowThatItIsCreated,
            ClaimRecoveryFunction, MyFloatsFunction);
    }

    private void setupPlayerPanel(int groupNum, int playerNum, GameObject player)
    {
        GameObject playerPanels = getPlayerPanel(groupNum);
        Debug.Assert(playerPanels != null);
        if (player.name[0] == 'b')
        {
            playerPanels.transform.GetChild(playerNum).gameObject.SetActive(false);
        }
        else
        {
            playerPanels.transform.GetChild(playerNum).gameObject.GetComponent<PlayerPanel>().setPlayer(player.transform.GetChild(0).gameObject.GetComponent<Text>().text, Int16.Parse(player.name));
            m_Players.Add(Int16.Parse(player.name), groupNum);
        }
    }

    /// <summary>
    /// A function that is called right after an ASL game object is created if that object was passed in the class name and function name of this function.
    /// This is called immediately upon creation, allowing the user a way to access their newly created object after the server has spawned it
    /// </summary>
    /// <param name="_gameObject">The gameobject that was created</param>
    public static void WhatToDoWithMyOtherGameObjectNowThatItIsCreated(GameObject _gameObject)
    {
        BoardGameManager bgManager= BoardGameManager.GetInstance();
        int groupNum = _gameObject.transform.GetSiblingIndex() + 1;
        int player1 = bgManager.playerGrouping.groupLimit * (groupNum - 1) + 2;
        //GameObject player = bgManager.playerGrouping.playersGrid.transform.GetChild(player1).gameObject;
        //bgManager.setupPlayerPanel(groupNum, player1, player);
        int j = 0;
        for (int i = player1; (i < player1 + 4) && (i < bgManager.playerGrouping.playersGrid.transform.childCount); i++)
        {
            GameObject player = bgManager.playerGrouping.playersGrid.transform.GetChild(i).gameObject;
            //bgManager.setupPlayerPanel(groupNum, i, player);
            GameObject playerPanels = _gameObject.transform.Find("Canvas").Find("PlayerPanels").gameObject;
            Debug.Assert(playerPanels != null);
            if (player.name[0] == 'b')
            {
                playerPanels.transform.GetChild(j++).gameObject.SetActive(false);
            }
            else
            {
                playerPanels.transform.GetChild(j++).gameObject.GetComponent<PlayerPanel>().setPlayer(player.transform.GetChild(0).gameObject.GetComponent<Text>().text, Int16.Parse(player.name));
                bgManager.m_Players.Add(Int16.Parse(player.name), groupNum);
            }

        }
    }

    /// <summary>
    /// A function that is called when an ASL object's claim is rejected. This function can be set to be called upon object creation.
    /// </summary>
    /// <param name="_id">The id of the object who's claim was rejected</param>
    /// <param name="_cancelledCallbacks">The amount of claim callbacks that were cancelled</param>
    public static void ClaimRecoveryFunction(string _id, int _cancelledCallbacks)
    {
        Debug.Log("Aw man. My claim got rejected for my object with id: " + _id + " it had " + _cancelledCallbacks + " claim callbacks to execute.");
        //If I can't have this object, no one can. (An example of how to get the object we were unable to claim based on its ID and then perform an action). Obviously,
        //deleting the object wouldn't be very nice to whoever prevented your claim
        if (ASL.ASLHelper.m_ASLObjects.TryGetValue(_id, out ASL.ASLObject _myObject))
        {
            _myObject.GetComponent<ASL.ASLObject>().DeleteObject();
        }

    }

    /// <summary>
    /// A function that is called whenever an ASL object calls <see cref="ASL.ASLObject.SendFloatArray(float[])"/>.
    /// This function can be assigned to an ASL object upon creation.
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_myFloats"></param>
    public static void MyFloatsFunction(string _id, float[] _myFloats)
    {

    }

    public static BoardGameManager GetInstance()
    {
        if (m_Instance != null)
        {
            return m_Instance;
        }
        else
        {
            Debug.LogError("BoardGameManager not initialized.");
        }
        return null;
    }
}
