using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FPSGameMode : MonoBehaviour
{
    private World2D world;
    private RaycastEntity targetEntity;
    private RaycastEntity objectiveEntity;

	void Start ()
	{
	    world = GetComponent<World2D>();

	    RoomGenerator generator = new RoomGenerator();
	    generator.GetGridSystem.SetGridSize(new IntVector2(100, 100));
        generator.GenerateRooms();


        world.worldMap = LinqConvert(generator.GetGridSystem.GetGrid);
	    GameObject[] o =  GameObject.FindGameObjectsWithTag("Player");
        List<FPSPlayer> players = new List<FPSPlayer>();
	    for (int i = 0; i < o.Length; i++)
	        players.Add(o[i].GetComponent<FPSPlayer>());

        for (int i = 0; i < generator.SpawnPointList.Count; i++)
        {
            players[i % players.Count].SpawnPos.Add(generator.SpawnPointList[i]);
            players[i % players.Count].GetComponent<FPSPlayer>().SetPosition(generator.SpawnPointList[i]);
        }

	    //get the target and objective entities
	    //targetEntity = generator

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

        RaycastEntity oldent = null;
        foreach (var entity in world.Entities)
        {
            if (oldent != null)
            {
                Debug.DrawLine(entity.Position, oldent.Position, Color.red, 0.1f);
            }
            oldent = entity;
        }

        foreach (var entity in world.Entities)
        {
            if (oldent != null)
            {
                Debug.DrawLine(entity.Position, oldent.Position, Color.red, 0.1f);
            }
            oldent = entity;

            if (entity.Properties.Contains("ItemA"))
            {
                if ((entity.Position - entity.Position).sqrMagnitude < 50)
                {

                }
            }
        }
    }
}
