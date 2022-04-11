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
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OpenQ);
        buttontxt = "Question: " + this.GetComponentInChildren<Text>().text;
        //questionPanel = GameObject.Find("Question");
        questionPanel = GameObject.Find("Canvas").transform.Find("Question").gameObject;
        question = GameObject.Find("Canvas").transform.Find("Question").Find("QuestionText").GetComponent<Text>();
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
        questionPanel.GetComponent<QuestionPanel>().setAnswer(answer);
        if (questionPanel != null) {  
            bool isActive = questionPanel.activeSelf;  
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
