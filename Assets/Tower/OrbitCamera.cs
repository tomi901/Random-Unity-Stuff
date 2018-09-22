using UnityEngine;


public class OrbitCamera : PolarCoordsTransform
{

    [Header("Camera")]

    [SerializeField]
    private PolarCoordsTransform target;
    private PolarCoords TargetPos
    {
        get
        {
            PolarCoords coords = target.Coordinates;
            coords.Height += yOffset;
            return coords;
        }
    }

    [SerializeField]
    private float yOffset = 1f;

    [SerializeField]
    private float smoothTime = 0.1f;

    private PolarCoords currentVelocity;

    public override float Separation => target != null ? target.Separation + base.Separation : base.Separation;


    private void Start()
    {
        Coordinates = TargetPos;
    }

    private void LateUpdate()
    {
        Coordinates = PolarCoords.SmoothDamp(Coordinates, TargetPos, ref currentVelocity, smoothTime);
        transform.rotation = Quaternion.LookRotation(target.WorldPosition - WorldPosition, Vector3.up);
    }

}
