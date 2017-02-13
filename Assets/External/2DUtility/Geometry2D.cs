using UnityEngine;
using System.Collections;

public class Geometry2D
{
    const float EPS = 0.001f;

    public class Line
    {
        public Vector2 A;
        public Vector2 B;
        public Line(Vector2 A, Vector2 B)
        {
            this.A = A;
            this.B = B;
        }
    }

    public static Vector2 RotateVector(Vector2 vec, float rad)
    {
        Vector2 ret = Vector2.zero;
        ret.x = vec.x * Mathf.Cos(rad) - vec.y * Mathf.Sin(rad);
        ret.y = vec.x * Mathf.Sin(rad) + vec.y * Mathf.Cos(rad);
        return ret;
    }

    public static float GetAngle(Vector2 a, Vector2 b)
    {
        float angle = Mathf.Atan2(b.y, b.x) - Mathf.Atan2(a.y, a.x);
        return GetLimitedAngle(angle);
    }

    public static float GetLimitedAngle(float rad)
    {
        float eps = 0.00001f;
        while (rad <= -Mathf.PI) rad += Mathf.PI * 2;
        while (rad > Mathf.PI + eps) rad -= Mathf.PI * 2;
        return rad;
    }

    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - b.x * a.y;
    }
    public static float Dot(Vector2 a, Vector2 b)
    {
        return Vector2.Dot(a, b);
    }

    public static bool CCW(Vector2 a, Vector2 b, Vector2 c)
    {
        return Cross(b - a, c - a) >= 0;
    }

    public static Vector2 Projection(Line l, Vector2 p)
    {
        float t = Dot(p - l.A, l.A - l.B) / (l.A - l.B).sqrMagnitude;
        return l.A + t * (l.A - l.B);
    }

    public static Vector2 Reflection(Line l, Vector2 p)
    {
        return p + 2 * (Projection(l, p) - p);
    }

    public static bool IntersectSP(Line s, Vector2 p)
    {
        return (s.A - p).magnitude + (s.B - p).magnitude - (s.B - s.A).magnitude < EPS;
    }

    public static float DistanceLP(Line l, Vector2 p)
    {
        return (p - Projection(l, p)).magnitude;
    }
    public static float DistanceSP(Line s, Vector2 p)
    {
        Vector2 r = Projection(s, p);
        if (IntersectSP(s, r)) return (r - p).magnitude;
        return Mathf.Min((s.A - p).magnitude, (s.B - p).magnitude);
    }

    public static Rect GetRectFromBoxCollider(Transform transform, BoxCollider2D boxColider)
    {

        Vector2 sc = transform.localScale;
        Vector2 pos = new Vector2(boxColider.offset.x * sc.x, boxColider.offset.y * sc.y) + (Vector2)transform.position;
        Vector2 size = new Vector2(boxColider.size.x * sc.x, boxColider.size.y * sc.y);

        return new Rect(pos - size / 2, size);
    }
}