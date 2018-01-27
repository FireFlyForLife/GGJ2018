﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointGenerator {

    private List<Vector2> m_playerList = new List<Vector2>();
    private List<Vector2> m_randomSpawnPoints = new List<Vector2>();

    public List<Vector2> SpawnPointList { get { return m_playerList; } }


    public SpawnPointGenerator(GridSystem a_gridSystem)
    {
        m_randomSpawnPoints = GetEmptyTiles(a_gridSystem);
        for(int i = 0; i < 4; i++)
            if (!SpawnPlayer(a_gridSystem))
                return;

        Vector2 item = GetAndOccupyPosRange();
        a_gridSystem.SetOccupied((int)item.x,(int)item.y,TileType.item);
    }

    public void Init(GridSystem a_gridSystem)
    {
        m_randomSpawnPoints = GetEmptyTiles(a_gridSystem);
    }

    public bool SpawnPlayer(GridSystem gridSystem)
    {


        if (m_playerList.Count < 4)
        {
            Vector2 newPlayer;
            //bool checkPlayers = false;
            newPlayer = GetAndOccupyPosRange();

 // newPlayer.PlayerIndex = m_playerList.Count;

            //GameObject tempObject = GameObject.Instantiate(Resources.Load<GameObject>("PlayerQuad"));
            //tempObject.transform.position = newPlayer;

            m_playerList.Add(newPlayer);
            Debug.Log(m_randomSpawnPoints.Count);

            return true;
        }
        //else if (m_playerList.Count == 1)
        //{
        //    Vector2 player = m_playerList[0].PlayerPosition;
        //    List<Vector2> vec = GetUnusedPoints(GetEmptyTiles(gridSystem));
        //    float maxDist = -1;
        //    Vector2 best = new Vector2(0, 0);

        //    for (int i = 0; i < vec.Count; i++)
        //    {
        //        Vector2 rel = new Vector2(player.x - vec[i].x, player.y - vec[i].y);
        //        float dist = Mathf.Abs(rel.x + rel.y);
        //        if (dist > maxDist)
        //        {
        //            maxDist = dist;
        //            best = rel;
        //        }
        //    }
        //    Player newPlayer = new Player();
        //    newPlayer.PlayerPosition = best;
        //    newPlayer.PlayerIndex = m_playerList.Count;
        //    m_playerList.Add(newPlayer);
        //    return true;
        //}
        //else if (m_playerList.Count >= 1 && m_playerList.Count < 4)
        //{
        //    Player newPlayer = new Player();

        //    Vector2 newPosition = GetFurthestPoint(gridSystem);

        //    newPlayer.PlayerPosition = newPosition;
        //    newPlayer.PlayerIndex = m_playerList.Count;

        //    GameObject tempObject = GameObject.Instantiate(Resources.Load<GameObject>("PlayerQuad"));
        //    tempObject.transform.position = newPlayer.PlayerPosition;


        //    m_playerList.Add(newPlayer);
        //    //Debug.Log(m_playerList[m_playerList.Count - 1].PlayerPosition);

        //    return true;
        //}
        else
            return false;
        
    }

    public Vector2 GetAndOccupyPosRange()
    {
        int randomNumber = UnityEngine.Random.Range(0, m_randomSpawnPoints.Count - 1);
        Vector2 newPosition = m_randomSpawnPoints[randomNumber];

        m_randomSpawnPoints.RemoveAt(randomNumber);

        for (int i = m_randomSpawnPoints.Count-1; i >= 0; i--)
        {
            /*  float a = m_randomSpawnPoints[i].y - newPosition.y;
              float b = Math.Abs(m_randomSpawnPoints[i].x - newPosition.x);*/

            if ((m_randomSpawnPoints[i] - newPosition).sqrMagnitude <= 300)//Math.Abs(m_randomSpawnPoints[i].x - newPosition.x) <= 10 || Math.Abs(m_randomSpawnPoints[i].y - newPosition.y) <=   10)
            {
                m_randomSpawnPoints.RemoveAt(i);
            }
        }

        return newPosition;
    }

    private Vector2 GetFurthestPoint(GridSystem gridSystem)
    {
        Vector2 generalDistance = new Vector2();

        for (int i = 0; i < m_playerList.Count; i++)
        {
            generalDistance += m_playerList[i];
        }

        generalDistance = generalDistance / m_playerList.Count;

        Vector2 furthestPoint = new Vector2();
        Vector2 difference = new Vector2(-999, -999);

        List<Vector2> positions = GetUnusedPoints(GetEmptyTiles(gridSystem));

        for (int i = 0; i < positions.Count; i++)
        {
            if(difference.x + difference.y < Mathf.Abs(generalDistance.x - positions[i].x) + Mathf.Abs(generalDistance.y - positions[i].y))
            {
                difference = new Vector2(Mathf.Abs(generalDistance.x - positions[i].x), Mathf.Abs(generalDistance.y - positions[i].y));
                furthestPoint = positions[i];
            }
        }

        Debug.Log(furthestPoint);
        return furthestPoint;
    }

    private List<Vector2> GetEmptyTiles(GridSystem gridSystem)
    {
        List<Vector2> v = new List<Vector2>();
        for (int y = 0; y < gridSystem.GridSize.Y; y++)
            for (int x = 0; x < gridSystem.GridSize.Y; x++)
                if (gridSystem.GetTile(x, y) == TileType.floor)
                    v.Add(new Vector2(x, y));
        return v;
    }

    private List<Vector2> GetUnusedPoints(List<Vector2> m_randomSpawnPoints)
    {
        List<Vector2> v = new List<Vector2>();
        for(int i = 0; i < m_randomSpawnPoints.Count; i++)
        {
            Vector2 p = m_randomSpawnPoints[i];
            for(int j = 0; j < m_playerList.Count; j++)
            {
                Vector2 p2 = m_playerList[j];
                if (p.x == p2.x && p.y == p2.y)
                {
                    m_randomSpawnPoints.RemoveAt(i);
                    //Debug.Log("SKIPPED");
                    continue;
                }
                //else v.Add(p);
                //Debug.Log("ADDED " + p.x + "  " + p.y);
            }
        }
        return m_randomSpawnPoints;
    }

    public bool SpawnEndPoint(Vector2 a_endPosition)
    {
        return true;
    }

    public bool SpawnObjects(/*Object , */ Vector2 a_objectPosition)
    {
        return true;
    }
    
}
