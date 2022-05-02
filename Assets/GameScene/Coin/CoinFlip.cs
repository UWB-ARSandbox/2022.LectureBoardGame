using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class CoinFlip : MonoBehaviour
{
    private Vector3 originalPos;
    private static Rigidbody rb;
    public static GameObject CoinView;
    public static GameObject CoinDetector;
    public static bool good;
    private BoardGameManager bgm;
    private PlayerGrouping pGroup;
    private PlayerMovement pm;
    public static bool canFlip;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CoinView = GameObject.Find("CoinView");
        CoinView.SetActive(false);
        CoinDetector = GameObject.Find("CoinDetector");
        CoinDetector.SetActive(false);
        originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        good = true;
        pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();

        Invoke("findPlayer", 1);
    }

    void findPlayer()
    {
        if (GameLiftManager.GetInstance().m_PeerId != 1)
        {
            int playerNumber = 0;
            for (int i = 1; i <= pGroup.m_playerGroups[bgm.getPlayerGroup() - 1].Count; i++)
            {
                if (pGroup.m_playerGroups[bgm.getPlayerGroup() - 1][i - 1] == GameLiftManager.GetInstance().m_PeerId)
                {
                    playerNumber = i;
                }
            }
            pm = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player" + playerNumber + "Piece(Clone)").GetComponent<PlayerMovement>();
        }
    }

    void Flip()
    {
        CoinView.SetActive(true);
        rb.velocity = Vector3.zero;
        float dirX = Random.Range(0, 5000);
        float dirY = Random.Range(0, 5000);
        float dirZ = Random.Range(0, 5000);
        float upForce = Random.Range(200, 500);
        transform.position = originalPos;
        transform.rotation = Quaternion.identity;
        rb.AddForce(transform.up * upForce);
        rb.AddTorque(dirX, dirY, dirZ);
        Invoke("coinOff", 3.5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canFlip)
        {
            canFlip = false;
            CoinView.SetActive(true);
            CoinDetector.SetActive(true);
            Invoke("coinOff", 5);
            Flip();
        }
    }

    void coinOff()
    {
        CoinView.SetActive(false);
        pm.fight(good);
    }
}
