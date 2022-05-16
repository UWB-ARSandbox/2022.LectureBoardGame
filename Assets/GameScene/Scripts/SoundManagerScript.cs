using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{

    public AudioClip notiSound;
    public AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        //notiSound = Resource.Load<AudioClip> ("notiSound");
        audioSrc = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V)){
            playSound();
        }
    }

    public void playSound(){
        audioSrc.PlayOneShot(notiSound);
    }
}
