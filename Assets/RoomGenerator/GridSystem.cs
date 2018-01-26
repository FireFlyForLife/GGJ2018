using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { NULL = -1, empty = 0, floor, wall, door}

public class GridSystem : GameSystem
{
    public void Initialize (int sizeX, int sizeY)
    {
        m_grid = new int[sizeY][];
        for (int i = 0; i < sizeY; i++)
        {
            m_grid[i] = new int[sizeX];
        }

        m_gridSize.X = sizeX;
        m_gridSize.Y = sizeY;
    }
    private int[][] m_grid;

    public int[][] GetGrid { get { return m_grid; } set { m_grid = value; } }

    private IntVector2 m_gridSize;
    public IntVector2 GridSize { get { return m_gridSize; } }

    public override void Initialize(GameManager a_gameManager)
    {
        base.Initialize(a_gameManager);
        SetGridSize(new IntVector2(100, 100));
    }

    public void SetGridSize(IntVector2 a_size)
    {
        m_grid = new int[a_size.Y][];
        for (int i = 0; i < a_size.Y; i++)
        {
            m_grid[i] = new int[a_size.X];
        }
        m_gridSize = a_size;
    }

    public bool IsAvailable(int x, int y)
    {
        if (!IsWithinGrid(x, y)) return false;
        return m_grid[y][x] == 0;
    }

    public bool IsAvailable(List<IntVector2> positions)
    {
        bool valid = true;
        for (int i = 0; i < positions.Count; i++)
        {
            if (!IsAvailable(positions[i].X, positions[i].Y))
                valid = false;
        }
        return valid;
    }

    public void SetOccupied(int x, int y, TileType type)
    {
        m_grid[y][x] = (int)type;
    }

    public TileType GetTile(int x, int y)
    {
        if (!IsWithinGrid(x, y)) return TileType.NULL;
        else return (TileType)m_grid[y][x];
    }

    public IntVector2 GetCenter()
    {
        return new IntVector2((int)(m_grid[0].Length / 2), (int)(m_grid.Length)/2);
    }

    private bool IsWithinGrid(int x, int y)
    {
        if (m_grid.Length <= y || m_grid[0].Length <= x) return false;
        if (x < 0 || y < 0) return false;
        return true;
    }

    public override void Update() { }

    public override void Clear()
    {
        //
    }

    static bool IntersectsElement(Vector2 origin, Vector2 dir, IntVector2 tile)
    {
        Vector2 bMin = new Vector2(tile.X - .5f, tile.Y - .5f);
        Vector2 bMax = new Vector2(tile.X + .5f, tile.Y + .5f);

        float tMin = (bMin.x - origin.x) / dir.x;
        float tmax = (bMax.x - origin.x) / dir.x;

        if (tMin > tmax)
        {
            float t = tMin;
            tMin = tmax;
            tmax = t;
        }

        float tyMin = (bMin.y - origin.y) / dir.y;
        float tyMax = (bMax.y - origin.y) / dir.y;

        if (tyMin > tyMax)
        {
            float t = tyMin;
            tyMin = tyMax;
            tyMax = t;
        }

        if ((tMin > tyMax) || (tyMin > tmax))
            return false;

        if (tyMin > tMin)
            tMin = tyMin;

        if (tyMax < tmax)
            tmax = tyMax;

        return true;
    }
}
