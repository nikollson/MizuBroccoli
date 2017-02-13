using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Library
{
    public class SmartPhoneTouchData
    {
        private int fingerId;
        private float deltaTime;
        private List<Vector2> worldPositions;
        private List<Vector2> screenPositions;
        private SmartPhoneTouchMode touchMode;

        public int FingerID { get { return fingerId; } }
        public float DeltaTime { get { return deltaTime; } }
        public List<Vector2> Postions { get { return worldPositions; } }
        public List<Vector2> ScreenPositions { get { return screenPositions; } }
        public SmartPhoneTouchMode TouchMode { get { return touchMode; } }

        public Vector2 Position { get { return worldPositions[worldPositions.Count - 1]; } }
        public Vector2 StartPosition { get { return worldPositions[0]; } }

        public Vector2 ScreenPosition { get { return screenPositions[screenPositions.Count - 1]; } }
        public Vector2 StartScreenPosition { get { return screenPositions[0]; } }


        public SmartPhoneTouchData(int fingerId, float deltaTime, List<Vector2> screenPositions, List<Vector2> worldPositions, SmartPhoneTouchMode touchMode)
        {
            this.fingerId = fingerId;
            this.deltaTime = deltaTime;
            this.touchMode = touchMode;

            this.screenPositions = new List<Vector2>();
            this.screenPositions.AddRange(screenPositions);

            this.worldPositions = new List<Vector2>();
            this.worldPositions.AddRange(worldPositions);
        }
    }
}
