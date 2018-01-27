using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastResources : MonoBehaviour
{
    public static RaycastResources Instance;

    public Sprite[] SpriteRegistery;
    public Texture2D[] Textures;

    void Start()
    {
        if(Instance)
            throw new Exception("RaycastResources is already in the scene! the second instance is on: " + gameObject);

        Instance = this;
    }
}
