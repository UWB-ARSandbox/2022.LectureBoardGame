using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class DiceRoll : MonoBehaviour
{
    private static Rigidbody rb;
    public static int DiceNumber;
    public static GameObject DiceView;
    public static bool canRoll;
    public static GameObject DiceDetector;
    private PlayerMovement pm;

    private Vector3 originalPos;

    // Start is called before the first frame update
    void Start()
    {
        DiceNumber = 0;
        canRoll = true;
        rb = GetComponent<Rigidbody>();
        DiceView = GameObject.Find("DiceView");
        DiceView.SetActive(false);
        DiceDetector = GameObject.Find("DiceDetector");
        DiceDetector.SetActive(false);
        Invoke("findPlayer", 1);

        originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    void findPlayer()
    {
        pm = GameObject.Find("Player" + GameLiftManager.GetInstance().m_PeerId + "Piece(Clone)").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canRoll)
        {
            canRoll = false;
            Invoke("tempMethodEnableRoll", 5);
            DiceView.SetActive(true);
            DiceDetector.SetActive(true);
            Invoke("DiceOff", 5);
            Roll();
        }
    }

    private void tempMethodEnableRoll()
    {
        canRoll = true;
    }

    private void DiceOff()
    {
        DiceView.SetActive(false);
        pm.diceMove();
    }

    void Roll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = Vector3.zero;
            float dirX = Random.Range(0, 5000);
            float dirY = Random.Range(0, 5000);
            float dirZ = Random.Range(0, 5000);
            float upForce = Random.Range(200, 500);
            transform.position = originalPos;
            transform.rotation = Quaternion.identity;
            rb.AddForce(transform.up * upForce);
            rb.AddTorque(dirX, dirY, dirZ);
        }
    }
}
