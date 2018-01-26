using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGenerator
{
    public DoorGenerator(GridSystem a_gridSystem)
    {
        GenerateDoors(a_gridSystem);
    }

    public void GenerateDoors(GridSystem a_gridSystem)
    {
        for(int i = 0; i < a_gridSystem.GridSize.X; i++)
        {
            for (int j = 0; j < a_gridSystem.GridSize.Y; j++)
            {
                if (a_gridSystem.GetTile(i, j) == TileType.floor)
                {
                    if (CheckForWalls(a_gridSystem, i, j) == true)
                    {
                        a_gridSystem.SetOccupied(i, j, TileType.door);
                        GameObject tile = GameObject.Instantiate(Resources.Load<GameObject>("DoorQuad"));
                        tile.transform.position = new Vector2(i, j);
                    }
                }
            }
        }
    }

    private bool CheckForWalls(GridSystem a_gridSystem, int a_gridIndexX, int a_gridIndexY)
    {
        if(    a_gridSystem.GetTile(a_gridIndexX, a_gridIndexY - 1) == TileType.wall
            && a_gridSystem.GetTile(a_gridIndexX, a_gridIndexY + 1) == TileType.wall 
            && a_gridSystem.GetTile(a_gridIndexX - 1, a_gridIndexY) != TileType.door
            && a_gridSystem.GetTile(a_gridIndexX + 1, a_gridIndexY) != TileType.door)
        {
            return true;
        }
        else if(a_gridSystem.GetTile(a_gridIndexX - 1, a_gridIndexY) == TileType.wall
            &&  a_gridSystem.GetTile(a_gridIndexX + 1, a_gridIndexY) == TileType.wall 
            &&  a_gridSystem.GetTile(a_gridIndexX, a_gridIndexY - 1) != TileType.door
            &&  a_gridSystem.GetTile(a_gridIndexX, a_gridIndexY + 1) != TileType.door)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
