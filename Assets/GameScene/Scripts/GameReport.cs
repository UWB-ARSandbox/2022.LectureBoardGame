using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;
using ASL;

public class GameReport : MonoBehaviour
{
    private PlayerGrouping playerGrouping;
    private EndGameUI endGameBehavior;
    //public static StarRankingPanel starRanking; //teacher UI only
    public StarRankingPanel groupRanking; //student use
    private static bool isHost = false;
    public static List<TeacherData> reportData;
    public static Dictionary<int,StudentData> studentReportData; //Key=questionIndex
    public static Dictionary<int, StudentStat> studentStats; //Key=id
    public static int qPosted = 0;
    public class StudentStat
    {
        public int numCorrect = 0;
        public int numAnswered = 0;
        public int stars = 0;
    }
    //Store one data for one line in student csv
    public class StudentData
    {
        public string myAnswer;
        public int selfGrade; //Correct=1 or Incorrect=-1
        //For student report
        public string question;
        public string answer;
        //For teacher report
        public int questionIndex;
        public int peerID;
        public string username;
    }
    //Store one data for one line in teacher csv
    public class TeacherData
    {
        public string question;
        public string answer;
        public string numAnswered = "0";
        public string notAnswered = "0";
        public string numCorrect = "0";
        public string numIncorrect = "0";
        //Key=student's peer id, val=student data for this qa
        public Dictionary<int, StudentData> studentAnswers = new Dictionary<int, StudentData>();

