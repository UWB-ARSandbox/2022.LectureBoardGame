using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Animator anim;
    ASLObject m_ASLObject;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        m_ASLObject = gameObject.GetComponent<ASLObject>();
        Debug.Assert(m_ASLObject != null);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetInteger("movement", 1);
            m_ASLObject.SendAndSetClaim(() =>
                {
                    Vector3 m_AdditiveMovementAmount = Vector3.forward * speed * Time.deltaTime;
                    m_ASLObject.SendAndIncrementWorldPosition(m_AdditiveMovementAmount);
                });
        }

        if (Input.GetKey(KeyCode.S))
        {
            anim.SetInteger("movement", 1);
            m_ASLObject.SendAndSetClaim(() =>
                {
                    Vector3 m_AdditiveMovementAmount = Vector3.back * speed * Time.deltaTime;
                    m_ASLObject.SendAndIncrementWorldPosition(m_AdditiveMovementAmount);
                });
        }

        if (Input.GetKey(KeyCode.D))
        {
            anim.SetInteger("movement", 2);
            m_ASLObject.SendAndSetClaim(() =>
                {
                    Vector3 m_AdditiveMovementAmount = Vector3.right * speed * Time.deltaTime;
                    m_ASLObject.SendAndIncrementWorldPosition(m_AdditiveMovementAmount);
                });
        }

        if (Input.GetKey(KeyCode.A))
        {
            anim.SetInteger("movement", 2);
            m_ASLObject.SendAndSetClaim(() =>
                {
                    Vector3 m_AdditiveMovementAmount = Vector3.left * speed * Time.deltaTime;
                    m_ASLObject.SendAndIncrementWorldPosition(m_AdditiveMovementAmount);
                });
        }

        if(Input.GetKey(KeyCode.G)){
            anim.SetInteger("movement", 0);
        }
    }
}
