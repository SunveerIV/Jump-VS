using UnityEngine;

namespace Game.Utility {
    public static class GameState {
        public static void SetOrientationLock() {
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortraitUpsideDown = false;
        }
    }
}