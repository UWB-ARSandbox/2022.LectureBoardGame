using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ASL;

public class GameReport : MonoBehaviour
{
    private PlayerGrouping playerGrouping;
    private EndGameUI endGameBehavior;
    private static bool isHost = false;
    private string filepath;
    private string filename;
    public static List<TeacherData> reportData;
    public static Dictionary<int,StudentData> studentReportData; //Key=questionIndex


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
        public string numAnswered;
        public string notAnswered;
        public string numCorrect;
        public string numIncorrect;
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
        isHost = GameLiftManager.GetInstance().m_PeerId == 1;
        filepath = "C:\\Users\\kuoch\\Downloads\\"; //temp
        if (isHost)
        {
            filename = "Teacher_GameReport_" + DateTime.Today.ToString("MM-dd-yy") + ".csv";
            reportData = new List<TeacherData>();
        } else
        {
            filename = GameLiftManager.GetInstance().m_Username + "_GameReport_"+ DateTime.Today.ToString("MM-dd-yy") + ".csv";
            studentReportData = new Dictionary<int, StudentData>();
        }

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void downloadReport()
    {
        if (!isHost)
        {
            studentReport();
        }
        else
        {
            teacherReport();
        }
    }

    public void studentReport()
    {
        int numColumns = 4;
        //Include statistics? Correct Questions Percentage
        string tableHeader = "Correct,Answered,Questions,";
        addRecord(tableHeader, filepath + filename);
        string line = endGameBehavior.stuStats.numCorrect + "," + endGameBehavior.stuStats.numAnswered + "," + endGameBehavior.stuStats.numQuestions + ",";
        addRecord(line, filepath + filename);
        addRecord(",,,,", filepath + filename);
        tableHeader = "Question,Answer,MyAnswer,SelfGrade";
        addRecord(tableHeader, filepath + filename);
        foreach (var kvp in studentReportData)
        {
            line = csvFormatString(kvp.Value.question) + "," + csvFormatString(kvp.Value.answer) + "," + 
                csvFormatString(kvp.Value.myAnswer) + "," + (kvp.Value.selfGrade == 1 ? "Correct" : "Incorrect");
            addRecord(line, filepath + filename);
        }
    }

    public void teacherReport()
    {
        int numColumns = 6 + playerGrouping.playerCount;
        string tableHeader = "Question,Answer,Answered,NoAnswered,Correct,Incorrect";
        foreach (KeyValuePair<int, int> player in playerGrouping.m_players)
        {
            tableHeader += "," + csvFormatString(GameLiftManager.GetInstance().m_Players[player.Key]);
        }
        addRecord(tableHeader, filepath + filename);
        foreach (TeacherData qaData in reportData)
        {
            string line = csvFormatString(qaData.question) + "," + csvFormatString(qaData.answer) + "," +
                qaData.numAnswered + "," + qaData.notAnswered + "," + qaData.numCorrect + "," + qaData.numIncorrect;
            foreach (KeyValuePair<int, int> player in playerGrouping.m_players)
            { //KeyValuePair<int, StudentData> kvp in qaData.studentAnswers
                line += "," + csvFormatString((qaData.studentAnswers[player.Key].selfGrade == 1 ? "Correct: ":"Incorrect: ") + 
                    ": " + qaData.studentAnswers[player.Key].myAnswer);
            }
            addRecord(line, filepath + filename);
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

    public void updateStats(int questionIndex, int numAnswers, int numCorrect, int numIncorrect)
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
        if (questionIndex >= reportData.Count || questionIndex < 0) { return; }
        if (isHost)
        {
            reportData[questionIndex].studentAnswers[id].myAnswer = studentResponse;
        }
        else
        {
            studentReportData[questionIndex].myAnswer = studentResponse;
        }
    }
    public static void updateStudentGrade(int id, int questionIndex, int selfGrade)
    {
        if (questionIndex >= reportData.Count || questionIndex < 0) { return; }
        if (isHost)
        {
            reportData[questionIndex].studentAnswers[id].selfGrade = selfGrade;
        }
        else
        {
            studentReportData[questionIndex].selfGrade = selfGrade;
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
                if (isHost)
                {
                    updateStudentAnswer((int)_myFloats[1], (int)_myFloats[2], response);
                } else
                {
                    studentReportData[(int)_myFloats[2]].myAnswer = response;
                }
                break;
            case 2: //send student self grade: 2, id, questionIndex, correct/incorrect
                if (isHost)
                {
                    updateStudentGrade((int)_myFloats[1], (int)_myFloats[2], (int)_myFloats[3]);
                } else
                {
                    studentReportData[(int)_myFloats[2]].selfGrade = (int)_myFloats[3];
                }
                break;
            default:
                Debug.Log("DownloadableReport Float function Default");
                break;
        }
    }
}
