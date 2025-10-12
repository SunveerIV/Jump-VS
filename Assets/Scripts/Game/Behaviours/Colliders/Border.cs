using UnityEngine;
using Unity.Netcode;
using Game.Constants;

namespace Game.Behaviours.Colliders {
    public class Border : NetworkBehaviour {

        [SerializeField] private Transform masterTransform;
        
        public static Border Create(Border prefab) {
            var border = Instantiate(prefab);
            return border;
        }

        public void UpdateTransform(float yPosition, float yDifference) {
            Move(yPosition);
            Scale(yDifference);
        }

        private void Move(float yPosition) {
            Vector3 position = masterTransform.position;
            position.y = yPosition;
            masterTransform.position = position;
        }

        private void Scale(float yDifference) {
            Vector3 scale = masterTransform.localScale;
            scale.y = yDifference * BorderConst.SCALE_SCALE + 1f;
            masterTransform.localScale = scale;
        }
    }
}