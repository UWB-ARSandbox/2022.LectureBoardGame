using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPanel : MonoBehaviour
{
    private string answer;
    public InputField userInput;
    public Button submitButton;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = submitButton.GetComponent<Button>();
        btn.onClick.AddListener(checkAnswer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setAnswer(string a){
        answer = a;
        Debug.Log("Got answer: " + a);
    }

    void checkAnswer(){
        if(userInput.text == answer){
            Debug.Log("correct");
        }
    }
}
