using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointGenerator {

    private List<Vector2> m_playerList = new List<Vector2>();
    private List<Vector2> m_randomSpawnPoints = new List<Vector2>();

    public List<Vector2> PlayerPositions { get { return m_playerList; } }

    public void AddSpawnPoint(Vector2 a_spawnPoint)
    {
        m_randomSpawnPoints.Add(a_spawnPoint);
    }

    public SpawnPointGenerator()
    {

    }

    public SpawnPointGenerator(GridSystem a_gridSystem)
    {
        for (int i = 0; i < 4; i++)
        {
            if(!SpawnPlayer(a_gridSystem))
                return;
        }
    }

    public bool SpawnPlayer(GridSystem gridSystem)
    {
        if (m_playerList.Count < 1 || m_playerList.Count > 2)
        {
            Vector2 pos;
            //bool checkPlayers = false;
            m_randomSpawnPoints = GetEmptyTiles(gridSystem);
            int index = UnityEngine.Random.Range(0, m_randomSpawnPoints.Count - 1);
            Vector2 newPosition = m_randomSpawnPoints[index];

            pos = newPosition;

            GameObject tempObject = GameObject.Instantiate(Resources.Load<GameObject>("PlayerQuad"));
            tempObject.transform.position = pos;

            m_randomSpawnPoints.RemoveAt(index);
            m_playerList.Add(pos);
            Debug.Log(m_playerList[m_playerList.Count - 1]);

            return true;
        }
        else if (m_playerList.Count == 1)
        {
            int index = 0;
            Vector2 player = m_playerList[0];
            float maxDist = -1;
            Vector2 best = new Vector2(0, 0);

            for (int i = 0; i < m_randomSpawnPoints.Count; i++)
            {
                Vector2 rel = m_randomSpawnPoints[i] - player;
                float dist = rel.sqrMagnitude;
                if (dist > maxDist)
                {
                    maxDist = dist;
                    best = m_randomSpawnPoints[i];
                    index = i;
                }
            }
            GameObject tempObject = GameObject.Instantiate(Resources.Load<GameObject>("PlayerQuad"));
            tempObject.transform.position = best;
            m_playerList.Add(best);
            m_randomSpawnPoints.RemoveAt(index);
            return true;
        }
        else if (m_playerList.Count >= 1 && m_playerList.Count < 4)
        {
            int index = 0;
            Vector2 player = new Vector2(0, 0);

            for (int i = 0; i < m_randomSpawnPoints.Count; i++)
                player += m_randomSpawnPoints[i];

            player /= m_randomSpawnPoints.Count;

            float maxDist = -1;
            Vector2 best = new Vector2(0, 0);

            for (int i = 0; i < m_randomSpawnPoints.Count; i++)
            {
                Vector2 rel = m_randomSpawnPoints[i] - player;
                float dist = rel.sqrMagnitude;
                if (dist > maxDist)
                {
                    maxDist = dist;
                    best = m_randomSpawnPoints[i];
                    index = i;
                }
            }
            GameObject tempObject = GameObject.Instantiate(Resources.Load<GameObject>("PlayerQuad"));
            tempObject.transform.position = best;
            m_playerList.Add(best);
            m_randomSpawnPoints.RemoveAt(index);
            return true;
        }
        else if (m_playerList.Count >= 1 && m_playerList.Count < 4)
        {
            Vector2 pos = GetFurthestPoint(gridSystem);

            //GameObject tempObject = GameObject.Instantiate(Resources.Load<GameObject>("PlayerQuad"));
            //tempObject.transform.position = newPlayer.PlayerPosition;
            GameObject tempObject = GameObject.Instantiate(Resources.Load<GameObject>("PlayerQuad"));
            tempObject.transform.position = pos;

            m_playerList.Add(pos);
            Debug.Log(m_playerList[m_playerList.Count - 1]);

            return true;
        }
        else
            return false;

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
        int index = 0;

        for (int i = 0; i < m_randomSpawnPoints.Count; i++)
        {
            if (difference.x + difference.y < Mathf.Abs(generalDistance.x - m_randomSpawnPoints[i].x) + Mathf.Abs(generalDistance.y - m_randomSpawnPoints[i].y))
            {
                difference = new Vector2(Mathf.Abs(generalDistance.x - m_randomSpawnPoints[i].x), Mathf.Abs(generalDistance.y - m_randomSpawnPoints[i].y));
                furthestPoint = m_randomSpawnPoints[i];
                index = i;
            }
        }
        m_randomSpawnPoints.RemoveAt(index);
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

    private List<Vector2> GetUnusedPoints()
    {
        List<Vector2> v = new List<Vector2>();
        for (int i = 0; i < m_randomSpawnPoints.Count; i++)
        {
            Vector2 p = m_randomSpawnPoints[i];
            for (int j = 0; j < m_playerList.Count; j++)
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
