using System;
using UnityEngine;


public class PolarCoordsTransform : MonoBehaviour
{

    [SerializeField]
    private PolarCoords coords;
    public PolarCoords Coordinates { get { return coords; } set { coords = value; UpdatePosition(); } }

    [SerializeField]
    private float separation = 0f;

    private void UpdatePosition()
    {
        Tower tower = Tower.Instance;
        float distance = tower.Radius + separation;

        transform.position = coords.GetPosition(distance) + tower.transform.position;
    }

    public void Translate(Vector2 movement)
    {
        movement.x /= Tower.GetRadius + separation;
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

    private const float PI2 = Mathf.PI * 2f;

    [SerializeField]
    private float radAngle;
    public float RadAngle { get { return radAngle; } set { radAngle = value; ClampAngle(); } }
    public float DegAngle { get { return radAngle * Mathf.Deg2Rad; } set { RadAngle = value * Mathf.Rad2Deg; } }

    [SerializeField]
    private float height;
    public float Height { get { return height; } set { height = value; } }

    public PolarCoords(float radAngle, float height) : this(radAngle, height, true)
    {

    }
    private PolarCoords(float radAngle, float height, bool check)
    {
        if (check) radAngle %= PI2;
        this.radAngle = radAngle;
        this.height = height;
    }

    public void ClampAngle()
    {
        radAngle %= PI2;
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
