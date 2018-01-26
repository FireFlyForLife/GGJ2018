using System;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private Dictionary<Type, GameSystem> m_gameSystems;
    private List<GameSystem> m_updatableSystems;

    void Start()
    {
        //m_gameSystems = new Dictionary<Type, GameSystem>();
        //m_updatableSystems = new List<GameSystem>();

        //Add<GridSystem>();

        // generate the rooms
        RoomGenerator generator = new RoomGenerator();
        generator.GetGridSystem.SetGridSize(new IntVector2(100,100));
        generator.GenerateRooms();

        int[][] grid = generator.GetGridSystem.GetGrid;
        TileType type = generator.GetGridSystem.GetTile(0, 1);
    }

    void Update()
    {
        //for (int i = 0; i < m_updatableSystems.Count; i++)
        //{
        //    m_updatableSystems[i].Update();
        //}
    }

    public T Get<T>() where T : GameSystem
    {
        if (!m_gameSystems.ContainsKey(typeof(T)))
            Add<T>();

        return (T)m_gameSystems[typeof(T)];
    }

    private void Add<T>() where T : GameSystem
    {
        if (m_gameSystems.ContainsKey(typeof(T)))
            return;

        m_gameSystems.Add(typeof(T), Activator.CreateInstance<T>());
        m_gameSystems[typeof(T)].Initialize(this);
    }

    public void SetUpdatable<T>(T instance) where T : GameSystem
    {
        if (!m_updatableSystems.Contains(instance))
        {
            m_updatableSystems.Add(instance);
        }
    }

    public void RemoveUpdatable<T>(T instance) where T : GameSystem
    {
        if (m_updatableSystems.Contains(instance))
        {
            m_updatableSystems.Remove(instance);
        }
    }
}
