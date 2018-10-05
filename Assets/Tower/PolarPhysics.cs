using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace TowerGame.Physics
{
    public static class PolarPhysics
    {

        private static readonly List<PolarCollider> colliders = new List<PolarCollider>();


        internal static void AddCollider(PolarCollider collider)
        {
            colliders.Add(collider);
        }

        internal static void RemoveCollider(PolarCollider collider)
        {
            colliders.Remove(collider);
        }


        public struct RaycastHitPolar
        {

            public readonly PolarCollider collider;
            public readonly float fraction;

            public RaycastHitPolar(PolarCollider collider, float fraction)
            {
                this.collider = collider;
                this.fraction = fraction;
            }

        }

        public static RaycastHitPolar Raycast(PolarCoords origin, Vector2 direction)
        {
            return RaycastAll(origin, direction).FirstOrDefault();
        }

        public static IEnumerable<RaycastHitPolar> RaycastAll(PolarCoords origin, Vector2 direction)
        {
            yield return new RaycastHitPolar();
        }

    }
}