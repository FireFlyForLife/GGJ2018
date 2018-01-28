using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointGenerator
{

    private List<Vector2> m_playerList = new List<Vector2>();
    private List<Vector2> m_randomSpawnPoints = new List<Vector2>();

    public List<Vector2> SpawnPointList { get { return m_playerList; } }


    public SpawnPointGenerator(GridSystem gSystem, World2D world)
    {
        m_randomSpawnPoints = GetEmptyTiles(gSystem);

        Vector2 pos = GetWithoutOccupyRange(gSystem);
        GameObject o = new GameObject();
        o.AddComponent<RaycastEntity>();
        RaycastEntity entity = o.GetComponent<RaycastEntity>();
        entity.Position = pos;
        entity.TextureId = 5;
        world.Entities.Add(entity);

        GameObject o2 = new GameObject();
        o2.AddComponent<RaycastEntity>();
        Vector2 pos2 = GetAndOccupyPosRange(gSystem);
        RaycastEntity endPoint = o2.GetComponent<RaycastEntity>();
        endPoint.Position = pos2;
        endPoint.TextureId = 6;
        endPoint.tag = "EndPoint";
        world.Entities.Add(endPoint);

        //gSystem.SetOccupied((int)pos.x, (int)pos.y, TileType.item);


        for (int i = 0; i < 8; i++)
            if (!SpawnPlayer(gSystem))
                return;
    }

    public bool SpawnPlayer(GridSystem gridSystem)
    {
        //bool checkPlayers = false;
        Vector2 newPlayer = GetAndOccupyPosRange(gridSystem);
        if (newPlayer.x < 0 || newPlayer.y < 0)
            return false;

        //GameObject tempObject = GameObject.Instantiate(Resources.Load<GameObject>("PlayerQuad"));
        //tempObject.transform.position = newPlayer;

        m_playerList.Add(newPlayer);
        //Debug.Log(m_randomSpawnPoints.Count);

        return true;
    }

    public Vector2 GetWithoutOccupyRange(GridSystem gridSystem, bool occupyTile = false)
    {
        int randomNumber = UnityEngine.Random.Range(0, m_randomSpawnPoints.Count - 1);

        Vector2 newPosition = new Vector2(0, 0);
        while (m_randomSpawnPoints.Count > 0)
        {
            newPosition = m_randomSpawnPoints[randomNumber];
            if (HasSpace(gridSystem, newPosition))
                break;

            m_randomSpawnPoints.RemoveAt(randomNumber);
            randomNumber = UnityEngine.Random.Range(0, m_randomSpawnPoints.Count - 1);
        }
        if (m_randomSpawnPoints.Count <= 0)
            return new Vector2(-1, -1);

        if (occupyTile)
            m_randomSpawnPoints.RemoveAt(randomNumber);

        return newPosition;
    }

    public Vector2 GetAndOccupyPosRange(GridSystem gridSystem)
    {
        int randomNumber = UnityEngine.Random.Range(0, m_randomSpawnPoints.Count - 1);

        Vector2 newPosition = new Vector2(0, 0);
        while (m_randomSpawnPoints.Count > 0)
        {
            newPosition = m_randomSpawnPoints[randomNumber];
            if (HasSpace(gridSystem, newPosition))
                break;

            m_randomSpawnPoints.RemoveAt(randomNumber);
            randomNumber = UnityEngine.Random.Range(0, m_randomSpawnPoints.Count - 1);
        }
        if (m_randomSpawnPoints.Count <= 0)
            return new Vector2(-1, -1);

        m_randomSpawnPoints.RemoveAt(randomNumber);

        for (int i = m_randomSpawnPoints.Count - 1; i >= 0; i--)
        {
            /*  float a = m_randomSpawnPoints[i].y - newPosition.y;
              float b = Math.Abs(m_randomSpawnPoints[i].x - newPosition.x);*/

            if ((m_randomSpawnPoints[i] - newPosition).sqrMagnitude <= 75)//Math.Abs(m_randomSpawnPoints[i].x - newPosition.x) <= 10 || Math.Abs(m_randomSpawnPoints[i].y - newPosition.y) <=   10)
            {
                m_randomSpawnPoints.RemoveAt(i);
            }
        }

        return newPosition;
    }

    // return if the tile has floor tiles around it
    private bool HasSpace(GridSystem gSystem, Vector2 newPosition)
    {
        return true;
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                if (!gSystem.GetTile((int)newPosition.x + x, (int)newPosition.y + y).IsWalkable())
                    return false;
            }
        }
        return true;
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
            if (difference.x + difference.y < Mathf.Abs(generalDistance.x - positions[i].x) + Mathf.Abs(generalDistance.y - positions[i].y))
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
                if (gridSystem.GetTile(x, y).IsWalkable())
                    v.Add(new Vector2(x, y));
        return v;
    }

    private List<Vector2> GetUnusedPoints(List<Vector2> m_randomSpawnPoints)
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
