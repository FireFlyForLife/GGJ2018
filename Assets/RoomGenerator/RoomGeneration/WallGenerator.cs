using UnityEngine;
using System.Collections;

public class WallGenerator
{
    private GridSystem m_gridSystem;
    private int[][] m_grid;
    private GameObject m_wallObject;

    public WallGenerator(GridSystem gridSystem)
    {
        m_gridSystem = gridSystem;
        m_wallObject = Resources.Load<GameObject>("BlackQuad");
    }


    public void PlaceWalls()
    {
        for (int x = 0; x < m_gridSystem.GridSize.X; x++)
        {
            for (int y = 0; y < m_gridSystem.GridSize.Y; y++)
            {
                if (m_gridSystem.IsAvailable(x, y))
                {
                    CheckTile(x, y);
                }

                if (IsEdgePosition(x, y) && m_gridSystem.GetTile(x, y) == TileType.floor)
                    OccupyWall(x,y);
            }
        }
    }

    private void CheckTile(int x, int y)
    {

        for (int yPlus = y - 1; yPlus <= y + 1; yPlus++)
        {
            for (int xPlus = x - 1; xPlus <= x + 1; xPlus++)
            {
                if (m_gridSystem.GetTile(xPlus, yPlus) == TileType.floor)
                {
                    OccupyWall(x, y);
                    return;
                }
            }
        }
    }

    private void OccupyWall(int x, int y)
    {
        m_gridSystem.SetOccupied(x, y, TileType.wall);
        GameObject tile = GameObject.Instantiate(m_wallObject);
        tile.transform.position = new Vector2(x, y);
    }

    private bool IsEdgePosition(int x, int y)
    {
        return (x == 0 || y == 0 || x == m_gridSystem.GridSize.X - 1 || y == m_gridSystem.GridSize.Y - 1);
    }
}
