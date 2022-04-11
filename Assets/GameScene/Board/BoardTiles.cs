using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class BoardTiles : MonoBehaviour
{
    private static Dictionary<float, List<GameObject>> map;

    // Start is called before the first frame update
    void Start()
    {
        map = new Dictionary<float, List<GameObject>>();

        // set up board from the top left map[row.col]
        // top left corner is map[1.01]; bottom right corner is map[13.13]
        map[1.01f] = new List<GameObject>();
        map[1.02f] = new List<GameObject>();
        map[1.03f] = new List<GameObject>();
        map[1.04f] = new List<GameObject>();
        map[1.05f] = new List<GameObject>();
        map[1.06f] = new List<GameObject>();
        map[1.07f] = new List<GameObject>();
        map[1.08f] = new List<GameObject>();
        map[1.09f] = new List<GameObject>();
        map[1.10f] = new List<GameObject>();
        map[1.11f] = new List<GameObject>();
        map[1.12f] = new List<GameObject>();
        map[1.13f] = new List<GameObject>();

        map[2.01f] = new List<GameObject>();
        map[2.13f] = new List<GameObject>();

        map[3.01f] = new List<GameObject>();
        map[3.13f] = new List<GameObject>();

        map[4.01f] = new List<GameObject>();
        /*map[4.02f] = new List<GameObject>();
        map[4.03f] = new List<GameObject>();
        map[4.04f] = new List<GameObject>();
        map[4.10f] = new List<GameObject>();
        map[4.11f] = new List<GameObject>();
        map[4.12f] = new List<GameObject>();*/
        map[4.13f] = new List<GameObject>();

        map[5.01f] = new List<GameObject>();
        map[5.04f] = new List<GameObject>();
        map[5.10f] = new List<GameObject>();
        map[5.13f] = new List<GameObject>();

        map[6.01f] = new List<GameObject>();
        map[6.04f] = new List<GameObject>();
        map[6.10f] = new List<GameObject>();
        map[6.13f] = new List<GameObject>();


        map[7.01f] = new List<GameObject>();
        map[7.04f] = new List<GameObject>();
        map[7.10f] = new List<GameObject>();
        map[7.13f] = new List<GameObject>();

        map[8.01f] = new List<GameObject>();
        map[8.04f] = new List<GameObject>();
        map[8.10f] = new List<GameObject>();
        map[8.13f] = new List<GameObject>();

        map[9.01f] = new List<GameObject>();
        map[9.04f] = new List<GameObject>();
        map[9.10f] = new List<GameObject>();
        map[9.13f] = new List<GameObject>();

        map[10.01f] = new List<GameObject>();
        /*map[10.02f] = new List<GameObject>();
        map[10.03f] = new List<GameObject>();
        map[10.04f] = new List<GameObject>();
        map[10.10f] = new List<GameObject>();
        map[10.11f] = new List<GameObject>();
        map[10.12f] = new List<GameObject>();*/
        map[10.13f] = new List<GameObject>();

        map[11.01f] = new List<GameObject>();
        map[11.13f] = new List<GameObject>();

        map[12.01f] = new List<GameObject>();
        map[12.13f] = new List<GameObject>();

        map[13.01f] = new List<GameObject>();
        map[13.02f] = new List<GameObject>();
        map[13.03f] = new List<GameObject>();
        map[13.04f] = new List<GameObject>();
        map[13.05f] = new List<GameObject>();
        map[13.06f] = new List<GameObject>();
        map[13.07f] = new List<GameObject>();
        map[13.08f] = new List<GameObject>();
        map[13.09f] = new List<GameObject>();
        map[13.10f] = new List<GameObject>();
        map[13.11f] = new List<GameObject>();
        map[13.12f] = new List<GameObject>();
        map[13.13f] = new List<GameObject>();

        spawnPlayer();
    }

    public void spawnPlayer()
    {
        // if not the host, spawn in player model
        if (true/*!GameLiftManager.GetInstance().AmHighestPeer()*/)
        {
            float row = 0;
            float col = 0;

            // get valid space on the board
            while (!validSpace(row + col))
            {
                row = Random.Range(1, 14);
                col = (Random.Range(1, 14)) / 100.0f;
            }

            // spawn in player
            Vector3 pos = getCoordinate(row + col);
            ASLHelper.InstantiateASLObject("Player" + GameLiftManager.GetInstance().m_PeerId + "Piece", pos, Quaternion.identity);
            // map[row + col] = "CREATED PLAYER PIECE";
        } else
        {
            Debug.Log("HOST");
        }
    }

    public static bool validSpace(float space)
    {
        return (map.ContainsKey(space));
    }

    public static bool validSpace(Vector3 space)
    {
        if (space.z > 4.26f || space.z < -4.5f || space.x > 4.5f || space.x < -4.26f)
        {
            return false;
        }
        float row = 2 + (-1 * Mathf.Round(((space.z - 4.26f) / 0.73f) + 1.0f));
        float col = ((space.x + 4.26f) / 73.0f) + 0.01f;
        return validSpace(row + col);
    }

    public static Vector3 getCoordinate(float space)
    {
        float posX = -4.26f + (( (Mathf.Round(100 * (space - (int)space)) / 100.0f) - 0.01f) * 73.0f);
        float posY = 0;
        float posZ = 4.26f - (((int)space - 1) * 0.73f);
        return new Vector3(posX, posY, posZ);
    }

    public static void playerMove(float prevSpace, string direction, GameObject player)
    {
        if (direction.Equals("up"))
        {
            if (validSpace(prevSpace - 1))
            {
                Debug.Log("Moved up");
                //map[prevSpace].Remove(player);
                //map[prevSpace - 1].Add(player);
            }
        }
        else if (direction.Equals("down"))
        {
            if (validSpace(prevSpace + 1))
            {
                Debug.Log("Moved down");
                //map[prevSpace].Remove(player);
                //map[prevSpace + 1].Add(player);
            }
        }
        else if (direction.Equals("right"))
        {
            if (validSpace(prevSpace + 0.01f))
            {
                Debug.Log("Moved right");
                //map[prevSpace].Remove(player);
                //map[prevSpace + 0.01f].Add(player);
            }
        }
        else if (direction.Equals("left"))
        {
            if (validSpace(prevSpace - 0.01f))
            {
                Debug.Log("Moved left");
                //map[prevSpace].Remove(player);
                //map[prevSpace - 0.01f].Add(player);
            }
        }
    }
}