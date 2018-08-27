using System.Collections.Generic;
using UnityEngine;


public class Portal : MonoBehaviour
{

    public Portal otherPortal;

    private List<Transform> transformsInPortal = new List<Transform>();

    public Vector3 Position { get { return transform.position; } }
    public Vector3 LocalPosition { get { return transform.localPosition; } }

    public Quaternion Rotation { get { return transform.rotation; } }

    void Teleport (Transform otherTransform)
    {
        Vector3 positionOffset = otherTransform.position - transform.position;
        float dotProduct = Vector3.Dot(transform.forward, positionOffset);


        if (dotProduct < 0f) SetPortalRelativeTransform(otherTransform);
    }


    public void SetPortalRelativeTransform(Transform transform)
    {
        SetPortalRelativeTransform(transform, transform);
    }
    public void SetPortalRelativeTransform (Transform transform, Transform relativeFrom)
    {
        transform.position = GetOtherPortalRelativePosition(relativeFrom);
        transform.rotation = GetOtherPortalRelativeRotation(relativeFrom);
    }

    /// <summary>
    /// Gets the global position if we go from this portal to the other portal
    /// </summary>
    /// <param name="transform">The transform to "teleport"</param>
    /// <returns>World position teleported</returns>
    public Vector3 GetOtherPortalRelativePosition(Transform transform)
    {
        return GetOtherPortalRelativePosition(transform.position);
    }
    public Vector3 GetOtherPortalRelativePosition(Vector3 position)
    {
        Vector3 relativePosition = transform.InverseTransformPoint(position);
        return otherPortal.transform.TransformPoint(relativePosition.Mirrored());
    }

    /// <summary>
    /// Gets the global rotation if we go from this portal to the other portal
    /// </summary>
    /// <param name="transform">The transform to "teleport"</param>
    /// <returns>World rotation teleported</returns>
    public Quaternion GetOtherPortalRelativeRotation(Transform transform)
    {
        return GetOtherPortalRelativeRotation(transform.forward);
    }
    public Quaternion GetOtherPortalRelativeRotation(Vector3 forwardDirection)
    {
        Vector3 relativeDirection = transform.InverseTransformDirection(forwardDirection);
        Vector3 lookRotation = otherPortal.transform.TransformDirection(relativeDirection.Mirrored());
        return Quaternion.LookRotation(lookRotation, otherPortal.transform.up);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (transformsInPortal.Contains(other.transform)) return;

        Teleport(other.transform);

        transformsInPortal.Add(other.transform);
        otherPortal.transformsInPortal.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        transformsInPortal.Remove(other.transform);
    }

}
