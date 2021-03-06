﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastEntity : MonoBehaviour
{
    public bool IsPlayer = false;

    public List<string> Properties = new List<string>();

    Vector2 dirVector = new Vector2();
    Vector2 planeVector = new Vector2();

    public int TextureId;

    public float X
    {
        set
        {
            var pos = transform.position;
            pos.x = value;
            transform.position = pos;
        }
        get { return transform.position.x; }
    }

    public float Y
    {
        set
        {
            var pos = transform.position;
            pos.y = value;
            transform.position = pos;
        }
        get { return transform.position.y; }
    }

    public Vector2 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

}
