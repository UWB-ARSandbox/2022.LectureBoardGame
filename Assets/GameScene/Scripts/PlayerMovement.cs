using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class PlayerMovement : MonoBehaviour
{
    // private Animator anim;
    ASLObject m_ASLObject;
    public string currDirection;
    private BoardGameManager bgm;
    public static TileNode currentTile;
    private PlayerData playerData;
    public GameObject questions;
    public GameObject qPanel;
    public GameObject sg;
    public CoinFlip coin;
    private PlayerGrouping pGroup;

    // Start is called before the first frame update
    void Start()
    {
        // anim = GetComponent<Animator>();
        m_ASLObject = gameObject.GetComponent<ASLObject>();
        Debug.Assert(m_ASLObject != null);
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();
        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();
        questions = GameObject.Find("StudentUI").transform.Find("GroupWorld(Clone)").Find("Canvas").Find("StudentPanel").Find("Scroll View").Find("Viewport").Find("Content").gameObject;
        qPanel = GameObject.Find("StudentUI").transform.Find("GroupWorld(Clone)").Find("Canvas").Find("Question").gameObject;
        sg = GameObject.Find("StudentUI").transform.Find("GroupWorld(Clone)").Find("Canvas").Find("Selfgrade").gameObject;

        pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        int playerNumber = 0;
        for (int i = 1; i <= pGroup.m_playerGroups[bgm.getPlayerGroup() - 1].Count; i++)
        {
            if (pGroup.m_playerGroups[bgm.getPlayerGroup() - 1][i - 1] == GameLiftManager.GetInstance().m_PeerId)
            {
                playerNumber = i;
            }
        }
        coin = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Coin").gameObject.GetComponent<CoinFlip>();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.V)){
            QTile();
        }

    }
    void QTile(){
        int children = questions.transform.childCount;
        Debug.Log(children + " number of questions");
        //Random pick a number and get Q and A info from question
        int pick = Random.Range(0, children);
        Debug.Log("Picked question #"+pick);
        //Uses question panel
        string q = questions.transform.GetChild(pick).gameObject.GetComponent<ButtonBehavior>().getQ();
        string a = questions.transform.GetChild(pick).gameObject.GetComponent<ButtonBehavior>().getA();
        Debug.Log(q + "\nAnswer: "+ a);
        qPanel.transform.GetChild(0).GetComponent<Text>().text = q;
        qPanel.GetComponent<QuestionPanel>().setAnswer(a);
        qPanel.SetActive(true);
        qPanel.transform.GetChild(2).gameObject.SetActive(false);
        sg.GetComponent<Selfgrader>().setText(q, "Teacher's Answer: "+a); 
    }

    public void fight(bool win)
    {
        if (win)
        {
            Debug.Log("WON THE FIGHT!");
            DiceRoll.starCount++;
        } else
        {
            Debug.Log("LOST THE FIGHT...");
        }
    }

    public void diceMove()
    {
        for (int i = 0; i < DiceRoll.DiceNumber; i++)
        {
            if (i == 0 && currentTile.split != null)
            {
                currentTile = currentTile.split;
            }
            else
            {
                currentTile = currentTile.next;
            }
        }

        if (currentTile.tag == "StarTile")
        {
            DiceRoll.starCount++;
        }
        else if (currentTile.tag == "DropTile")
        {
            if (DiceRoll.starCount > 0)
            {
                DiceRoll.starCount--;
            }
        } else if (currentTile.tag == "QuestionTile"){
            QTile();
        } else if (currentTile.tag == "FightTile")
        {
            CoinFlip.canFlip = true;
            Debug.Log("CAN FLIP COIN");
        }
        playerData.sendData();

        m_ASLObject.SendAndSetClaim(() =>
        {
            m_ASLObject.SendAndSetLocalPosition(currentTile.transform.localPosition);
        });
    }
}
