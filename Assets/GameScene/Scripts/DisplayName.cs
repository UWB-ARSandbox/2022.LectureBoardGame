using UnityEngine;
using System.Collections;
using ASL;

public class DisplayName : MonoBehaviour
{
    // 1 - Create a 3Dtext prefab and add this script to it;
    // 2 - Put the prefab under the object who will have the name displayed

    private TextMesh textToDisplay;
    private PlayerGrouping pGroup;
    private BoardGameManager bgm;

    // Use this for initialization
    void Start()
    {
        textToDisplay = gameObject.GetComponent<TextMesh>();
        pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();
    }

    void Update()
    {
        textToDisplay.text = ((string)transform.parent.name).Replace("Piece(Clone)", "");
    }

    void LateUpdate()
    {
        //Make the text allways face the camera
        transform.rotation = Camera.main.transform.rotation;
    }
}