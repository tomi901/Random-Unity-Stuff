using UnityEngine;


public static class Radian
{

    /// <summary>
    /// 360 degrees in radians
    /// </summary>
    public const float PI2 = Mathf.PI * 2f;
    /// <summary>
    /// 540 degrees in radians
    /// </summary>
    public const float PI3 = Mathf.PI * 3f;


    public static float Clamp(float angle)
    {
        return angle % PI2;
    }

    public static float DeltaAngle(float a, float b)
    {
        return Clamp(Clamp(b - a) + PI3) - Mathf.PI;
    }

    public static float Lerp(float a, float b, float t)
    {
        return a + DeltaAngle(a, b) * t;
    }

    public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime)
    {
        return SmoothDamp(current, target, ref currentVelocity, smoothTime, Time.deltaTime);
    }
    public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, 
        float deltaTime)
    {
        return Mathf.SmoothDamp(current, current + DeltaAngle(current, target), ref currentVelocity, smoothTime, 
            Mathf.Infinity, deltaTime);
    }

}
