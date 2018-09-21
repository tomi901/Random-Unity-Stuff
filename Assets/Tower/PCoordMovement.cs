using UnityEngine;

[RequireComponent(typeof(PolarCoordsTransform))]
public class PCoordMovement : MonoBehaviour
{

    [SerializeField]
    private float speed = 3f;

    private Vector2 currentVelocity;

    private PolarCoordsTransform pcTransform;

    private void Awake()
    {
        pcTransform = GetComponent<PolarCoordsTransform>();
    }

    private void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        pcTransform.Translate(input * speed * Time.deltaTime);
    }

}
