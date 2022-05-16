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
    public GameObject studentAnswersPanel;
    private Text q2;
    private Text stats;
    //Int for the stat text
    //Format: Correct/Answered/Students
    int Correct;
    int Answered;
    int Students;
    //For GameReport's reportData list
    public int questionIndex = -1;
    private GameReport gameReport;
    public bool byQuestionNum = false; //True: Button text is question number; False: Use Question

    // Start is called before the first frame update
    void Start()
    {
        gameReport = GameObject.Find("GameReport").GetComponent<GameReport>();
        if (studentAnswersPanel == null)
        {
            studentAnswersPanel = GameObject.Find("Canvas").transform.Find("StudentAnswersPanel").gameObject;
        }
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OpenPanel);
        q2 = gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        stats = gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setUp(bool inGame, int qIndex, bool byQuestionNum) //qIndex only set if not ingame
    {
        this.byQuestionNum = byQuestionNum;
        if (inGame)
        {
            if (editPanel==null){
                editPanel = GameObject.Find("Canvas").transform.Find("AddQ").gameObject;
            }
        } else
        {
            questionIndex = qIndex;
        }
    }

    public void setQA(string q, string a)
    {
        question = q;
        answer = a;
        if (q2==null){
            q2 = gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        }
        if (byQuestionNum)
        {
            q2.text = "Q" + (questionIndex + 1).ToString();
        }
        else {
            q2.text = question;
        }
    }

    void OpenPanel(){
        if (published)
        {
            studentAnswersPanel.SetActive(true);
            studentAnswersPanel.GetComponent<StudentAnswersPanel>().loadPanel(questionIndex);
        } else
        {
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
                //checkPublished();
                editPanel.SetActive(true);
            }
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

    public void updateGameReportStats(int numAnswers, int numCorrect, int numIncorrect)
    {
        gameReport.updateStats(questionIndex, numAnswers, numCorrect, numIncorrect);
    }
}