using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class SendNewQuestion : MonoBehaviour
{
    public Text question;
    public Text answer;

    // Start is called before the first frame update
    private void Start()
    {
        gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(readQuestion);
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
        }
    }

    public void sendQuestion()
    {
        //question = GameObject.Find("");
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            // one additional float length is for question and answer separator (negative value)
            float[] sendValue = new float[question.text.Length + 1 + answer.text.Length];
            int index = 0;
            // register question
            foreach (char c in question.text)
            {
                sendValue[index] = c;
                index++;
            }

            // register separator
            sendValue[index] = -1;
            index++;

            // register answer
            foreach (char c in answer.text)
            {
                sendValue[index] = c;
                index++;
            }

            // send float array
            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }
}
