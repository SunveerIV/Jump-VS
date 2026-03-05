using UnityEngine;

namespace Game.Utility {
    public static class PhysicsTools {
        public static void StickTo(this Component self, GameObject target) {
            if (target == null) return;

            // Clear old joints to guarantee exactly one
            self.Unstick();

            var joint = self.gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = target.GetComponent<Rigidbody2D>();
        }

        public static void Unstick(this Component self) {
            // Remove all FixedJoint2D components on this object
            foreach (var joint in self.GetComponents<FixedJoint2D>()) {
                Object.Destroy(joint);
            }
        }
    }
}