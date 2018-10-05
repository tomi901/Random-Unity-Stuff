using UnityEngine;

namespace TowerGame.Physics
{

    public abstract class PolarCollider : MonoBehaviour
    {

        private PolarCoordsTransform polarTransform;
        public PolarCoordsTransform PolarTransform
        {
            get
            {
                if (polarTransform == null) polarTransform = GetComponent<PolarCoordsTransform>();
                return polarTransform;
            }
        }

        private void Awake()
        {
            PolarPhysics.AddCollider(this);
            OnAwake();
        }
        protected virtual void OnAwake()
        {
            PolarPhysics.Raycast(PolarCoords.Zero, Vector2.zero);
        }

    }

}