using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Library
{
    public enum SmartPhoneTouchMode { None, Tap, Flick, LongTouch };
    public class SmartPhoneTouchClassifier
    {
        public Result Classify(List<Vector2> positions, float deltaTime, bool isEnd, List<SmartPhoneTouchReciever> hitObjects)
        {
            var detecter = new Detecter();
            var selector = new RecieverSelector();

            // collect data

            Detecter.RESULT tapResult = detecter.IsTap(positions, deltaTime, isEnd);
            Detecter.RESULT flickResult = detecter.IsFlick(positions, deltaTime, isEnd);
            Detecter.RESULT longTouchResult = detecter.IsLongTouch(positions, deltaTime, isEnd);

            int tapIndex = selector.GetRecieverIndex(hitObjects, SmartPhoneTouchMode.Tap);
            int flickIndex = selector.GetRecieverIndex(hitObjects, SmartPhoneTouchMode.Flick);
            int longTouchIndex = selector.GetRecieverIndex(hitObjects, SmartPhoneTouchMode.LongTouch);

            if (tapIndex == -1) tapResult = Detecter.RESULT.NO;
            if (flickIndex == -1) flickResult = Detecter.RESULT.NO;
            if (longTouchIndex == -1) longTouchResult = Detecter.RESULT.NO;


            // classify
            if (longTouchResult == Detecter.RESULT.YES) return new Result(longTouchIndex, SmartPhoneTouchMode.LongTouch);
            if (flickResult == Detecter.RESULT.YES) return new Result(flickIndex, SmartPhoneTouchMode.Flick);
            if (tapResult == Detecter.RESULT.YES) return new Result(tapIndex, SmartPhoneTouchMode.Tap);

            if (longTouchResult == Detecter.RESULT.UNkNOWN)
            {
                if (tapResult == Detecter.RESULT.NO && flickResult == Detecter.RESULT.NO)
                {
                    return new Result(longTouchIndex, SmartPhoneTouchMode.LongTouch);
                }
            }

            return new Result(-1, SmartPhoneTouchMode.None);
        }

        // data class
        public class Result
        {
            public int inedex;
            public SmartPhoneTouchMode touchMode;
            public Result(int index, SmartPhoneTouchMode touchMode)
            {
                this.inedex = index;
                this.touchMode = touchMode;
            }
        }


        // implement class
        class RecieverSelector
        {
            private bool checkState(SmartPhoneTouchReciever reciever, SmartPhoneTouchMode mode)
            {
                if (mode == SmartPhoneTouchMode.Tap) return reciever.checkTap;
                if (mode == SmartPhoneTouchMode.Flick) return reciever.checkFlick;
                if (mode == SmartPhoneTouchMode.LongTouch) return reciever.checkLongTouch;
                return false;
            }
            private bool IsFront(SmartPhoneTouchReciever target, SmartPhoneTouchReciever opponent)
            {
                return target.gameObject.transform.position.z < opponent.gameObject.transform.position.z;
            }
            public int GetRecieverIndex(List<SmartPhoneTouchReciever> hitObjects, SmartPhoneTouchMode mode)
            {
                int index = -1;
                for (int i = 0; i < hitObjects.Count; i++)
                {
                    if (checkState(hitObjects[i], mode))
                    {
                        if (index == -1 || IsFront(hitObjects[i], hitObjects[index]))
                        {
                            index = i;
                        }
                    }
                }
                return index;
            }
        }


        // implement class
        class Detecter
        {
            public enum RESULT { NO, UNkNOWN, YES };

            public RESULT IsTap(List<Vector2> positions, float deltaTime, bool isEnd)
            {
                var detecter = new TapDetecter();
                return detecter.Detect(positions, deltaTime, isEnd);
            }
            public RESULT IsFlick(List<Vector2> positions, float deltaTime, bool isEnd)
            {
                var detecter = new FlickDetecter();
                return detecter.Detect(positions, deltaTime, isEnd);
            }
            public RESULT IsLongTouch(List<Vector2> positions, float deltaTime, bool isEnd)
            {
                var detecter = new LongTouchDetecter();
                return detecter.Detect(positions, deltaTime, isEnd);
            }

            class TapDetecter
            {
                private const float maxTime = 0.4f;
                private const float maxSpeed = 20f;
                public RESULT Detect(List<Vector2> positions, float deltaTime, bool isEnd)
                {
                    if(deltaTime >= maxTime)return RESULT.NO;

                    if (isEnd)
                    {
                        float speed = DetectUtility.GetSpeed(positions, deltaTime);
                        if (speed > maxSpeed) return RESULT.NO;
                        else return RESULT.YES;
                    }

                    return RESULT.UNkNOWN;
                }
            }
            class FlickDetecter
            {
                private const float maxTime = 0.4f;
                private const float minSpeed = 20f;
                public RESULT Detect(List<Vector2> positions, float deltaTime, bool isEnd)
                {
                    if (deltaTime >= maxTime) return RESULT.NO;

                    if (isEnd)
                    {
                        float speed = DetectUtility.GetSpeed(positions, deltaTime);
                        if (speed < minSpeed) return RESULT.NO;
                        else return RESULT.YES;
                    }
                    return RESULT.UNkNOWN;
                }
            }
            class LongTouchDetecter
            {
                private const float decideTime = 1.0f;
                public RESULT Detect(List<Vector2> positions, float deltaTime, bool isEnd)
                {
                    if (deltaTime >= decideTime) return RESULT.YES;
                    return RESULT.UNkNOWN;
                }
            }

            class DetectUtility
            {
                public static float GetSpeed(List<Vector2> positions, float deltaTime)
                {
                    float sum = 0;
                    for (int i = 1; i < positions.Count; i++) sum += (positions[i] - positions[i - 1]).sqrMagnitude;
                    return sum / deltaTime;
                }
            }
        }
    }
}