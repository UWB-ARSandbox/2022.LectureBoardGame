using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    ASLObject m_ASLObject;
    public string currDirection;
    private BoardGameManager bgm;
    public static TileNode currentTile;
    public TileNode pastTile;
    private PlayerData playerData;
    public GameObject questions;
    public GameObject qPanel;
    public GameObject sg;
    public CoinFlip coin;
    private PlayerGrouping pGroup;
    private PlayerData pd;
    private int playerNumber;
    private GameObject notify;
    private GameObject notifyClose;
    public float speed = 1f;
    private bool start = true;

    private Vector3 startPos;
    private int counter = 0;
    public SoundManagerScript notification;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        m_ASLObject = gameObject.GetComponent<ASLObject>();
        Debug.Assert(m_ASLObject != null);
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();
        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();
        questions = GameObject.Find("StudentUI").transform.Find("GroupWorld(Clone)").Find("Canvas").Find("StudentPanel").Find("Scroll View").Find("Viewport").Find("Content").gameObject;
        qPanel = GameObject.Find("StudentUI").transform.Find("GroupWorld(Clone)").Find("Canvas").Find("Question").gameObject;
        sg = GameObject.Find("StudentUI").transform.Find("GroupWorld(Clone)").Find("Canvas").Find("Selfgrade").gameObject;

        pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        coin = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Coin").gameObject.GetComponent<CoinFlip>();
        pd = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();
        for (int i = 1; i <= pGroup.m_playerGroups[bgm.getPlayerGroup() - 1].Count; i++)
        {
            if (pGroup.m_playerGroups[bgm.getPlayerGroup() - 1][i - 1] == GameLiftManager.GetInstance().m_PeerId)
            {
                playerNumber = i;
            }
        }
        notify = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("Notification").gameObject;
        notify.SetActive(false);
        notifyClose = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("NotificationCloseButton").gameObject;
        notifyClose.SetActive(false);

        gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(gettingRobbed);
        startPos = transform.localPosition;
        notification = GameObject.Find("SoundManager").GetComponent<SoundManagerScript>();
    }

    void Update(){
        /*if(Input.GetKeyDown(KeyCode.V)){
            CoinFlip.canFlip = true;
        }*/

        if (transform.localPosition!=currentTile.transform.localPosition&&!start){
            var step =  speed * Time.deltaTime; // calculate distance to move
            if(pastTile!=null){
                animation();
            }
            transform.localPosition = Vector3.MoveTowards(transform.localPosition,currentTile.transform.localPosition, step);
            m_ASLObject.SendAndSetClaim(() =>
            {
                m_ASLObject.SendAndSetLocalPosition(transform.localPosition);
            });
            if(transform.localPosition==currentTile.transform.localPosition){
                anim.SetInteger("movement", 0);
            }
        }

    }

    void animation(){
        if(pastTile.transform.localPosition.z == -4.26 || pastTile.transform.localPosition.z == 4.5 || 
            ((pastTile.transform.localPosition.z==-2.07||pastTile.transform.localPosition.z==2.31)&&(pastTile.transform.localPosition.x != 4.26 || pastTile.transform.localPosition.x != -4.5))){
            //Horizontal: Left and Right
            anim.SetInteger("movement", 2);
        } else if (pastTile.transform.localPosition.x == 4.26 || pastTile.transform.localPosition.x == -4.5 || 
            ((pastTile.transform.localPosition.x==2.07||pastTile.transform.localPosition.x==-2.31)&&(pastTile.transform.localPosition.z != -4.26 || pastTile.transform.localPosition.z != 4.5))){
            //Vertical: Up and Down
            anim.SetInteger("movement", 1);
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
        sg.GetComponent<Selfgrader>().qButton = null;
        sg.GetComponent<Selfgrader>().setText(q, "Teacher's Answer: "+a); 
    }

    

    public void fight(bool win)
    {
        if (win)
        {
            Debug.Log("WON THE FIGHT!");
            Transform world = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane");
            if (world.childCount >= 72)
            {
                int victimNum = Random.Range(70, world.childCount);
                while (victimNum - 69 == playerNumber)
                {
                    victimNum = Random.Range(70, world.childCount);
                }

                Debug.Log("Stealing from player " + (victimNum - 69));
                switch (victimNum - 69)
                {
                    case 1:
                        if (pd.p1Stars > 0)
                        {
                            Debug.Log("STEALING");
                            int stolenStars = 0;
                            if (pd.p1Stars < 4)
                            {
                                DiceRoll.starCount += pd.p1Stars;
                                stolenStars = pd.p1Stars;
                            } else
                            {
                                DiceRoll.starCount += 4;
                                stolenStars = 4;
                            }
                            playerData.sendData();
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player1Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Stole " + stolenStars + " star(s) from " + playerName;
                            notify.SetActive(true);
                            notifyClose.SetActive(true);
                            steal(1);
                        }
                        else
                        {
                            Debug.Log("No stars to be stolen");
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player1Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Tried to steal from " + playerName + ", but no stars to be stolen";
                            notify.SetActive(true);
                            notifyClose.SetActive(true);
                        }
                        break;

                    case 2:
                        if (pd.p2Stars > 0)
                        {
                            Debug.Log("STEALING");
                            int stolenStars = 0;
                            if (pd.p2Stars < 4)
                            {
                                DiceRoll.starCount += pd.p2Stars;
                                stolenStars = pd.p2Stars;
                            }
                            else
                            {
                                DiceRoll.starCount += 4;
                                stolenStars = 4;
                            }
                            playerData.sendData();
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player2Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Stole " + stolenStars + " star(s) from " + playerName;
                            notify.SetActive(true);
                            notifyClose.SetActive(true);
                            steal(2);
                        }
                        else
                        {
                            Debug.Log("No stars to be stolen");
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player2Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Tried to steal from " + playerName + ", but no stars to be stolen";
                            notify.SetActive(true);
                            notifyClose.SetActive(true);
                        }
                        break;

                    case 3:
                        if (pd.p3Stars > 0)
                        {
                            Debug.Log("STEALING");
                            int stolenStars = 0;
                            if (pd.p3Stars < 4)
                            {
                                DiceRoll.starCount += pd.p3Stars;
                                stolenStars = pd.p3Stars;
                            }
                            else
                            {
                                DiceRoll.starCount += 4;
                                stolenStars = 4;
                            }
                            playerData.sendData();
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player3Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Stole " + stolenStars + " star(s) from " + playerName;
                            notify.SetActive(true);
                            notifyClose.SetActive(true);
                            steal(3);
                        }
                        else
                        {
                            Debug.Log("No stars to be stolen");
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player3Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Tried to steal from " + playerName + ", but no stars to be stolen";
                            notify.SetActive(true);
                            notifyClose.SetActive(true);
                        }
                        break;

                    case 4:
                        if (pd.p4Stars > 0)
                        {
                            Debug.Log("STEALING");
                            int stolenStars = 0;
                            if (pd.p4Stars < 4)
                            {
                                DiceRoll.starCount += pd.p4Stars;
                                stolenStars = pd.p4Stars;
                            }
                            else
                            {
                                DiceRoll.starCount += 4;
                                stolenStars = 4;
                            }
                            playerData.sendData();
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player4Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Stole " + stolenStars + " star(s) from " + playerName;
                            notify.SetActive(true);
                            notifyClose.SetActive(true);
                            steal(4);
                        }
                        else
                        {
                            Debug.Log("No stars to be stolen");
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player4Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Tried to steal from " + playerName + ", but no stars to be stolen";
                            notify.SetActive(true);
                            notifyClose.SetActive(true);
                        }
                        break;
                }
            } else
            {
                notify.transform.Find("Text").GetComponent<Text>().text = "No players to steal from";
                notify.SetActive(true);
                notifyClose.SetActive(true);
                Debug.Log("No players to steal from...");
            }
        } else
        {
            notify.transform.Find("Text").GetComponent<Text>().text = "You lost";
            notify.SetActive(true);
            notifyClose.SetActive(true);
            Debug.Log("LOST THE FIGHT...");
        }
        //unsure if we want to use different audios if they win or lose
        notification.tileNotification();
    }

    private void steal(int victim)
    {
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[3];
            sendValue[0] = bgm.getPlayerGroup();
            sendValue[1] = victim;
            sendValue[2] = playerNumber;

            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }

    private void gettingRobbed(string _id, float[] _f)
    {
        Debug.Log("GETTING ROBBED");
        if (_f[0] == bgm.getPlayerGroup() && _f[1] == playerNumber)
        {
            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player" + _f[2] + "Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
            notify.transform.Find("Text").GetComponent<Text>().text = playerName + " has stolen 4 stars!";
            notify.SetActive(true);
            notifyClose.SetActive(true);
            //Not sure if being robbed and losing stars due to tile should be the same sound
            notification.tileNotification();
            Debug.Log("GOT ROBBEDDD");
            if (DiceRoll.starCount < 4)
            {
                DiceRoll.starCount = 0;
            } else
            {
                DiceRoll.starCount -= 4;
            }
            playerData.sendData();
        }
    }

    public void diceMove()
    {
        start = false;
        pastTile = currentTile;
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
            DiceRoll.starCount += 2;
            notification.getStars();
        }
        else if (currentTile.tag == "DropTile")
        {
            if (DiceRoll.starCount > 0)
            {
                DiceRoll.starCount--;
                notification.dropStars();
            }
        } else if (currentTile.tag == "QuestionTile"){
            notification.tileNotification();
            QTile();
        } else if (currentTile.tag == "FightTile")
        {
            CoinFlip.canFlip = true;
        } else if (currentTile.tag == "TeleportTile")
        {
            notify.transform.Find("Text").GetComponent<Text>().text = "Teleporting to a random location!";
            notify.SetActive(true);
            notifyClose.SetActive(true);
            notification.tileNotification();
            Invoke("teleporting", 1.5f);
        }
        playerData.sendData();

        //m_ASLObject.SendAndSetClaim(() =>
        //{
            //m_ASLObject.SendAndSetLocalPosition(currentTile.transform.localPosition);
        //});
    }

    void teleporting()
    {
        int randomTile = Random.Range(0, 69);
        currentTile = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").GetChild(randomTile).GetComponent<TileNode>();
        m_ASLObject.SendAndSetClaim(() =>
        {
            m_ASLObject.SendAndSetLocalPosition(currentTile.transform.localPosition);
        });
    }
}
