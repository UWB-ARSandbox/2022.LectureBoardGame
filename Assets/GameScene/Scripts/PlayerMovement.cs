using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Animator anim;
    ASLObject m_ASLObject;
    public string currDirection;
    private bool split = false;
    private BoardGameManager bgm;
    public static Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        m_ASLObject = gameObject.GetComponent<ASLObject>();
        Debug.Assert(m_ASLObject != null);
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();
    }

    public void diceMove()
    {
        Vector3 temp = gameObject.transform.localPosition;
        temp -= offset;
        for (int i = 0; i < DiceRoll.DiceNumber; i++)
        {
            // Debug.Log(temp.x + "   " + temp.z + "     " + split);

            // if landed exactly on the split, player will split off
            Debug.Log(temp);
            if ((i == 0 && temp.x == -4.26f && temp.z == -2.31f) || (i == 0 && temp.x == 4.5f && temp.z == 2.07f) ||
                (i == 0 && temp.x == -4.3f && temp.z == -2.3f) || (i == 0 && temp.x == 4.5f && temp.z == 2.1f))
            {
                split = true;
            }

            if ((split || i == 0) && temp.x >= -4.26f && temp.x < -2.08f && temp.z == -2.31f)
            {
                currDirection = "right";
                split = true;
            }
            else if ((split || i == 0) && ((temp.x <= -2.06f && temp.z >= -2.31f && temp.z < 2.07f) || (temp.x == -2.1f && temp.z == -2.3f)))
            {
                currDirection = "up";
                split = true;
            }
            else if ((split || i == 0) && ((temp.x > -4.26f && temp.x <= -2.07f && temp.z <= 2.08f) || (temp.x == -2.1f && (temp.z == (float)2.07 || temp.z == (float)2.1))))
            {
                currDirection = "left";
                split = true;
            }
            else if ((split || i == 0) && ((temp.x == -4.26f && temp.z == 2.07f) || (temp.x == -4.3f && temp.z == 2.1f)))
            {
                currDirection = "up";
                split = false;
            }
            else if ((split || i == 0) && ((temp.x <= 4.5f && temp.x > 2.31f && temp.z == 2.07f) || (temp.x == 4.5f && temp.z == 2.1f)))
            {
                currDirection = "left";
                split = true;
            }
            else if ((split || i == 0) && ((temp.x == 2.31f && temp.z <= 2.07f && temp.z > -2.31f) || (temp.x == 2.3f && temp.z == 2.1f)))
            {
                currDirection = "down";
                split = true;
            }
            else if ((split || i == 0) && ((temp.x < 4.5f && temp.x >= 2.31f && temp.z == -2.31f) || (temp.x == 2.3f && temp.z == -2.3f)))
            {
                currDirection = "right";
                split = true;
            }
            else if ((split || i == 0) && ((temp.x == 4.5f && temp.z == -2.31f) || (temp.x == 4.5f && temp.z == -2.3f)))
            {
                currDirection = "down";
                split = false;
            }
            else if ((temp.x >= -4.26f && temp.x < 4.5f && temp.z == 4.26f) || (temp.x == -4.3f && temp.z == 4.3f))
            {
                currDirection = "right";
                split = false;
            }
            else if (temp.z > -4.5f && temp.z <= 4.26f && temp.x == 4.5f)
            {
                currDirection = "down";
                split = false;
            }
            else if ((temp.x <= 4.5f && temp.x > -4.26f && temp.z == -4.5f) || (temp.x == 4.5f && temp.z == -4.5f))
            {
                currDirection = "left";
                split = false;
            }
            else if ((temp.z < 4.26f && temp.z >= -4.5f && temp.x == -4.26f) || (temp.x == -4.3f && temp.z == -4.5f))
            {
                currDirection = "up";
                split = false;
            }
            temp += moveOnce(currDirection);
        }
        m_ASLObject.SendAndSetClaim(() =>
        {
            temp += offset;
            m_ASLObject.SendAndSetLocalPosition(temp);
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
}
