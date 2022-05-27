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
    private GameObject eventLog;
    private GameObject buttonsPanel;
    private GameObject splitButton;
    private GameObject straightButton;
    private int moved;
    private bool splitting;
    private GameObject rental;

    private Vector3 startPos;
    private int counter = 0;
    public SoundManagerScript notification;

    // Start is called before the first frame update
    void Start()
    {
        splitting = false;
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
        notify.GetComponent<NotificationTimer>().enabled = false;
        notifyClose = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("NotificationCloseButton").gameObject;
        notifyClose.SetActive(false);

        eventLog = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("EventLog").Find("LogPanel").Find("Scroll View").Find("Viewport").Find("Content").Find("Text").gameObject;
        bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("EventLog").Find("LogPanel").gameObject.SetActive(false);

        buttonsPanel = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("ButtonsPanel").gameObject;
        buttonsPanel.SetActive(false);

        splitButton = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("SplitButton").gameObject;
        splitButton.SetActive(false);

        straightButton = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("StraightButton").gameObject;
        straightButton.SetActive(false);

        rental = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("Rental").gameObject;

        moved = 0;

        gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(gettingRobbed);
        startPos = transform.localPosition;
        notification = GameObject.Find("SoundManager").GetComponent<SoundManagerScript>();
    }

    public void SplitPath()
    {
        buttonsPanel.SetActive(false);
        splitButton.SetActive(false);
        straightButton.SetActive(false);
        currentTile = currentTile.split;
        moved++;
        Debug.Log(gameObject.ToString() + " Taking Split Path");
        diceMove();
    }

    public void StraightPath()
    {
        buttonsPanel.SetActive(false);
        splitButton.SetActive(false);
        straightButton.SetActive(false);
        currentTile = currentTile.next;
        moved++;
        Debug.Log(gameObject.ToString() + " Taking Straight Path");
        diceMove();
    }



    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.V))
        {
            DiceRoll.movePoints++;
        }*/

        if (transform.localPosition != currentTile.transform.localPosition && !start)
        {
            var step = speed * Time.deltaTime; // calculate distance to move
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, currentTile.transform.localPosition, step);
            m_ASLObject.SendAndSetClaim(() =>
            {
                m_ASLObject.SendAndSetLocalPosition(transform.localPosition);
            });
            if (transform.localPosition == currentTile.transform.localPosition)
            {
                anim.SetInteger("movement", 0);
                gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
                {
                    float[] sendValue = new float[1];
                    sendValue[0] = (float)0;
                    gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
                });
            }
        }

    }


    void QTile()
    {
        int children = questions.transform.childCount;
        Debug.Log(children + " number of questions");
        //Random pick a number and get Q and A info from question
        int pick = Random.Range(0, children);
        Debug.Log("Picked question #" + pick);
        //Uses question panel
        string q = questions.transform.GetChild(pick).gameObject.GetComponent<ButtonBehavior>().getQ();
        string a = questions.transform.GetChild(pick).gameObject.GetComponent<ButtonBehavior>().getA();
        Debug.Log(q + "\nAnswer: " + a);
        qPanel.transform.GetChild(0).GetComponent<Text>().text = q;
        qPanel.GetComponent<QuestionPanel>().setAnswer(a);
        qPanel.SetActive(true);
        qPanel.transform.GetChild(2).gameObject.SetActive(false);
        sg.GetComponent<Selfgrader>().qButton = null;
        sg.GetComponent<Selfgrader>().setText(q, "Teacher's Answer: " + a);
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
                            }
                            else
                            {
                                DiceRoll.starCount += 4;
                                stolenStars = 4;
                            }
                            playerData.sendData();
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player1Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Stole " + stolenStars + " star(s) from " + playerName;
                            notify.SetActive(true);
                            notify.GetComponent<NotificationTimer>().enabled = true;
                            notifyClose.SetActive(true);

                            eventLog.GetComponent<Text>().text += "\nStole " + stolenStars + " star(s) from " + playerName;
                            steal(1);
                        }
                        else
                        {
                            Debug.Log("No stars to be stolen");
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player1Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Tried to steal from " + playerName + ", but no stars to be stolen";
                            notify.SetActive(true);
                            notify.GetComponent<NotificationTimer>().enabled = true;
                            notifyClose.SetActive(true);

                            eventLog.GetComponent<Text>().text += "\nTried to steal from " + playerName + ", but no stars to be stolen";
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
                            notify.GetComponent<NotificationTimer>().enabled = true;
                            notifyClose.SetActive(true);

                            eventLog.GetComponent<Text>().text += "\nStole " + stolenStars + " star(s) from " + playerName;
                            steal(2);
                        }
                        else
                        {
                            Debug.Log("No stars to be stolen");
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player2Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Tried to steal from " + playerName + ", but no stars to be stolen";
                            notify.SetActive(true);
                            notify.GetComponent<NotificationTimer>().enabled = true;
                            notifyClose.SetActive(true);

                            eventLog.GetComponent<Text>().text += "\nTried to steal from " + playerName + ", but no stars to be stolen";
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
                            notify.GetComponent<NotificationTimer>().enabled = true;
                            notifyClose.SetActive(true);

                            eventLog.GetComponent<Text>().text += "\nStole " + stolenStars + " star(s) from " + playerName;
                            steal(3);
                        }
                        else
                        {
                            Debug.Log("No stars to be stolen");
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player3Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Tried to steal from " + playerName + ", but no stars to be stolen";
                            notify.SetActive(true);
                            notify.GetComponent<NotificationTimer>().enabled = true;
                            notifyClose.SetActive(true);

                            eventLog.GetComponent<Text>().text += "\nTried to steal from " + playerName + ", but no stars to be stolen";
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
                            notify.GetComponent<NotificationTimer>().enabled = true;
                            notifyClose.SetActive(true);

                            eventLog.GetComponent<Text>().text += "\nStole " + stolenStars + " star(s) from " + playerName;
                            steal(4);
                        }
                        else
                        {
                            Debug.Log("No stars to be stolen");
                            string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player4Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                            notify.transform.Find("Text").GetComponent<Text>().text = "Tried to steal from " + playerName + ", but no stars to be stolen";
                            notify.SetActive(true);
                            notify.GetComponent<NotificationTimer>().enabled = true;
                            notifyClose.SetActive(true);

                            eventLog.GetComponent<Text>().text += "\nTried to steal from " + playerName + ", but no stars to be stolen";
                        }
                        break;
                }
            }
            else
            {
                notify.transform.Find("Text").GetComponent<Text>().text = "No players to steal from";
                notify.SetActive(true);
                notify.GetComponent<NotificationTimer>().enabled = true;
                notifyClose.SetActive(true);
                eventLog.GetComponent<Text>().text += "\nTried to steal, but no players to steal from";
                Debug.Log("No players to steal from...");
            }
        }
        else
        {
            notify.transform.Find("Text").GetComponent<Text>().text = "Failed to steal...";
            notify.SetActive(true);
            notify.GetComponent<NotificationTimer>().enabled = true;
            notifyClose.SetActive(true);
            eventLog.GetComponent<Text>().text += "\nTried to steal, but failed";
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

    //Also used to send animation info
    private void gettingRobbed(string _id, float[] _f)
    {

        //test
        if (_f.Length == 1)
        {
            anim.SetInteger("movement", (int)_f[0]);
        }
        else
        {
            Debug.Log("GETTING ROBBED");
            if (_f[0] == bgm.getPlayerGroup() && _f[1] == playerNumber)
            {
                string playerName = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player" + _f[2] + "Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text;
                notify.transform.Find("Text").GetComponent<Text>().text = playerName + " has stolen 4 stars!";
                notify.SetActive(true);
                notify.GetComponent<NotificationTimer>().enabled = true;
                notifyClose.SetActive(true);
                eventLog.GetComponent<Text>().text += "\n" + playerName + " has stolen 4 stars";
                //Not sure if being robbed and losing stars due to tile should be the same sound
                notification.tileNotification();
                Debug.Log("GOT ROBBEDDD");
                if (DiceRoll.starCount < 4)
                {
                    DiceRoll.starCount = 0;
                }
                else
                {
                    DiceRoll.starCount -= 4;
                }
                playerData.sendData();
            }
        }
    }

    private void showOptions()
    {
        buttonsPanel.SetActive(true);
        splitButton.SetActive(true);
        straightButton.SetActive(true);
    }

    private void displayRental()
    {
        rental.SetActive(true);
    }

    public void renting()
    {
        if (DiceRoll.starCount >= 2)
        {
            DiceRoll.starCount -= 2;
            // SET currentTile TAG AS RentedTile
            currentTile.gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
            {
                //Send and then set (once received - NOT here) the tag
                currentTile.gameObject.GetComponent<ASLObject>().SendAndSetTag("RentedTile");
            });

            // MARK currentTile WITH PREFAB
            ASLHelper.InstantiateASLObject("RentedMark" + playerNumber, new Vector3(0, 0.0001f, 0), Quaternion.Euler(90f, 0, 0), currentTile.gameObject.GetComponent<ASLObject>().m_Id);

            notify.transform.Find("Text").GetComponent<Text>().text = "Spent 2 stars on a tile rental.";
            notify.SetActive(true);
            notify.GetComponent<NotificationTimer>().enabled = true;
            notifyClose.SetActive(true);
            eventLog.GetComponent<Text>().text += "\nSpent 2 stars on a tile rental";
        } else
        {
            notify.transform.Find("Text").GetComponent<Text>().text = "Not enough stars to rent out the tile";
            notify.SetActive(true);
            notify.GetComponent<NotificationTimer>().enabled = true;
            notifyClose.SetActive(true);
            eventLog.GetComponent<Text>().text += "\nFailed rental due to lack of stars";
        }
        playerData.sendData();
    }

    public void diceMove()
    {
        start = false;
        anim.SetInteger("movement", currentTile.animation);
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[1];
            sendValue[0] = (float)currentTile.animation;
            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });

        // GENERAL PLAYERPIECE MOVEMENT
        int needToMove = 0;
        if (splitting)
        {
            needToMove = DiceRoll.DiceNumber - moved;
            splitting = false;
        }
        else
        {
            needToMove = DiceRoll.DiceNumber;
            moved = 0;
        }
        Debug.Log(needToMove);
        for (int i = 0; i < needToMove; i++)
        {
            if (currentTile.tag == "SplitTile")
            {
                splitting = true;
                showOptions();
                moved = i;
                break;
            }
            else
            {
                moved = 0;
                currentTile = currentTile.next;
            }
        }


        // DIFFERENT TILE INTERACTIONS
        if (currentTile.tag == "StarTile")
        {
            DiceRoll.starCount += 2;
            notification.getStars();
            notify.transform.Find("Text").GetComponent<Text>().text = "Got 2 stars!";
            notify.SetActive(true);
            notify.GetComponent<NotificationTimer>().enabled = true;
            notifyClose.SetActive(true);
            eventLog.GetComponent<Text>().text += "\nGained 2 stars";
        }
        else if (currentTile.tag == "DropTile")
        {
            if (DiceRoll.starCount > 0)
            {
                DiceRoll.starCount--;
                notification.dropStars();
                notify.transform.Find("Text").GetComponent<Text>().text = "Lost a star...";
                notify.SetActive(true);
                notify.GetComponent<NotificationTimer>().enabled = true;
                notifyClose.SetActive(true);
                eventLog.GetComponent<Text>().text += "\nLost a star";
            }
        }
        else if (currentTile.tag == "QuestionTile")
        {
            notification.tileNotification();
            QTile();
            eventLog.GetComponent<Text>().text += "\nLanded on question tile";
        }
        else if (currentTile.tag == "FightTile")
        {
            CoinFlip.canFlip = true;
        }
        else if (currentTile.tag == "TeleportTile")
        {
            notify.transform.Find("Text").GetComponent<Text>().text = "Teleporting to a random location!";
            notify.SetActive(true);
            notify.GetComponent<NotificationTimer>().enabled = true;
            notifyClose.SetActive(true);
            notification.tileNotification();
            eventLog.GetComponent<Text>().text += "\nTeleported to new location";
            Invoke("teleporting", 1.5f);
        }
        else if (currentTile.tag == "DiceTile")
        {
            DiceRoll.movePoints++;
            notify.transform.Find("Text").GetComponent<Text>().text = "Got 1 Move Point!";
            notify.SetActive(true);
            notify.GetComponent<NotificationTimer>().enabled = true;
            notifyClose.SetActive(true);
            eventLog.GetComponent<Text>().text += "\nGained a Move Point";
        }
        else if (currentTile.tag == "NormalTile")
        {
            // SHOW OPTION TO RENT IT OUT FOR 2 STARS
            displayRental();
        }
        else if (currentTile.tag == "RentedTile")
        {
            Debug.Log("LANDED ON RENTED TILE");
            // LOSE 4 STARS
            if (currentTile.transform.Find("RentedMark" + playerNumber + "(Clone)") == null)
            {
                DiceRoll.starCount -= 4;
                if (DiceRoll.starCount < 0)
                {
                    DiceRoll.starCount = 0;
                }
                notify.transform.Find("Text").GetComponent<Text>().text = "Landed on a rented space and lost 4 stars!";
                notify.SetActive(true);
                notify.GetComponent<NotificationTimer>().enabled = true;
                notifyClose.SetActive(true);
                eventLog.GetComponent<Text>().text += "\nLanded on a rented tile and lost 4 stars.";
            }
        }
        playerData.sendData();

        /*Debug.Log(currentTile.ToString());
        m_ASLObject.SendAndSetClaim(() =>
        {
            m_ASLObject.SendAndSetLocalPosition(currentTile.transform.localPosition);
        });*/
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
