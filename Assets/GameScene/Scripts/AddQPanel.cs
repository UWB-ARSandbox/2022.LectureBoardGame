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
    public TeacherButton button;

    // Start is called before the first frame update
    void Start()
    {
        saveButton.onClick.AddListener(save);
        
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
        }
        QInput.text = AInput.text = "";
        gameObject.SetActive(false);
    }
}
