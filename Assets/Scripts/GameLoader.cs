using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public struct LoadingBar
{
    
}

public class GameLoader
{
    private List<GameObject> m_windows;
    //private List<Transform> m_covers;
    private float m_delay;
    private int m_index;
    private int m_phase;

    public GameLoader(List<GameObject> windows)
    {
        m_windows = windows;
        //m_covers = new List<Transform>();

        //for (int i = 0; i < windows.Count; i++)
        //{
        //    m_covers.Add(windows[i].transform.Find("Cover"));
        //}

        m_windows = m_windows.OrderBy(a => Guid.NewGuid()).ToList();

        // the time to "open windows
        m_delay = UnityEngine.Random.Range(0.5f, 1.0f);
        m_index = 0;
        m_phase = 0;
    }

    // when false, start the game
    public bool ShouldUpdate()
    {
        if (m_phase == 0)
            ShowWindows();
        else if(m_phase == 1)
            LoadGames();

        if (m_phase > 1)
            return false;

        return true;
    }

    private void LoadGames()
    {

    }

    private void ShowWindows()
    {
        m_delay -= Time.deltaTime;

        if (m_delay <= 0)
        {
            m_windows[m_index].SetActive(true);
            m_index = ++m_index % m_windows.Count;

            if (m_index > 0)
                m_delay = UnityEngine.Random.Range(0.1f, 0.5f);
            else m_phase++;
        }
    }
}
