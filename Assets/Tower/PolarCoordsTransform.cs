using UnityEngine;


public class PolarCoordsTransform : MonoBehaviour
{

    [SerializeField]
    private PolarCoords coords;
    public PolarCoords Coordinates { get { return coords; } set { coords = value; UpdatePosition(); } }

    [SerializeField]
    private float separation = 0f;
    public virtual float Separation { get { return separation; } }

    [SerializeField]
    private bool updateRotation = false;

    private Vector3 relativePos;
    public Vector3 RelativePosition => relativePos;
    public Vector3 WorldPosition => transform.position;


    private void UpdatePosition()
    {
        float distance = Tower.Radius + Separation;

        relativePos = coords.GetPosition(distance);
        transform.position = Tower.WorldPosition + RelativePosition;

        if (updateRotation)
        {
            transform.rotation = Quaternion.Euler(0, -coords.DegAngle, 0);
        }
    }

    public void Translate(Vector2 movement)
    {
        coords += new PolarCoords(movement, Tower.Radius + Separation);
        UpdatePosition();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        //position.ClampAngle();
        Coordinates = coords;
    }
#endif

}
