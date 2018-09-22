using UnityEngine;

[RequireComponent(typeof(PolarCoordsTransform))]
public class PCoordMovement : MonoBehaviour
{

    [SerializeField]
    private float speed = 3f;

    [SerializeField]
    private float moveDamp = 10f;

    private Vector2 currentVelocity;

    private PolarCoordsTransform pcTransform;

    private void Awake()
    {
        pcTransform = GetComponent<PolarCoordsTransform>();
    }

    private void Update()
    {
        currentVelocity = Vector2.Lerp(currentVelocity,
            new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed,
            moveDamp * Time.smoothDeltaTime);
        pcTransform.Translate(currentVelocity * Time.deltaTime);
    }

}
