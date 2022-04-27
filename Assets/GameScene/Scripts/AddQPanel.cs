using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddQPanel : MonoBehaviour
{
    public string question;
    string answer;
    public GameObject publishButton;
    public Button saveButton;
    public InputField QInput;
    public InputField AInput;
    public Button CancelButton;
    public TeacherButton button;
    public Scroll content;

    // Start is called before the first frame update
    void Start()
    {
        saveButton.onClick.AddListener(save);
        publishButton.GetComponent<Button>().onClick.AddListener(publish);
        CancelButton.onClick.AddListener(close);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateQA(string q, string a){
        question = QInput.text = q;
        answer = AInput.text = a;
    }

    void save(){
        question = QInput.text;
        answer = AInput.text;
        if(button!=null){
            button.setQA(question, answer);
        } else {
            GameObject item = content.newQ(question, answer);
        }
        QInput.text = AInput.text = "";
        close();
    }

    void publish(){
        if(button==null){
            GameObject item = content.newQ(question, answer);
            button = item.GetComponent<TeacherButton>();
        }
        button.published = true;
        //Yellow - Color (255,214,0)
        button.gameObject.GetComponent<Image>().color = new Color32(157, 241, 146, 255);
        GameObject DataSend = GameObject.Find("DataSend");
        DataSend.GetComponent<SendNewQuestion>().sendQuestion(QInput.text, AInput.text);
        save();
    }

    void close(){
        publishButton.SetActive(true);
        saveButton.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}