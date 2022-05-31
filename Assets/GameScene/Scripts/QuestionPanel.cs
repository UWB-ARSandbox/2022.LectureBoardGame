using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPanel : MonoBehaviour
{
    private string answer;
    public InputField userInput;
    public Button submitButton;
    public GameObject selfgrade;
    public Button closeButton;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = submitButton.GetComponent<Button>();
        btn.onClick.AddListener(checkAnswer);
        Button btn2 = closeButton.GetComponent<Button>();
        btn2.onClick.AddListener(close);
        if (selfgrade == null)
            selfgrade = GameObject.Find("Canvas").transform.Find("Selfgrade").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setAnswer(string a){
        answer = a;
    }

    void checkAnswer(){
        if (selfgrade != null) {
            bool isActive = selfgrade.activeSelf;
            selfgrade.SetActive(!isActive);
            selfgrade.GetComponent<Selfgrader>().studentSubmit(userInput.text);
            userInput.text = "";
            selfgrade.GetComponent<Selfgrader>().activeButtons();
            close();
        }
    }

    void close(){
        if(!closeButton.gameObject.activeSelf){
            closeButton.gameObject.SetActive(true);
        }
        gameObject.SetActive(false);
    }
}
