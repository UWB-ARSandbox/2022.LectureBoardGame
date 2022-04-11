using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroll : MonoBehaviour
{
    public Text txt;
    public GameObject button;
    public ManageCSV qa;
    private int number = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            string q = qa.grid[0,number];
            string a = qa.grid[1,number];
            if (q!=""){
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
                number++;
            }

        }

        
    }
}
