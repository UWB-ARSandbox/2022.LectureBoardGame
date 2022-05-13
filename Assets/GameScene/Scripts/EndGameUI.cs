using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class EndGameUI : MonoBehaviour
{
    public Text questionText;
    public Text teacherAnswerText;
    public Text studentAnswerText;
    public Image saBackground; //student answer
    public Text studentStats;
    public GameObject playerAnswersPanel;

    private Color32 red;
    private Color32 green;

    public GameObject listContent;
    public GameObject studentListButton; //used by student, show QA A
    public GameObject teacherListButton; //used by teacher, show QA and player list
    //public GameObject playerListButton; //used by teacher, show student's A
    public StarRankingPanel starRankPanel_s;
    public GameObject starRankButton;

    private GameReport gameReport;
    public StudentStats stuStats; //student only
    private PlayerGrouping playerGrouping;
    public static bool ended = false;

    // Start is called before the first frame update
    void Start()
    {
        gameReport = GameObject.Find("GameReport").GetComponent<GameReport>();
        playerGrouping = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        red = new Color32(255, 138, 146, 255);
        green = new Color32(198, 255, 138, 255);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void endGameSetUp()
    {
        if (!ended)
        {
            ended = true;
            if (GameLiftManager.GetInstance().m_PeerId != 1)
            {
                Debug.Log("id: " + GameLiftManager.GetInstance().m_PeerId);
                Debug.Log("endgame setup player group" + BoardGameManager.GetInstance().getPlayerGroup());
                stuStats = BoardGameManager.GetInstance().getGroupWorld(BoardGameManager.GetInstance().getPlayerGroup()).GetComponent<StudentStats>();
                studentStats.text = "Stats: " + stuStats.numCorrect + "/" + stuStats.numAnswered + "/" + stuStats.numQuestions;
                starRankPanel_s.gameObject.SetActive(true);
                starRankPanel_s.groupRankSetUp(BoardGameManager.GetInstance().getPlayerGroup());
                starRankPanel_s.gameObject.SetActive(false);
                loadStudentButtons();
            } else
            {
                studentStats.gameObject.SetActive(false);
                starRankButton.SetActive(false);
                loadTeacherButtons();
            }
        }
    }

    public void loadStudentButtons()
    {
        Debug.Log("Student End Game");
        foreach (KeyValuePair<int, GameReport.StudentData> kvp in GameReport.studentReportData)
        {
            GameObject newQ = GameObject.Instantiate(studentListButton);
            newQ.GetComponent<StudentAnswerButton>().setup(kvp.Key + 1,kvp.Value.question, kvp.Value.answer, kvp.Value.myAnswer, 
                kvp.Value.selfGrade, this);
            newQ.transform.parent = listContent.transform;
            newQ.SetActive(true);
        }
    }

    public void loadTeacherButtons()
    {
        Debug.Log("Teacher End Game");
        int index = 0;
        foreach (GameReport.TeacherData data in GameReport.reportData)
        {
            GameObject newQ = GameObject.Instantiate(teacherListButton);
            TeacherButton tb = newQ.GetComponent<TeacherButton>();
            tb.setUp(false, index++, true);
            tb.setQA(data.question, data.answer);
            //tb.studentAnswersPanel = playerAnswersPanel;
            tb.published = true;
            newQ.transform.GetChild(1).gameObject.GetComponent<Text>().text = data.numCorrect + "/" + data.numAnswered + "/" + GameReport.reportData.Count;
            newQ.transform.parent = listContent.transform;
            newQ.SetActive(true);
        }
    }

    //public void teacherButtonBehavior()
    //{
    //    playerAnswersPanel.SetActive(true);
    //    playerAnswersPanel.GetComponent<StudentAnswersPanel>().loadPanel_noLabel(questionIndex);
    //}

}
