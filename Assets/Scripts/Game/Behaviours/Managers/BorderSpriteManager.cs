using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Constants;

namespace Game.Behaviours.Managers {
    public class BorderSpriteManager : MonoBehaviour {

        [SerializeField] private GameObject borderSpriteObjectPrefab;
        
        private Transform trackingTransform;
        
        private SortedDictionary<int, GameObject> leftBlocks;
        private SortedDictionary<int, GameObject> rightBlocks;

        public static BorderSpriteManager Create(BorderSpriteManager prefab, Transform trackingTransform) {
            var border = Instantiate(prefab);
            border.trackingTransform = trackingTransform;
            border.leftBlocks = new SortedDictionary<int, GameObject>();
            border.rightBlocks = new SortedDictionary<int, GameObject>();
            border.InitializeBorders();
            border.StartCoroutine(border.UpdateEverySecond());
            return border;
        }

        private void InitializeBorders() {
            int startY = Mathf.RoundToInt(trackingTransform.position.y - BorderConst.RANGE_FROM_PLAYER);
            int endY = Mathf.RoundToInt(trackingTransform.position.y + BorderConst.RANGE_FROM_PLAYER);

            for (int y = startY; y <= endY; y++) {
                InstantiateBlock(y);
            }
        }

        private IEnumerator UpdateEverySecond() {
            while (true) {
                UpdateBorderLengths();
                yield return new WaitForSeconds(1f);
            }
        }

        private void UpdateBorderLengths() {
            int minY = Mathf.RoundToInt(trackingTransform.position.y - BorderConst.RANGE_FROM_PLAYER);
            int maxY = Mathf.RoundToInt(trackingTransform.position.y + BorderConst.RANGE_FROM_PLAYER);

            // Remove blocks below range
            var below = leftBlocks.Keys.Where(y => y < minY).ToList();
            foreach (int y in below) {
                DeleteBlock(y);
            }

            // Remove blocks above range
            var above = leftBlocks.Keys.Where(y => y > maxY).ToList();
            foreach (int y in above) {
                DeleteBlock(y);
            }

            // Add missing blocks below
            for (int y = minY; y <= maxY; y++) {
                if (!leftBlocks.ContainsKey(y)) {
                    InstantiateBlock(y);
                }
            }
        }

        private void InstantiateBlock(int y) {
            var leftBlock = Instantiate(borderSpriteObjectPrefab, new Vector3(-BorderConst.BORDER_X_POS, y, 0), Quaternion.identity, transform);
            var rightBlock = Instantiate(borderSpriteObjectPrefab, new Vector3(BorderConst.BORDER_X_POS, y, 0), Quaternion.identity, transform);
            leftBlocks[y] = leftBlock;
            rightBlocks[y] = rightBlock;
        }

        private void DeleteBlock(int y) {
            if (leftBlocks.TryGetValue(y, out var left)) {
                Destroy(left);
                leftBlocks.Remove(y);
            }

            if (rightBlocks.TryGetValue(y, out var right)) {
                Destroy(right);
                rightBlocks.Remove(y);
            }
        }
    }
}
