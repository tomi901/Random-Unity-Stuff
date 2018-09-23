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

        // TODO: Update Rotation
    }

    public void Translate(Vector2 movement)
    {
        coords.Translate(movement, Tower.GetRadius + Separation);
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
