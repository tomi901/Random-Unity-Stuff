using UnityEngine;

public static class Extensions
{

    public static Vector3 Mirrored(this Vector3 point)
    {
        point.x *= -1;
        point.z *= -1;
        return point;
    }

}
