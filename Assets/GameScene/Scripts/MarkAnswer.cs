using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class MarkAnswer : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(sendResult);
    }

    public void sendResult(string _id, float[] _f)
    {
        Debug.Log("RECEIVED BY " + GameLiftManager.GetInstance().m_PeerId);
        if (GameLiftManager.GetInstance().m_PeerId == 1)
        {
            string question = "";
            bool correct = false;
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
                        question += System.Convert.ToChar((int)f);
                    }
                    else if ((int)f == 0)
                    {
                        correct = false;
                    }
                    else if ((int)f == 1)
                    {
                        correct = true;
                    }
                }
            }

            Debug.Log(question + "   " + correct);

            // TRANSFER DATA TO TEACHER UI
        }
    }
    public void markCorrect(string questionTxt)
    {
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            Debug.Log("SENDING CORRECT");
            float[] sendValue = new float[questionTxt.Length + 2];
            int index = 0;
            foreach (char c in questionTxt)
            {
                sendValue[index] = c;
                index++;
            }

            // register separator
            sendValue[index] = -1;
            index++;

            // register as correct
            sendValue[index] = 1;

            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }

    public void markIncorrect(string questionTxt)
    {
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            Debug.Log("SENDING INCORRECT");
            float[] sendValue = new float[questionTxt.Length + 1];
            int index = 0;
            foreach (char c in questionTxt)
            {
                sendValue[index] = c;
                index++;
            }

            // register separator
            sendValue[index] = -1;
            index++;

            // register as incorrect
            sendValue[index] = 0;

            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }
}
