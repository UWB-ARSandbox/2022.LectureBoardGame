using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class BoardTiles : MonoBehaviour
{
    private BoardGameManager bgm;
    private PlayerGrouping pGroup;

    // Start is called before the first frame update
    void Start()
    {
        pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();
        spawnPlayer();
    }

    public void spawnPlayer()
    {
        // if not the host, spawn in player model
        if (GameLiftManager.GetInstance().m_PeerId != 1)
        {
            // spawn in player
            int randomTile = Random.Range(0, 69);
            Vector3 spawnTile = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").GetChild(randomTile).transform.localPosition;
            PlayerMovement.currentTile = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").GetChild(randomTile).gameObject.GetComponent<TileNode>();

            int playerNumber = 0;
            for (int i = 1; i <= pGroup.m_playerGroups[bgm.getPlayerGroup() - 1].Count; i++)
            {
                if (pGroup.m_playerGroups[bgm.getPlayerGroup() - 1][i - 1] == GameLiftManager.GetInstance().m_PeerId)
                {
                    playerNumber = i;
                }
            }

            ASLHelper.InstantiateASLObject("Player" + playerNumber + "Piece", spawnTile, Quaternion.identity,
                                           bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").GetComponent<ASLObject>().m_Id);
        }
    }
}