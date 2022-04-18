using System.IO;
using UnityEngine;
using Unity;
using UnityEditor;
using System;
using ASL;

public class ManageCSV : MonoBehaviour
{
    /// <summary>
    /// The csv file can be dragged throughthe inspector.
    /// </summary>
    public static string csvFile;
    public TextAsset csvFile2;

    /// <summary>
    /// The grid in which the CSV File would be parsed.
    /// </summary>
    public static string[,] grid;

    static int xLength = 0;
    static int yLength = 0;

    private bool isHost = false;

    private ASLObject aSLObject;

    void Start()
    {
        /// create an invisible ASL Object like on the demo with the ManageCSV script attached
        /// this object should only be generated for the host, not the players. The players will
        /// have another object (script) that will call the host's ASL object's static setCSVCounts()
        /// method in order to send their data
        /// (maybe they will need to have a universally shared object with a script with a fucntion
        /// that first checks if the CSVManager ASL Object exists in the scene, if it does exist,
        /// it will signify that it is the host's scene. Once host's scnene is verified, call the
        /// static setCSVCounts() method through the shared object. Perhaps I don't even need to
        /// "set and send" because that method is for proper synchronization of object statuses
        /// between the multiple users applications, but in this case, we don't need to synchronize,
        /// only calling the static method)

        aSLObject = gameObject.GetComponent<ASLObject>();

        isHost = GameLiftManager.GetInstance().AmHighestPeer();

        //gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(setCSVCounts);

        // grid = getCSVGrid(csvFile.text);
        if (isHost)
        {
            Debug.Log("Host");
            //grid = getCSVGrid(csvFile.text);
        }
        //grid = getCSVGrid(csvFile2.text);

        /*// simluating teacher's data being updated by the students' sent data
        // sendVal(question#, correct/incorrect)
        sendVal(1, true);
        sendVal(1, false);
        sendVal(2, false);
        sendVal(2, true);

        // simulating teacher adding new questions
        addCSVRow("NewQuestion1", "NewAnswer1");

        updateCSV();*/
    }

    /// <summary>
    /// only for debugging purposes, prints out the grid in order from top left to bottom right
    /// </summary>
    private void printGrid()
    {
        for (int i = 1; i < yLength; i++)
        {
            for (int j = 0; j < xLength; j++)
            {
                Debug.Log("grid[" + j + ", " + i + "] = " + grid[j, i]);
            }
        }
    }



    private static void createCSV()
    {
        // make new CSV file with proper formatting but with no values yet
    }

    /// <summary>
    /// splits a CSV file into a 2D string array
    /// </summary>
    /// <returns> 2 day array of the csv file.</returns>
    /// <param name="csvText">the CSV data as string</param>
    public static string[,] getCSVGrid(string csvText)
    {
        //split the data on split line character
        string[] lines = csvText.Split("\n"[0]);

        // find the max number of columns
        int totalColumns = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');
            totalColumns = Mathf.Max(totalColumns, row.Length);
        }
        xLength = totalColumns;

        // creates new 2D string grid to output to
        string[,] outputGrid = new string[totalColumns + 1, lines.Length + 1];
        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = lines[y].Split(',');
            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];
                yLength = Mathf.Max(yLength, y);
            }
        }

        return outputGrid;
    }

    public void setCSVCounts(string _id, float[] _f)
    {
        Debug.Log("setting Values");
        grid[(int)_f[0], (int)_f[1]] = (Int32.Parse(grid[(int)_f[0], (int)_f[1]]) + 1).ToString();
        updateCSV();
    }

    public void sendingCorrect(int qNum)
    {
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[2];
            sendValue[0] = 4f;
            sendValue[1] = qNum;
            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }

    public void sendingIncorrect(int qNum)
    {
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[2];
            sendValue[0] = 5f;
            sendValue[1] = qNum;
            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }

    public void addCSVRow(string question, string answer)
    {
        grid[0, yLength] = question;
        grid[1, yLength] = answer;
        grid[4, yLength] = "0";
        grid[5, yLength] = "0";
        yLength++;
    }

    /*public void sendVal(int qNum, bool correct)
    {
        if (correct)
        {
            grid[4, qNum] = (Int32.Parse(grid[4, qNum]) + 1).ToString();
        } else
        {
            grid[5, qNum] = (Int32.Parse(grid[5, qNum]) + 1).ToString();
        }
    }*/

    public void updateCSV()
    {
        string newData = "";
        for (int i = 0; i < yLength; i++)
        {
            newData += grid[0, i];
            for (int j = 1; j < xLength; j++)
            {
                newData += "," + grid[j, i];
            }
            newData += "\n";
        }

        string csvLocation = Application.dataPath;
        Console.WriteLine(csvLocation);
        File.WriteAllText("Assets/ASL/Resources/TestDataCopy.csv", newData);
    }
}
