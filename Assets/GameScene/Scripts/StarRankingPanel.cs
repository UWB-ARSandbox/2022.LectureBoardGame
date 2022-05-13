using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class StarRankingPanel : MonoBehaviour
{
    public PlayerGrouping pGroup;
    private GameReport gameReport;

    public Text title;
    public GameObject rankingContent;
    public GameObject starRankButton;
    //teacherUI only
    public GameObject playerButton;
    public GameObject studentQPanel;
    public GameObject playerListContent;

    public int peerId; //for current player info being displayed

    // Start is called before the first frame update
    void Start()
    {
        gameReport = GameObject.Find("GameReport").GetComponent<GameReport>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //should be called once
    public void teacherUISetUp() //fill player list
    {
        bool evenGroup = false; //for alternating player button color by group
        foreach (List<int> group in pGroup.m_playerGroups)
        {
            foreach (int playerID in group)
            {
            }
            if (evenGroup)
                evenGroup = false;
            else
                evenGroup = true;
        }
    }
    public void groupRankSetUp(int groupNum)
    {
        if (groupNum <= 0)
            return;
        title.text = "Group " + groupNum + " Star Ranking";
        List<int> group = pGroup.m_playerGroups[groupNum - 1];
        foreach (int playerID in group)
        {
            if (playerID > 0)
                addPlayerButton(GameLiftManager.GetInstance().m_Players[playerID], playerID, true);
        }
    }

    public void addPlayerButton(string username, int id, bool forRankingList)
    {
        if (forRankingList)
        {
            GameObject newStudent = GameObject.Instantiate(starRankButton);
            newStudent.GetComponent<StarRankingButton>().setup(username, id, forRankingList, this);
            newStudent.transform.parent = rankingContent.transform;
            newStudent.SetActive(true);
            rankPlayer(newStudent);
        } else
        {
            GameObject newStudent = GameObject.Instantiate(playerButton);
        }
    }

    public void rankPlayer(GameObject player)
    {
        //int index = player.transform.GetSiblingIndex();
        int stars = GameReport.studentStats[player.GetComponent<StarRankingButton>().peerId].stars;
        for (int i = 0; i < rankingContent.transform.childCount; i++)
        {
            GameObject curr = rankingContent.transform.GetChild(i).gameObject;
            int currStars = GameReport.studentStats[curr.GetComponent<StarRankingButton>().peerId].stars;
            if (stars > currStars || (i == rankingContent.transform.childCount))
            {
                player.transform.SetSiblingIndex(i);
                break;
            }
        }
        //re-rank players after the order change
        for (int i = 0 ;i < rankingContent.transform.childCount; i++)
        {
            GameObject curr = rankingContent.transform.GetChild(i).gameObject;
            curr.GetComponent<StarRankingButton>().setRank(i + 1);
        }
    }

    public GameObject GetStarRankButton(int id)
    {
        for (int i = 0; i < rankingContent.transform.childCount; i++)
        {
            GameObject curr = rankingContent.transform.GetChild(i).gameObject;
            if (curr.GetComponent<StarRankingButton>().peerId == id)
            {
                return curr;
            }
        }
        return null;
    }

    public void openQuestionList() //teacher UI
    {

    }
}
