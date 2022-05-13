using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudentAnswerButton : MonoBehaviour
{
    private string studentAnswer;
    private string studentName;
    private StudentAnswersPanel saPanel;
    private EndGameUI endGameUI;
    private Color32 color;
    //only for student use
    private string question;
    private string answer;
    public int questionIndex;
    private int selfGrade;

    private Color32 red;
    private Color32 green;
    private Color32 badColor;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(setText);
        color = new Color32(255, 255, 255, 255); //white
        red = new Color32(255, 138, 146, 255);
        green = new Color32(198, 255, 138, 255);
        badColor = new Color32(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Image>().color == badColor)
        {
            GetComponent<Image>().color = getColor(selfGrade);
        }
    }

    public void setup(string username, string answer, int selfGrade, StudentAnswersPanel sa)
    {
        studentName = username;
        studentAnswer = answer;
        this.selfGrade = selfGrade;
        saPanel = sa;
        GetComponent<Image>().color = color;
        transform.GetChild(0).gameObject.GetComponent<Text>().text = username;
    }

    public void setup(int questionIndex, string question, string answer, string studentAns, int selfGrade, EndGameUI endGameUI)
    {
        studentAnswer = studentAns;
        this.selfGrade = selfGrade;
        this.endGameUI = endGameUI;
        this.question = question;
        this.answer = answer;
        this.questionIndex = questionIndex;
        GetComponent<Image>().color = color;
        transform.GetChild(0).gameObject.GetComponent<Text>().text = (questionIndex + 1).ToString();
    }

    public void setText()
    {
        if (saPanel != null) //teacher use
        {
            saPanel.saBackground.color = getColor(selfGrade);
            saPanel.studentAnswerText.text = studentName + ": " + studentAnswer;
        }
        else if (endGameUI != null) //student use
        {
            endGameUI.questionText.text = question;
            endGameUI.teacherAnswerText.text = answer;
            endGameUI.saBackground.color = getColor(selfGrade);
            endGameUI.studentAnswerText.text = studentAnswer;
        }
    }

    private Color32 getColor(int selfGrade)
    {
        switch (selfGrade)
        {
            case -1:
                return red;
                break;
            case 1:
                return green;
                break;
            default:
                break;
        }
        return color;
    }
}
