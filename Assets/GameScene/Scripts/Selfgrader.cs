using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class Selfgrader : MonoBehaviour
{
    string question;
    string teacher;
    string student;
    public Button qButton;
    public Button correct;
    public Button incorrect;
    public Text questionTxt;
    public Text teacherAnswer;
    public Text studentAnswer;
    public MarkAnswer ma;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = correct.GetComponent<Button>();
        btn.onClick.AddListener(markCorrect);
        Button btn2 = incorrect.GetComponent<Button>();
        btn2.onClick.AddListener(markIncorrect);

        ma = GameObject.Find("DataSend").GetComponent<MarkAnswer>();
        gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(sendResult);
    }

    private void sendResult(string _id, float[] _f)
    {
        foreach (float f in _f)
        {
            Debug.Log(f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setText(string q, string ta, string submit){
        question = q;
        teacher = ta;
        student = submit;
    }

    public void setText(string q, string a){
        question = q;
        teacher = a;
        questionTxt.text = q;
        teacherAnswer.text = a;
    }

    public void studentSubmit(string a){
        student = "Your Answer: " + a;
        studentAnswer.text = "Your Answer: " + a;
    }

    void markCorrect(){
        qButton.GetComponent<Image>().color = correct.image.color;
        qButton.GetComponent<ButtonBehavior>().answered = true;
        DiceRoll.movePoints++;

        ma.markCorrect(questionTxt.text);

        gameObject.SetActive(false);
    }

    void markIncorrect(){
        qButton.GetComponent<Image>().color = incorrect.image.color;
        qButton.GetComponent<ButtonBehavior>().answered = true;

        ma.markIncorrect(questionTxt.text);

        gameObject.SetActive(false);
    }
}
