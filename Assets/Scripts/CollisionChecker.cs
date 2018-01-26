using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

namespace Shapes
{
    public class CircleShape
    {
        public Vector2 Position;
        public float Radius = 0;

        public CircleShape(Vector2 pos, float radius)
        {
            Position = pos;
            Radius = radius;
        }
    }

    public class LineSegment
    {
        public Vector2 Start;
        public Vector2 End;

        public LineSegment(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
    }
}

//public static class CollisionChecker {
//    public static bool HasOverlap(CircleShape circle, LineSegment line)
//    {
//        Debug.DrawLine(line.Start, line.End, Color.red, 1f);
//        DebugDraw.DrawCircle(circle);

//        Vector2 relPos = circle.Position - line.Start;
//        Vector2 lineDir = line.End - line.Start;
//        float lineDot = Vector2.Dot(lineDir, relPos);
//        Vector2 closestPointOnLine = lineDir * lineDot;
//        Vector2 circleToClosestPoint = closestPointOnLine - circle.Position;
//        Debug.DrawLine(line.Start, line.Start + circleToClosestPoint, Color.cyan, 1f);

//        return circleToClosestPoint.SqrMagnitude() <= Mathf.Pow(circle.Radius, 2);
//    }

//    public static bool CheckLine(LineSegment line, List<RaycastEntity> entities)
//    {
//        foreach (RaycastEntity entity in entities)
//        {
//            CircleShape circle = new CircleShape(new Vector2((float) entity.X, (float) entity.Y), 1f);
//            if (HasOverlap(circle, line))
//                return true;
//        }

//        return false;
//    }
//}
