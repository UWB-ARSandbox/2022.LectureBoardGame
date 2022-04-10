using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetInteger("movement", 1);
            pos.z += speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            anim.SetInteger("movement", 1);
            pos.z -= speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            anim.SetInteger("movement", 2);
            pos.x += speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            anim.SetInteger("movement", 2);
            pos.x -= speed * Time.deltaTime;
        }

        transform.position = pos;
        if(Input.GetKey(KeyCode.G)){
            anim.SetInteger("movement", 0);
        }
        
    }
}
