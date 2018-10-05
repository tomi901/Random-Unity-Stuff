using UnityEngine;

namespace TowerGame.Physics
{

    [RequireComponent(typeof(PolarCoordsTransform))]
    public class PolarBoxCollider : PolarCollider
    {

        [SerializeField]
        private Vector2 size = Vector2.one;
        public PolarCoords PolarCoordsSize { get { return PolarCoords.FromVector2(size, Tower.Radius); } }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(0, -PolarTransform.Coordinates.DegAngle, 0), transform.lossyScale);

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, size.y, 1f));
        }

    }

}
