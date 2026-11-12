using UnityEngine;

namespace Game.Utility {
    public static class PhysicsTools {
        /// <summary>
        /// Sticks the "this" object to the target object via FixedJoint2D
        /// </summary>
        /// <param name="self">The child object</param>
        /// <param name="target">The parent object</param>
        public static void StickTo(this Component self, GameObject target) {
            if (target == null) return;

            // Clear old joints to guarantee exactly one
            self.Unstick();

            var joint = self.gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = target.GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Unsticks the "this" object from all FixedJoint2D joints
        /// </summary>
        /// <param name="self">The child object</param>
        public static void Unstick(this Component self) {
            // Remove all FixedJoint2D components on this object
            foreach (var joint in self.GetComponents<FixedJoint2D>()) {
                Object.Destroy(joint);
            }
        }
    }
}