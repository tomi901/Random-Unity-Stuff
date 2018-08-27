using System.Linq;

using UnityEngine;

namespace Game.Navigation
{
    public class ShortcutPathfinder2D : Pathfinder2D
    {

        public override Vector2 CalculateNextTarget()
        {
            return GetFurtherVisibleNode();
        }

        protected Vector2 GetFurtherVisibleNode ()
        {
            if (path.Count <= 0) return transform.position;

            int i = 1;
            foreach (Vector2 node in path.Reverse<Vector2>())
            {
                if (i >= path.Count) return node;

                if (CanGoToPoint(node)) return node;

                i++;
            }
            return transform.position;
        }
    }
}
