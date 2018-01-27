using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class DebugDraw : MonoBehaviour {
    public static DebugDraw Instance { get; private set; }

    public List<CircleShape> Circles = new List<CircleShape>();
    public List<Rect> Rectangles = new List<Rect>();

	void Start () {
		if(Instance)
            throw new Exception("DebugDraw already exists in the scene on: " + gameObject);

	    Instance = this;
	}

    void OnDrawGizmos()
    {
        if (!Instance)
            return;

        Gizmos.color = Color.red;
        foreach (var circle in Circles)
        {
            Debug.Log(circle.Position);
            Gizmos.DrawSphere(circle.Position, circle.Radius);
            Debug.DrawLine(circle.Position, circle.Position + new Vector2(circle.Radius, 0), Color.red, 1f);
            Debug.DrawLine(circle.Position, circle.Position + new Vector2(0, circle.Radius), Color.red, 1f);
        }

        Circles.Clear();
    }

    public static void DrawCircle(CircleShape circle)
    {
        Instance.Circles.Add(circle);
    }

    public static void DrawCircle(Vector3 position, float radius)
    {
        Instance.Circles.Add(new CircleShape(position, radius));
    }

    public static void DrawBox(Rect rect)
    {
        Instance.Rectangles.Add(rect);
    }

    public static void DrawBox(Vector2 pos, Vector2 size)
    {
        Instance.Rectangles.Add(new Rect(pos, size));
    }

    public static void DrawBox(float x, float y, float width, float height)
    {
        Instance.Rectangles.Add(new Rect(x, y, width, height));
    }
}
