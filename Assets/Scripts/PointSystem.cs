using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSystem
{
    //  adding points Shooting player
    //  losing points getting Hit
    //  adding points Getting Object
    //  loosing points 

    public PointSystem()
    {
        m_points = 100;
    }

    private int m_points;
    public int Points { get { return m_points; } set { m_points = value; } }

    public void ShotPlayer() { m_points += 50; }
    public void GotShot() { m_points -= 20; }
    public void AchieveObject() { m_points += 40; }
    public void LoseObject() { m_points -= 15; }



    
}