        public void updateStats(string numAnswered, string notAnswered, string numCorrect, string numIncorrect)
        {
            this.numAnswered = numAnswered;
            this.notAnswered = notAnswered;
            this.numCorrect = numCorrect;
            this.numIncorrect = numIncorrect;
        }
    }
    void Awake()
    {
        endGameBehavior = GameObject.Find("EndGameUI").GetComponent<EndGameUI>();
    }
    /// <summary>Initialize values</summary>
    void Start()
    {
        playerGrouping = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        GetComponent<ASL.ASLObject>()._LocallySetFloatCallback(MyFloatFunction);
        isHost = GameLiftManager.GetInstance().m_PeerId == BoardGameManager.hostID;
        studentStats = new Dictionary<int, StudentStat>();
        foreach (KeyValuePair<int, string> player in GameLiftManager.GetInstance().m_Players)
        {
            if (player.Key != 1)
                studentStats.Add(player.Key, new StudentStat());
        }
        if (isHost)
        {
            reportData = new List<TeacherData>();
        } else
        {
            studentReportData = new Dictionary<int, StudentData>();
        }

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void downloadReport()
    {
        SimpleFileBrowser.FileBrowser.ShowLoadDialog((filepath) => { downloadReportHelper(filepath[0] + "\\"); }, null, FileBrowser.PickMode.Folders,
            false, null, null, "Select Folder", "Select"); //select folder from file explorer, then call downloadReportHelper
    }

    private void downloadReportHelper(string filepath)
    {
        if (!isHost)
        {
            string username = GameLiftManager.GetInstance().m_Username;
            string filename = username + "_GameReport_" + DateTime.Today.ToString("MM-dd-yy") + ".csv";
            int i = 0;
            while (File.Exists(filepath + filename))
            {
                filename = username + "_GameReport_" + DateTime.Today.ToString("MM-dd-yy") + " (" + (++i) + ").csv";
            }
            studentReport(filepath + filename);
        }
        else
        {
            string filename = "Teacher_GameReport_" + DateTime.Today.ToString("MM-dd-yy") + ".csv";
            int i = 0;
            while (File.Exists(filepath + filename))
            {
                filename = "Teacher_GameReport_" + DateTime.Today.ToString("MM-dd-yy") + " (" + (++i) + ").csv";
            }
            teacherReport(filepath + filename);
        }
    }

    private void studentReport(string filepath)
    {
        int numColumns = 4; //for QA section
        //only add if positive
        int addedCommas = groupRanking.rankingContent.transform.childCount + 1 - numColumns;
        //Star ranking section
        string tableHeader = "Ranking,";
        string line = "Stars,";
        for (int i = 0; i < groupRanking.rankingContent.transform.childCount; i++)
        {
            StarRankingButton player = groupRanking.rankingContent.transform.GetChild(i).GetComponent<StarRankingButton>();
            tableHeader += player.ranking + ": " + player.username + ",";
            line += player.stars + ",";
        }
        if (addedCommas < 0)
        {
            for (int i = 0; i > addedCommas; i--) {
                tableHeader += ",";
                line += ",";
            }
        }
        addRecord(tableHeader, filepath);
        addRecord(line, filepath);
        line = ",,,,";
        if (addedCommas > 0)
        {
            for (int i = 0; i < addedCommas; i++) { line += ","; }
        }
        addRecord(line, filepath);
        //Statistics section
        tableHeader = "Correct,Answered,Questions,";
        addRecord(tableHeader, filepath);
        line = endGameBehavior.stuStats.numCorrect + "," + endGameBehavior.stuStats.numAnswered + "," + endGameBehavior.stuStats.numQuestions + ",";
        if (addedCommas > 0)
        {
            for (int i = 0; i < addedCommas; i++) { line += ","; }
        }
        addRecord(line, filepath);
        line = ",,,,";
        if (addedCommas > 0)
        {
            for (int i = 0; i < addedCommas; i++) { line += ","; }
        }
        addRecord(line, filepath);
        //QA section
        tableHeader = "Question,Answer,MyAnswer,SelfGrade";
        addRecord(tableHeader, filepath);
        foreach (var kvp in studentReportData)
        {
            line = csvFormatString(kvp.Value.question) + "," + csvFormatString(kvp.Value.answer) + "," + 
                csvFormatString(kvp.Value.myAnswer) + "," + (kvp.Value.selfGrade == 1 ? "Correct" : "Incorrect");
            if (addedCommas > 0)
            {
                for (int i = 0; i < addedCommas; i++) { line += ","; }
            }
            addRecord(line, filepath);
        }
        
    }

    private void teacherReport(string filepath)
    {
        int numColumns = 6 + playerGrouping.playerCount;
        string tableHeader = "";
        string line = "";
        tableHeader = "Question,Answer,Answered,NoAnswered,Correct,Incorrect";
        foreach (KeyValuePair<int, int> player in playerGrouping.m_players)
        {
            tableHeader += "," + csvFormatString(GameLiftManager.GetInstance().m_Players[player.Key]);
        }
        addRecord(tableHeader, filepath);
        foreach (TeacherData qaData in reportData)
        {
            line = csvFormatString(qaData.question) + "," + csvFormatString(qaData.answer) + "," +
                qaData.numAnswered + "," + qaData.notAnswered + "," + qaData.numCorrect + "," + qaData.numIncorrect;
            foreach (KeyValuePair<int, int> player in playerGrouping.m_players)
            { //KeyValuePair<int, StudentData> kvp in qaData.studentAnswers
                line += "," + csvFormatString((qaData.studentAnswers[player.Key].selfGrade == 1 ? "Correct: ":"Incorrect: ") + 
                    ": " + qaData.studentAnswers[player.Key].myAnswer);
            }
            addRecord(line, filepath);
        }
    }

    public static void addRecord(string csvLine, string filepath)
    {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath, true))
        {
            file.WriteLine(csvLine);
        }
    }
    //Format the string if needed so it can be added to the csv comma list
    public static string csvFormatString(string arg)
    {
        bool formattingNeeded = false;
        for (int i = 0; i < arg.Length; i++)
        {   //Quotation & comma need special formating for csv
            if (arg[i] == '"' || arg[i] == ',')
            {
                formattingNeeded = true;
                break;
            }
        }
        if (formattingNeeded)
        {
            arg = "\"" + arg;
            //if there a " make it ""
            for (int i = 1; i < arg.Length; i++)
            {
                if (arg[i] == '"')
                {   //Add ", skip next i
                    arg = arg.Insert(i++, "\"");
                }
            }
            arg += "\"";
        }
        return arg;
    }

    //only teacher should call this
    public int createTeacherData(string question, string answer)
    {
        TeacherData newQA = new TeacherData();
        newQA.question = question;
        newQA.answer = answer;
        var players = GameLiftManager.GetInstance().m_Players;
        foreach (var player in players)
        {
            if (player.Key != 1)
            {
                StudentData studentData = new StudentData();
                studentData.peerID = player.Key;
                studentData.username = player.Value;
                //studentData.question = question;
                //studentData.answer = answer;
                studentData.questionIndex = reportData.Count;
                newQA.studentAnswers.Add(player.Key, studentData);
            }
        }
        reportData.Add(newQA);
        return reportData.Count - 1;
    }
    public static void setTeacherQA(string question, string answer, int questionIndex) //existing TeacherData
    {
        reportData[questionIndex].question = question;
        reportData[questionIndex].answer = answer;
    }

    public void updateStats(int questionIndex, int numAnswers, int numCorrect, int numIncorrect) //question stats
    {
        if (questionIndex >= reportData.Count || questionIndex < 0) { return; }
        reportData[questionIndex].updateStats(numAnswers.ToString(), (playerGrouping.playerCount - numAnswers).ToString(),
                numCorrect.ToString(), numIncorrect.ToString());
    }
    //only student should call this
    public void createStudentData(int peerID, string username, string question, string answer, int questionIndex)
    {
        StudentData newQA = new StudentData();
        newQA.question = question;
        newQA.answer = answer;
        newQA.peerID = peerID;
        newQA.username = username;
        newQA.questionIndex = questionIndex;
        studentReportData.Add(questionIndex, newQA);
    }

    private int findQuestionIndex(string q, string a)
    {
        for (int i = 0; i < reportData.Count; i++)
        {
            if (reportData[i].question == q && reportData[i].answer == a)
            {
                return i;
            }
        }
        return -1;
    }

    public static void updateStudentAnswer(int id, int questionIndex, string studentResponse)
    {
        if (isHost)
        {
            if (questionIndex >= reportData.Count || questionIndex < 0) { return; }
            reportData[questionIndex].studentAnswers[id].myAnswer = studentResponse;
            studentStats[id].numAnswered++;
            BoardGameManager.GetInstance().starRanking.updatePlayerStats(id);
        }
        else if (GameLiftManager.GetInstance().m_PeerId == id)
        {
            studentReportData[questionIndex].myAnswer = studentResponse;
            studentStats[id].numAnswered++;
        }
    }
    public static void updateStudentGrade(int id, int questionIndex, int selfGrade)
    {
        if (isHost)
        {
            if (questionIndex >= reportData.Count || questionIndex < 0) { return; }
            reportData[questionIndex].studentAnswers[id].selfGrade = selfGrade;
            if (selfGrade == 1)
            {
                studentStats[id].numCorrect++;
                BoardGameManager.GetInstance().starRanking.updatePlayerStats(id);
            }
        }
        else if (GameLiftManager.GetInstance().m_PeerId == id)
        {
            studentReportData[questionIndex].selfGrade = selfGrade;
            if (selfGrade == 1)
                studentStats[id].numCorrect++;
        }
    }

    public static void MyFloatFunction(string _id, float[] _myFloats)
    {
        string floats = "GradeReport - Floats received: ";
        for (int i = 0; i < _myFloats.Length; i++)
        {
            floats += _myFloats[i].ToString();
            if (_myFloats.Length - 1 != i)
            {
                floats += ", ";
            }
        }
        Debug.Log(floats);
        switch (_myFloats[0])
        {
            case 1: //send student response: 1, id, questionIndex, response
                string response = "";
                //covert int to char to get response
                for (int i = 3; i < _myFloats.Length; i++)
                {
                    response += System.Convert.ToChar((int)_myFloats[i]);
                }
                updateStudentAnswer((int)_myFloats[1], (int)_myFloats[2], response);
                break;
            case 2: //send student self grade: 2, id, questionIndex, correct/incorrect
                updateStudentGrade((int)_myFloats[1], (int)_myFloats[2], (int)_myFloats[3]);
                break;
            case 3: //send player stars: 3, id, stars
                studentStats[(int)_myFloats[1]].stars = (int)_myFloats[2];
                if (isHost)
                    BoardGameManager.GetInstance().starRanking.updatePlayerStar((int)_myFloats[1], (int)_myFloats[2]);
                break;
            default:
                Debug.Log("DownloadableReport Float function Default");
                break;
        }
    }
}
