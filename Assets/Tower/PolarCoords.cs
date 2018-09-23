using System;
using UnityEngine;

[Serializable]
public struct PolarCoords
{

    [SerializeField]
    private float radAngle;
    public float RadAngle { get { return radAngle; } set { radAngle = Radian.Clamp(value); } }
    public float DegAngle { get { return radAngle * Mathf.Deg2Rad; } set { RadAngle = value * Mathf.Rad2Deg; } }

    [SerializeField]
    private float height;
    public float Height { get { return height; } set { height = value; } }

    public PolarCoords(float radAngle, float height) : this(radAngle, height, true)
    {

    }
    private PolarCoords(float radAngle, float height, bool check)
    {
        if (check) radAngle = Radian.Clamp(radAngle);
        this.radAngle = radAngle;
        this.height = height;
    }

    public Vector2 GetDisplacement(float radius = 1f)
    {
        return new Vector2(radAngle * radius, height);
    }
    public Vector3 GetPosition(float radius = 1f)
    {
        return new Vector3(Mathf.Sin(radAngle) * radius, height, -Mathf.Cos(radAngle) * radius);
    }

    public void Translate(Vector2 translate, float radius = 1f)
    {
        Translate(translate.x / radius, translate.y);
    }
    public void Translate(PolarCoords translate)
    {
        Translate(translate.radAngle, translate.height);
    }
    public void Translate(float rads, float height)
    {
        this.RadAngle += rads;
        this.height += height;
    }

    public static PolarCoords Lerp(PolarCoords from, PolarCoords to, float t)
    {
        return LerpUnclamped(from, to, Mathf.Clamp01(t));
    }
    public static PolarCoords LerpUnclamped(PolarCoords from, PolarCoords to, float t)
    {
        return new PolarCoords(
            Radian.Lerp(from.radAngle, to.radAngle, t),
            Mathf.LerpUnclamped(from.height, to.height, t),
            false
            );
    }


    public static PolarCoords SmoothDamp(PolarCoords from, PolarCoords to, ref PolarCoords currentVelocity,
    float smoothTime)
    {
        return SmoothDamp(from, to, ref currentVelocity, smoothTime, Time.deltaTime);
    }
    public static PolarCoords SmoothDamp(PolarCoords from, PolarCoords to, ref PolarCoords currentVelocity,
        float smoothTime, float deltaTime)
    {
        return new PolarCoords(
            Radian.SmoothDamp(from.radAngle, to.radAngle, ref currentVelocity.radAngle, smoothTime, deltaTime),
            Mathf.SmoothDamp(from.height, to.height, ref currentVelocity.height, smoothTime, Mathf.Infinity, deltaTime),
            false
            );
    }


    public static PolarCoords operator +(PolarCoords lhs, Vector2 rhs)
    {
        lhs.Translate(rhs);
        return lhs;
    }
    public static PolarCoords operator +(PolarCoords lhs, PolarCoords rhs)
    {
        lhs.Translate(rhs.radAngle, rhs.height);
        return lhs;
    }


    public static PolarCoords operator -(PolarCoords coords)
    {
        coords.radAngle = -coords.radAngle;
        coords.height = -coords.height;
        return coords;
    }
    public static PolarCoords operator -(PolarCoords lhs, Vector2 rhs)
    {
        lhs.Translate(-rhs);
        return lhs;
    }
    public static PolarCoords operator -(PolarCoords lhs, PolarCoords rhs)
    {
        lhs.Translate(-rhs);
        return lhs;
    }


    public static implicit operator PolarCoords(Vector2 v2)
    {
        return new PolarCoords(v2.x, v2.y);
    }

}
