using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Constants;
using Game.Interfaces;

namespace Game.Behaviours.Directors {
    public class LineDirector : MonoBehaviour {

        [SerializeField] private LineRenderer line;
        
        private Camera mainCamera;
        private DateTime timeCreated;
        private ILaunchable launchable;
        
        public static LineDirector Create(LineDirector prefab, ILaunchable launchable) {
            LineDirector director = Instantiate(prefab, launchable.transform);
            director.launchable = launchable;
            director.mainCamera = Camera.main;
            director.timeCreated = DateTime.Now;
            director.Move();
            director.DrawTrajectory();
            return director;
        }

        public void Update() {
            Move();
            DrawTrajectory();
            Delete();
        }

        private void DrawTrajectory() {
            Vector3 startPos = launchable.transform.position;
            Vector3 startVel = (transform.position - launchable.transform.position) * Player.VELOCITY_AMPLIFIER;
            int lineLength = (int)((DateTime.Now - timeCreated).TotalMilliseconds * Director.GROW_RATE);
            lineLength = Math.Min(lineLength, Director.MAX_LENGTH);

            List<Vector3> points = new List<Vector3>(lineLength);

            for (int i = 0; i < lineLength; i++) {
                float time = i * Director.TIME_STEP;
                Vector3 linePointPos = startPos + startVel * time + 0.5f * (Vector3)Physics2D.gravity * time * time;
                points.Add(linePointPos);
            }

            List<Vector3> pointsToBeRemoved = new List<Vector3>(5);
            foreach (Vector3 point in points) {
                if (Vector3.Distance(startPos, point) >= Director.MIN_DISTANCE_FROM_PLAYER) break;
                
                pointsToBeRemoved.Add(point);
            }

            foreach (Vector3 point in pointsToBeRemoved) {
                points.Remove(point);
            }

            line.positionCount = points.Count;
            line.SetPositions(points.ToArray());
        }
        
        /// <summary>
        /// Moves the object within +/- MAX_DISTANCE around the player's position based on touch input or mouse position.
        /// </summary>
        private void Move() {
            Vector2 inputPosition;
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            inputPosition = Input.mousePosition;
#elif UNITY_IOS || UNITY_ANDROID
                if (Input.touchCount > 0) {
                    inputPosition = Input.GetTouch(0).position;
                }
                else {
                    inputPosition = Vector2.zero;
                }
#endif
            Vector2 CirclePos = launchable.transform.position;
            float inputWorldPosX = Mathf.Clamp(mainCamera.ScreenToWorldPoint(inputPosition).x,
                CirclePos.x - Director.MAX_DISTANCE, CirclePos.x + Director.MAX_DISTANCE);
            float inputWorldPosY = Mathf.Clamp(mainCamera.ScreenToWorldPoint(inputPosition).y,
                CirclePos.y - Director.MAX_DISTANCE, CirclePos.y + Director.MAX_DISTANCE);
            transform.position = new Vector2(inputWorldPosX, inputWorldPosY);
        }
        
        /// <summary>
        /// If the player releases MouseButton1 or stops touching the screen,
        /// launchable is launched and Director is destroyed.
        /// If launchable is too close to Director, launchable is not launched
        /// </summary>
        private void Delete() {
            if (!Input.GetMouseButtonUp(0)) return;
            Destroy(gameObject);
            if (Vector3.Distance(transform.position, launchable.transform.position) < Director.MIN_DISTANCE_FROM_PLAYER) return;
            launchable.Launch(transform.position);
        }
    }
}