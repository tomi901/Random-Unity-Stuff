using UnityEngine;


public class Tower : MonoBehaviour
{

    private static Tower singleton;
    public static Tower Instance
    {
        get
        {
            if (!singleton) singleton = FindObjectOfType<Tower>();
            return singleton;
        }
    }

    [SerializeField]
    private float radius = 1f;
    public static float Radius { get { return Instance.radius; } }

    public static Vector3 WorldPosition { get { return Instance.transform.position; } }


    private void Awake()
    {
        singleton = this;
    }

}
