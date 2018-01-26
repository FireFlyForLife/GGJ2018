using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Texture2DExtensions {
	public static void DrawLine(this Texture2D tex, Vector2 p1, Vector2 p2, Color col)
	{
	    Vector2 t = p1;
	    float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
	    float ctr = 0;

	    while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
	    {
	        t = Vector2.Lerp(p1, p2, ctr);
	        ctr += frac;
	        tex.SetPixel((int)t.x, (int)t.y, col);
	    }
	}

    public static void DrawLine(this Texture2D tex, float startX, float startY, float endX, float endY, Color col)
    {
        tex.DrawLine(new Vector2(startX, startY), new Vector2(endX, endY), col);
    }
}
