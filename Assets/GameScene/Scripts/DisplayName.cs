using UnityEngine;
using System.Collections;

public class DisplayName : MonoBehaviour
{
    // 1 - Create a 3Dtext prefab and add this script to it;
    // 2 - Put the prefab under the object who will have the name displayed

    private TextMesh textToDisplay;

    // Use this for initialization
    void Start()
    {
        textToDisplay = gameObject.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        textToDisplay.text = ((string)transform.parent.name).Replace("(Clone)", "");
    }

    void LateUpdate()
    {
        //Make the text allways face the camera
        transform.rotation = Camera.main.transform.rotation;
    }
}