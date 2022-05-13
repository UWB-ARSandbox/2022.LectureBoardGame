using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class StarRankingButton : MonoBehaviour
{
    private Color32 lightBlue;
    private Color32 white;
    public bool forRankingList; //false--for Player List
    private StarRankingPanel rankPanel;

    public string username;
    public int peerId;
    public int groupNum;
    public int ranking;
    public int stars;

    // Start is called before the first frame update
    void Start()
    {
        lightBlue = new Color32(181, 236, 255, 255);
        white = new Color32(255, 255, 255, 255);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setup(string username, int id, bool forRankingList, StarRankingPanel starRankPanel)
    {
        this.username = username;
        peerId = id;
        groupNum = BoardGameManager.GetInstance().getPlayerGroup(id);
        this.forRankingList = forRankingList;
        rankPanel = starRankPanel;
        setStars();
        transform.GetChild(0).gameObject.GetComponent<Text>().text = username;
        if (GameLiftManager.GetInstance().m_PeerId == id)
        {
            GetComponent<Image>().color = Color.yellow;
        }
    }
    
    public void setRank(int rank) //star ranking button
    {
        ranking = rank;
        transform.GetChild(0).gameObject.GetComponent<Text>().text = rank + ". " + username;
    }

    public void setStars() //star ranking button
    {
        stars = GameReport.studentStats[peerId].stars;
        transform.GetChild(1).gameObject.GetComponent<Text>().text = stars.ToString();
    }

    public void setStat() //player list button
    {
        transform.GetChild(1).gameObject.GetComponent<Text>().text = GameReport.studentStats[peerId].numCorrect + "/" + 
            GameReport.studentStats[peerId].numAnswered + "/" + GameReport.qPosted;
    }
}
