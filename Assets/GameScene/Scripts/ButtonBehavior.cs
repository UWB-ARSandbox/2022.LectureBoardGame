using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour
{
    public GameObject questionPanel;
    //Question on the question panel
    public Text question;
    //Button's question text
    private string buttontxt;
    private string answer;
    //Selfgrade
    public Text question2;
    public Text answerTxt;
    public GameObject sg;
    public bool answered = false;
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OpenQ);
        buttontxt = "Question: " + this.GetComponentInChildren<Text>().text;
        //questionPanel = GameObject.Find("Question");
        questionPanel = GameObject.Find("Canvas").transform.Find("Question").gameObject;
        question = GameObject.Find("Canvas").transform.Find("Question").Find("QuestionText").GetComponent<Text>();
        question2 = GameObject.Find("Canvas").transform.Find("Selfgrade").Find("QuestionText").GetComponent<Text>();
        answerTxt = GameObject.Find("Canvas").transform.Find("Selfgrade").Find("TeacherAnswer").GetComponent<Text>();
        sg = GameObject.Find("Canvas").transform.Find("Selfgrade").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setQA(string q, string a){
        this.GetComponentInChildren<Text>().text = q;
        answer = a;
        buttontxt = "Question: " + q; 
    }

    void OpenQ(){
        if(answered && sg!=null){
            GameObject correct = GameObject.Find("Canvas").transform.Find("Selfgrade").Find("Correct").gameObject;
            GameObject incorrect = GameObject.Find("Canvas").transform.Find("Selfgrade").Find("Incorrect").gameObject;
            if(!sg.activeSelf){
                sg.SetActive(true);
                correct.SetActive(false);
                incorrect.SetActive(false);
            } else {
                correct.SetActive(true);
                incorrect.SetActive(true);
                sg.SetActive(false);
            }
        } else if (questionPanel != null && !sg.activeSelf) {  
            questionPanel.GetComponent<QuestionPanel>().setAnswer(answer);
            bool isActive = questionPanel.activeSelf;
            sg.GetComponent<Selfgrader>().qButton = GetComponent<Button>();
            sg.GetComponent<Selfgrader>().setText(buttontxt, "Answer: "+answer);  
            if(isActive){
                string q = question.text;
                if (buttontxt == q){
                    questionPanel.SetActive(!isActive); 
                } else {
                    question.text = buttontxt;
                }
            } else {
                questionPanel.SetActive(!isActive);
                question.text = buttontxt;
            }             
        } 
    }
}
