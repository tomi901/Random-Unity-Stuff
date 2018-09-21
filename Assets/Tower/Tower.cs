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
    public float Radius { get { return radius; } }
    public static float GetRadius { get { return singleton.radius; } }

    private void Awake()
    {
        singleton = this;
    }

}
