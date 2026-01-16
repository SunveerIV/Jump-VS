using UnityEngine;
using Unity.Netcode;
using Game.Constants;

namespace Game.Behaviours.Managers {
    public class BorderManager : NetworkBehaviour {
        
        public static BorderManager Create(BorderManager prefab) {
            var border = Instantiate(prefab);
            return border;
        }

        public void UpdateTransform(float yPosition , float yDifference = 0f) {
            Move(yPosition);
            Scale(yDifference);
        }

        private void Move(float yPosition) {
            Vector3 position = transform.position;
            position.y = yPosition;
            transform.position = position;
        }

        private void Scale(float yDifference) {
            Vector3 scale = transform.localScale;
            scale.y = Mathf.Abs(yDifference) + BorderConst.MIN_BORDER_SIZE;
            transform.localScale = scale;
        }
    }
}