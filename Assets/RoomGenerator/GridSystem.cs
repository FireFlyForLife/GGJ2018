using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { NULL = -1, empty = 0, floor, wall, door, s01Wall, s02Wall, s03Wall, s04Wall
}

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

    public static bool IntersectsElement(Vector2 origin, Vector2 dir, World2D World, double CameraWidth, out Vector2 hitValue/*, IntVector2 tile, ref float distance*/)
    {
        //Debug.Log(dir);
        float PlaneX = 0, PlaneY = 0.66f;
    //calculate ray position and direction
        double cameraX = 2 * origin.x / (double)CameraWidth - 1; //x-coordinate in camera space
        double rayPosX = origin.x;
        double rayPosY = origin.y;
        double rayDirX = dir.x + PlaneX * cameraX;
        double rayDirY = dir.y + PlaneY * cameraX;

        //which box of the map we're in
        int mapX = (int)rayPosX;
        int mapY = (int)rayPosY;

        //length of ray from current position to next x or y-side
        double sideDistX;
        double sideDistY;

        //length of ray from one x or y-side to next x or y-side
        double deltaDistX = Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
        double deltaDistY = Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));
        //double perpWallDist;

        //what direction to step in x or y-direction (either +1 or -1)
        int stepX;
        int stepY;

        int hit = 0; //was there a wall hit?
        int side = -1; //was a NS or a EW wall hit?
                       //calculate step and initial sideDist
        if (rayDirX < 0)
        {
            stepX = -1;
            sideDistX = (rayPosX - mapX) * deltaDistX;
        }
        else
        {
            stepX = 1;
            sideDistX = (mapX + 1.0 - rayPosX) * deltaDistX;
        }
        if (rayDirY < 0)
        {
            stepY = -1;
            sideDistY = (rayPosY - mapY) * deltaDistY;
        }
        else
        {
            stepY = 1;
            sideDistY = (mapY + 1.0 - rayPosY) * deltaDistY;
        }
        //perform DDA
        while (hit == 0)
        {
            //jump to next map square, OR in x-direction, OR in y-direction
            if (sideDistX < sideDistY)
            {
                sideDistX += deltaDistX;
                mapX += stepX;
                side = 0;
            }
            else
            {
                sideDistY += deltaDistY;
                mapY += stepY;
                side = 1;
            }
            //Check if ray has hit a wall
            if (World.worldMap[mapX, mapY] > 0)
            {
                hit = 1;
            }
        }
        hitValue = new Vector2(0, 0);

        if (hit == 1)
        {
            //-----------------INSERTLOGIC HERE---------------------------/////////////
            hitValue = new Vector2(mapX, mapY);
            Debug.Log("You hit the wall");
            return true;
          
        }

        /*   Vector2 bMin = new Vector2(tile.X - .5f, tile.Y - .5f);
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

           if (tMin < distance)
               return true;

           return false;*/
        return false;
    }
}
