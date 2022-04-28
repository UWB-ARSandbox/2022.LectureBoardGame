using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class SendNewQuestion : MonoBehaviour
{
    public InputField question;
    public InputField answer;
    public GameObject studentUI;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(readQuestion);
    }

    public void setDataSend()
    {
        if (GameLiftManager.GetInstance().m_PeerId != 1){
            GameObject studentUI = GameObject.Find("StudentUI").transform.Find("GroupWorld(Clone)").gameObject;
            //.Find("Canvas").Find("StudentPanel").Find("Scroll View").Find("Viewport").Find("Content").gameObject;
        }
    }

    private void readQuestion(string _id, float[] _f)
    {
        string printQuestion = "";
        string printAnswer = "";
        bool next = false;
        foreach (float f in _f)
        {
            if (f == -1)
            {
                next = true;
            }
            else
            {
                if (!next)
                {
                    printQuestion += System.Convert.ToChar((int)f);
                }
                else
                {
                    printAnswer += System.Convert.ToChar((int)f);
                }
            }
        }



        // DO SOMETHING WITH THE CREATED QUESTION AND ANSWER HERE
        if (GameLiftManager.GetInstance().m_PeerId != 1)
        {
            Debug.Log(GameLiftManager.GetInstance().m_PeerId + "Received Question: " + printQuestion);
            Debug.Log(GameLiftManager.GetInstance().m_PeerId + "Received Answer: " + printAnswer);
            if(studentUI==null){
                GameObject studentUI = GameObject.Find("StudentUI").transform.Find("GroupWorld(Clone)").gameObject;
            }
            studentUI.GetComponent<Scroll>().createButton(printQuestion, printAnswer);
        }
    }

    public void sendQuestion(string q, string a)
    {
        //question = GameObject.Find("");
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            // one additional float length is for question and answer separator (negative value)
            float[] sendValue = new float[q.Length + 1 + a.Length];
            int index = 0;
            // register question
            foreach (char c in q)
            {
                sendValue[index] = c;
                index++;
            }

            // register separator
            sendValue[index] = -1;
            index++;

            // register answer
            foreach (char c in a)
            {
                sendValue[index] = c;
                index++;
            }

            // send float array
            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }
}
