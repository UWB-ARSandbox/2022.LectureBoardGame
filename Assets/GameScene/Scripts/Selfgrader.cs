using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    // Start is called before the first frame update
    void Start()
    {
        Button btn = correct.GetComponent<Button>();
        btn.onClick.AddListener(markCorrect);
        Button btn2 = incorrect.GetComponent<Button>();
        btn2.onClick.AddListener(markIncorrect);
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
        Debug.Log("correct");
        qButton.GetComponent<Image>().color = correct.image.color;
        gameObject.SetActive(false);
    }

    void markIncorrect(){
        Debug.Log("correct");
        Vector4 myButtonsColor = incorrect.image.color;
        qButton.GetComponent<Image>().color = incorrect.image.color;
        gameObject.SetActive(false);
    }
}
