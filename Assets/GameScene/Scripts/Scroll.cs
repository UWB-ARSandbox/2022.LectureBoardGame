using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class Scroll : MonoBehaviour
{
    //public Text txt;
    public static GameObject button;
    public ManageCSV qa;
    private static int number = 1;
    ASLpanel m_ASLObject;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        m_ASLObject = gameObject.GetComponent<ASLpanel>();
        button = GameObject.Find("Button");
        yield return new WaitForSeconds(1);
        /*try{
        if(qa!=null){
            string q = ManageCSV.grid[0,number];
            string a = ManageCSV.grid[1,number];
            while(q!=""){
                createButton(q, a);
                q = ManageCSV.grid[0,number];
                a = ManageCSV.grid[1,number];
            } 
        }
        } catch (NullReferenceException err){}*/
    }

    public static void buttonSetup()
    {
        string q = ManageCSV.grid[0, number];
        string a = ManageCSV.grid[1, number];
        while (q != "")
        {
            createButton(q, a);
            q = ManageCSV.grid[0, number];
            a = ManageCSV.grid[1, number];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            string q = ManageCSV.grid[0, number];
            string a = ManageCSV.grid[1, number];
            if (q != "")
            {
                /*
                GameObject go = GameObject.Find("Content");
                //Get pos of last button and size of button
                int i = go.transform.childCount-1;
                var rectTransform = go.transform.GetChild(i).GetComponent<RectTransform>();
                Vector3 pos = rectTransform.position;
                Vector2 size = rectTransform.sizeDelta;
                //Change size of "content"
                rectTransform = go.GetComponent<RectTransform>();
                rectTransform.sizeDelta += new Vector2(0, size.y);
                pos -= new Vector3(0,size.y, 0); 
                //create new button
                GameObject newButton = Instantiate(button) as GameObject;
                newButton.transform.SetParent(rectTransform.transform, false);
                newButton.GetComponent<RectTransform>().position = pos;
                newButton.GetComponent<ButtonBehavior>().setQA(q, a);
                number++;*/
                createButton(q, a);
            }

        }
    }

    static void createButton(string q, string a)
    {
        GameObject go = GameObject.Find("Content");
        //Get pos of last button and size of button
        int i = go.transform.childCount - 1;
        var rectTransform = go.transform.GetChild(i).GetComponent<RectTransform>();
        Vector3 pos = rectTransform.position;
        Vector2 size = rectTransform.sizeDelta;
        //Change size of "content"
        rectTransform = go.GetComponent<RectTransform>();
        rectTransform.sizeDelta += new Vector2(0, size.y);
        //m_ASLObject.increaseScale(new Vector2(0,size.y));
        pos -= new Vector3(0, size.y, 0);
        //size += new Vector2(0,size.y);
        //m_ASLObject.SendAndSetLocalScale(new Vector3(size.x, size.y,0));
        //create new button
        GameObject newButton = Instantiate(button) as GameObject;
        newButton.transform.SetParent(rectTransform.transform, false);
        newButton.GetComponent<RectTransform>().position = pos;
        newButton.GetComponent<ButtonBehavior>().setQA(q, a);
        number++;
    }
}
