using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNode : MonoBehaviour
{
    public TileNode next;
    public TileNode split;
    public GameObject rentedMark;
    //Player moves vertical (up and down): 1
    //Player moves horizontal (left and right): 2
    public int animation;
}
