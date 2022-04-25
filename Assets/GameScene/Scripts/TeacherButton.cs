using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeacherButton : MonoBehaviour
{

    string question;
    string answer;
    public bool published = false;
    public GameObject editPanel;
    // Start is called before the first frame update
    void Start()
    {
        if(editPanel==null){
            editPanel = GameObject.Find("Canvas").transform.Find("AddQ").gameObject;
        }
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OpenPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setQA(string q, string a){
        question = q;
        answer = a;
        this.GetComponentInChildren<Text>().text = question;
    }

    void OpenPanel(){
        bool open = editPanel.activeSelf;
        if(open){
            //string text = editPanel.GetComponent<script>().question;
            //if (text != question){
                //change question and answer
            //} else {
                //editPanel.SetActive(false);
            //}
            editPanel.SetActive(false);
        } else {
            editPanel.GetComponent<AddQPanel>().updateQA(question,answer);
            editPanel.GetComponent<AddQPanel>().button = this;
            checkPublished();
            editPanel.SetActive(true);
        }
    }

    void checkPublished(){
        GameObject save = GameObject.Find("Canvas").transform.Find("AddQ").Find("Save").gameObject;
        GameObject publish = GameObject.Find("Canvas").transform.Find("AddQ").Find("Publish").gameObject;
        if(published){
            save.SetActive(false);
            publish.SetActive(false);
        }
    }
}