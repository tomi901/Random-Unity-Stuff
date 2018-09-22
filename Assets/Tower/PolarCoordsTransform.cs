using System;
using UnityEngine;


public class PolarCoordsTransform : MonoBehaviour
{

    [SerializeField]
    private PolarCoords coords;
    public PolarCoords Coordinates { get { return coords; } set { coords = value; UpdatePosition(); } }

    [SerializeField]
    private float separation = 0f;
    public virtual float Separation { get { return separation; } }

    private Vector3 relativePos;
    public Vector3 RelativePosition => relativePos;
    public Vector3 WorldPosition => transform.position;


    private void UpdatePosition()
    {
        Tower tower = Tower.Instance;
        float distance = tower.Radius + Separation;

        relativePos = coords.GetPosition(distance);
        transform.position = tower.transform.position + RelativePosition;
    }

    public void Translate(Vector2 movement)
    {
        movement.x /= Tower.GetRadius + Separation;
        Coordinates += movement;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        //position.ClampAngle();
        Coordinates = coords;
    }
#endif

}

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

    public Vector3 GetPosition(float xzDistance = 1f)
    {
        return new Vector3(Mathf.Sin(radAngle) * xzDistance, height, -Mathf.Cos(radAngle) * xzDistance);
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
        lhs.Translate(rhs.x, rhs.y);
        return lhs;
    }
    public static PolarCoords operator +(PolarCoords lhs, PolarCoords rhs)
    {
        lhs.Translate(rhs.radAngle, rhs.height);
        return lhs;
    }
    public static implicit operator PolarCoords(Vector2 v2)
    {
        return new PolarCoords(v2.x, v2.y);
    }

}
