using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Animator anim;
    ASLObject m_ASLObject;
    private string currDirection;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        m_ASLObject = gameObject.GetComponent<ASLObject>();
        Debug.Assert(m_ASLObject != null);
    }

    public void diceMove()
    {
        Vector3 temp = gameObject.transform.position;
        for (int i = 0; i < DiceRoll.DiceNumber; i++)
        {
            if (temp.x >= -4.26f && temp.x < 4.5f && temp.z == 4.26f)
            {
                currDirection = "right";
            }
            else if (temp.z > -4.5f && temp.z <= 4.26f && temp.x == 4.5f)
            {
                currDirection = "down";
            }
            else if (temp.x <= 4.5f && temp.x > -4.26f && temp.z == -4.5f)
            {
                currDirection = "left";
            }
            else if (temp.z < 4.26f && temp.z >= -4.5f && temp.x == -4.26f)
            {
                currDirection = "up";
            }
            temp += moveOnce(currDirection);
        }
        m_ASLObject.SendAndSetClaim(() =>
        {
            m_ASLObject.SendAndSetWorldPosition(temp);
        });
    }

    private Vector3 moveOnce(string direction)
    {
        if (direction.Equals("up"))
        {
            return new Vector3(0, 0, 0.73f);
        }
        else if (direction.Equals("right"))
        {
            return new Vector3(0.73f, 0, 0);
        }
        else if (direction.Equals("down"))
        {
            return new Vector3(0, 0, -0.73f);
        }
        else if (direction.Equals("left"))
        {
            return new Vector3(-0.73f, 0, 0);
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
