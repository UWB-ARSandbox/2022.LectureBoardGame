using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class PlayerMovement : MonoBehaviour
{
    // private Animator anim;
    ASLObject m_ASLObject;
    public string currDirection;
    private BoardGameManager bgm;
    public static TileNode currentTile;

    // Start is called before the first frame update
    void Start()
    {
        // anim = GetComponent<Animator>();
        m_ASLObject = gameObject.GetComponent<ASLObject>();
        Debug.Assert(m_ASLObject != null);
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();
    }

    public void diceMove()
    {
        for (int i = 0; i < DiceRoll.DiceNumber; i++)
        {
            if (i == 0 && currentTile.split != null)
            {
                currentTile = currentTile.split;
            } else
            {
                currentTile = currentTile.next;
            }
        }
        m_ASLObject.SendAndSetClaim(() =>
        {
            m_ASLObject.SendAndSetLocalPosition(currentTile.transform.localPosition);
        });
    }
}
