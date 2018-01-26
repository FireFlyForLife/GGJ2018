using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    up,
    right,
    down,
    left,
    none
}

public class RoomGenerator
{
    private GameManager m_gameManager;
    private GameObject m_prefab;
    private bool m_generating;

    private Dictionary<Direction, IntVector2> m_directions = new Dictionary<Direction, IntVector2>();
    private Stack<RoomTransform> m_availablePositions;

    RoomTransform m_transform;

    public void Initialize(GameManager gSystem, IntVector2 position)
    {
        m_gameManager = gSystem;

        m_directions.Add(Direction.up, new IntVector2(0, 1));
        m_directions.Add(Direction.down, new IntVector2(0, -1));
        m_directions.Add(Direction.left, new IntVector2(-1, 0));
        m_directions.Add(Direction.right, new IntVector2(1, 0));

        m_availablePositions = new Stack<RoomTransform>();
        m_transform = new RoomTransform(new IntVector2(5, 5), Direction.none, 0);
        m_prefab = Resources.Load<GameObject>("FloorQuad");
    }

    public void GenerateRooms()
    {
        m_generating = true;
        while(m_generating)
        {
            int roomSize = GetRoomSize();

            // update the current transform to the new room's center after placing it
            m_transform = SetTiles(m_transform, roomSize);
        }
    }

    List<RoomTransform> GetTransformations(RoomTransform transform, int roomSize)
    {
        List<RoomTransform> transforms = new List<RoomTransform>();

        for (int i = 0; i < 4; i++)
        {
            int toEdge = 0;
            if (m_transform.direction != Direction.none)
            {
                // if i is backwards, skip
                if (i == (((int)transform.direction + 2) % 4))
                    continue;

                // from the center to the edge of the placed tiles
                toEdge = (int)(((float)m_transform.roomSize / 2f) - .5);
            }

            // from the edge to the edge to the edge of a new room with the given size
            int toRoomCenter = toEdge + (int)(((float)roomSize / 2f) + .5);

            // the center of the new possible
            IntVector2 newRoomCenter = new IntVector2
            {
                // the position at the edge of the selected side
                X = transform.centerPosition.X + (toRoomCenter * m_directions[(Direction)i].X),
                Y = transform.centerPosition.Y + (toRoomCenter * m_directions[(Direction)i].Y)
            };

            // create a transform with the given position and direction
            RoomTransform target = new RoomTransform(newRoomCenter, (Direction)i, roomSize);

            // add to the possible targets when all tiles are free
            if (IsPositionValid(target, roomSize))
                transforms.Add(target);
        }
        return transforms;
    }

    bool IsPositionValid(RoomTransform transform, int roomSize)
    {
        List<IntVector2> positions = GetTiles(transform.centerPosition, roomSize);
        return m_gameManager.Get<GridSystem>().IsAvailable(positions);
    }

    private List<IntVector2> GetTiles(IntVector2 pos, int roomSize)
    {
        List<IntVector2> positions = new List<IntVector2>();

        // when the room size is 1 there will only be one tile
        if (roomSize == 1) return new List<IntVector2>(1) { pos };

        for (int yI = -roomSize / 2; yI <= roomSize / 2; yI++)
        {
            for (int xI = -roomSize / 2; xI <= roomSize / 2; xI++)
            {
                int x = pos.X + xI;
                int y = pos.Y + yI;
                positions.Add(new IntVector2(x, y));
            }
        }
        return positions;
    }

    private RoomTransform SetTiles(RoomTransform transform, int roomSize)
    {
        List<RoomTransform> transforms = GetTransformations(transform, roomSize);

        // if the current transform has options to place a room with the given size
        if (transforms.Count > 0)
        {
            RoomTransform selectedTransform = transform;
            // choose a random option
            int selected = Random.Range(0, transforms.Count - 1);
            for (int i = 0; i < transforms.Count; i++)
            {
                if (i == selected)
                {
                    SetTiles(GetTiles(transforms[selected].centerPosition, roomSize));
                    selectedTransform = transforms[selected];
                }

                else
                {
                    // push only 75% of the total possible paths to the stack
                    if (Random.Range(0, 100) > 50 && roomSize > 3 || roomSize == 9)
                    m_availablePositions.Push(transforms[i]);
                }
            }

            // the compiler does not know this will never be triggered
            return selectedTransform;
        }
        // if there are still options stacked
        else if (m_availablePositions.Count > 0)
        {
            RoomTransform target = m_availablePositions.Pop();
            SetTiles(GetTiles(target.centerPosition, target.roomSize));
            return new RoomTransform(target);
        }

        // if there are no more options, clear the builder
        else
        {
            Clear();
            return transform;
        }
    }

    private void SetTiles(List<IntVector2> positions)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            UnityEngine.GameObject tile = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Quad);
            tile.transform.position = new UnityEngine.Vector2(positions[i].X, positions[i].Y);
            m_gameManager.Get<GridSystem>().SetOccupied(positions[i].X, positions[i].Y, TileType.floor);
        }
    }

    private int GetRoomSize()
    {
        int roomSize = Random.Range(1, 6);

        switch (roomSize)
        {
            case 2: return 3;
            case 4: return 5;
            case 6: return 9;
        }

        return roomSize;
    }

    public void Clear()
    {
        m_generating = false;
        new WallGenerator(m_gameManager.Get<GridSystem>()).PlaceWalls();
    }
}

public struct RoomTransform
{
    public RoomTransform(IntVector2 pos, Direction dir, int size) { centerPosition = pos; direction = dir; roomSize = size; }
    public RoomTransform(RoomTransform a) { centerPosition = a.centerPosition; direction = a.direction; roomSize = a.roomSize; }
    public IntVector2 centerPosition;
    public Direction direction;
    public int roomSize;
}