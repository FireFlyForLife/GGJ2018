using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionGenerator : MonoBehaviour {

    public SectionGenerator(GridSystem system, TileType lt, TileType rt, TileType lb, TileType rb)
    {
        int gSizeX = system.GridSize.X;
        int gSizeY = system.GridSize.Y;

        for (int x = 0; x < gSizeX; x++)
        {
            bool left = (x < gSizeX / 2);
            for (int y = 0; y < gSizeY; y++)
            {
                bool top = (y < gSizeY / 2);
                if (top)
                {
                    if(left) system.SetOccupied(x,y,lt);
                    else system.SetOccupied(x,y,rt);
                }
                else if(left) system.SetOccupied(x,y,lb);
                else system.SetOccupied(x,y,rb);
            }
        }
    }
}
