using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FPSGameMode : MonoBehaviour
{
    private World2D world;

	void Start ()
	{
	    world = GetComponent<World2D>();

	    RoomGenerator generator = new RoomGenerator();
	    generator.GetGridSystem.SetGridSize(new IntVector2(100, 100));
	    generator.GenerateRooms();

	    //world.worldMap = LinqConvert(generator.GetGridSystem.GetGrid);
	    
	}

    static int[,] LinqConvert(int[][] source)
    {
        return new[] { new int[source.Length, source[0].Length] }
            .Select(_ => new { x = _, y = source.Select((a, ia) => a.Select((b, ib) => _[ia, ib] = b).Count()).Count() })
            .Select(_ => _.x)
            .First();
    }

    void Update ()
	{
		
	}
}
