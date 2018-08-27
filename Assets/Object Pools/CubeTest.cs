using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeTest : MonoBehaviour
{

    public float minHeight = 5f;
    public float maxHeight = 10f;

    public float maxHorizontal = 3f;

    public float rotationMultiplier = 10f;


    new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Vector2 randomHorizontal = Random.insideUnitCircle;

        Vector3 velocity = new Vector3(randomHorizontal.x, Random.Range(minHeight, maxHeight), randomHorizontal.y);

        rigidbody.AddForce(velocity, ForceMode.VelocityChange);
        if (rotationMultiplier != 0) rigidbody.AddTorque(Random.onUnitSphere * 10f, ForceMode.VelocityChange);
    }

}
