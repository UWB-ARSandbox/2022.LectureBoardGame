using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class teacherUI : MonoBehaviour
{
    public Button endGame;
    public Button newQ;
    private bool isHost = false;
    public GameObject addQ;
    // Start is called before the first frame update
    void Start()
    {
        isHost = GameLiftManager.GetInstance().m_PeerId == 1;
        if (isHost)
        {
            newQ.onClick.AddListener(newQuestion);
            endGame.onClick.AddListener(quit);
        } else {
            gameObject.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void quit(){

    }

    void newQuestion(){
        addQ.GetComponent<AddQPanel>().button=null;
        addQ.GetComponent<AddQPanel>().updateQA("","");
        GameObject save = GameObject.Find("Canvas").transform.Find("AddQ").Find("Save").gameObject;
        GameObject publish = GameObject.Find("Canvas").transform.Find("AddQ").Find("Publish").gameObject;
        if(!publish.activeSelf){
            save.SetActive(true);
            publish.SetActive(true);
        }
        addQ.SetActive(true);
    }
}