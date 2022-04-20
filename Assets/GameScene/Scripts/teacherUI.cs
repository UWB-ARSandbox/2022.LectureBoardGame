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
    public Button exitQ;
    // Start is called before the first frame update
    void Start()
    {
        isHost = GameLiftManager.GetInstance().m_PeerId == 1;
        if (isHost)
        {
            newQ.onClick.AddListener(newQuestion);
            endGame.onClick.AddListener(quit);
            exitQ.onClick.AddListener(closeq);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void quit(){

    }

    void newQuestion(){
        addQ.SetActive(true);
    }

    void closeq(){
        addQ.SetActive(false);
    }
}
