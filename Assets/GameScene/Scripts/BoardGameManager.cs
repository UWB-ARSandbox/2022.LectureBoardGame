//Used for help debug GameLift packet issues and other misc. GameLift potential problems.
#define ASL_DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aws.GameLift.Realtime.Event;
using Aws.GameLift.Realtime;
using Aws.GameLift.Realtime.Command;
using Aws.GameLift.Realtime.Types;
using ASL;

public class BoardGameManager : MonoBehaviour
{
    // The singleton instance for this class
    private static BoardGameManager m_Instance;

    private GameObject groupLobby;
    private GameObject groupLobbyCanvas;
    private GameObject studentUI;
    private GameObject teacherUI;
    private GameObject groupWorld1;
    private PlayerGrouping playerGrouping;
    [SerializeField] private int currId;


    // Start is called before the first frame update
    void Start()
    {
        m_Instance = this;
        groupLobby = GameObject.Find("GroupingLobby");
        groupLobbyCanvas = groupLobby.transform.Find("Canvas").gameObject;
        studentUI = GameObject.Find("StudentUI");
        groupWorld1 = studentUI.transform.GetChild(0).gameObject;
        teacherUI = GameObject.Find("TeacherUI");
        playerGrouping = this.gameObject.GetComponent<PlayerGrouping>();
        studentUI.SetActive(false);
        // teacherUI.SetActive(false);
        currId = GameLiftManager.GetInstance().m_PeerId;

    }

    // Update is called once per frame
    void Update()
    {

    }




    public void startGame()
    {
        //Claim the object
        groupLobby.GetComponent<ASL.ASLObject>().SendAndSetClaim(() =>
        {
            //Send and then set (once received - NOT here) the tag
            groupLobby.GetComponent<ASL.ASLObject>().SendAndSetTag("Finish");
        });
        groupLobbyCanvas.SetActive(false);
        #region set up student group worlds
        //int playerIndex = 2; //first 2 children don't count
        //for (int i = 1; i < playerGrouping.groupNumList.transform.childCount; i++)
        //{
        //    for (int j = 0; j < playerGrouping.groupLimit; j++)
        //    {
        //        GameObject player = playerGrouping.playersGrid.transform.GetChild(playerIndex++).gameObject;
        //        if (player != null)
        //        {
        //            if (player.name.Equals())
        //        }
        //    }
        //}
        //int disableOffset = 2;
        //int groupCount = 0;
        //for (int i = disableOffset; i < playerGrouping.playersGrid.transform.childCount; i++)
        //{
        //    GameObject player = playerGrouping.playersGrid.transform.GetChild(i).gameObject;
        //    int groupNum = (i - disableOffset) / playerGrouping.groupLimit + 1;
        //    if (groupNum > groupCount)
        //    {
        //        groupCount++;

        //    }

        //}
        #endregion
        Debug.Log("id: " + GameLiftManager.GetInstance().m_PeerId);
        if (GameLiftManager.GetInstance().m_PeerId == 1)
        {
            teacherUI.SetActive(true);
        }
        else
        {
            studentUI.SetActive(true);
        }
    }

    public void playerStartGame()
    {
        Debug.Log("playerStartGame");
        if (GameLiftManager.GetInstance().m_PeerId != 1)
        {
            groupLobbyCanvas.SetActive(false);
            studentUI.SetActive(true);
        }
    }

    private void createGroupWorld(int num)
    {
        GameObject newWorld = GameObject.Instantiate(groupWorld1);
        newWorld.name = "Group" + num;
        newWorld.SetActive(true);
        newWorld.transform.SetParent(studentUI.transform);
        //newWorld.localTransform
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
