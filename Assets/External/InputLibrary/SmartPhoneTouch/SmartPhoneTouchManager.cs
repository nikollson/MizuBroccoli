using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

namespace Library
{
    public class SmartPhoneTouchManager : SingletonMonoBehaviour<SmartPhoneTouchManager>
    {
        SimpleTouchGetter simpleTouchGetter = new SimpleTouchGetter();
        ConnectedInputGetter connectedInputGetter = new ConnectedInputGetter();
        DataSender dataSender = new DataSender();
        void Update()
        {
            simpleTouchGetter.Update();
            connectedInputGetter.DoConnect(simpleTouchGetter.GetData());
            dataSender.DoSend(connectedInputGetter.GetData());
        }




        // implement class
        class SimpleTouchGetter
        {
            TouchInputGetter touchInputGetter = new TouchInputGetter();
            MouseInputGetter mouseInputGetter = new MouseInputGetter();

            public List<SimpleTouchData> GetData()
            {
                var ret = new List<SimpleTouchData>();


                // uGUIとの衝突回避(Buttonを押したときに当たり判定が貫通してしまう)
                bool uGUISelected = (EventSystem.current != null) && (EventSystem.current.currentSelectedGameObject != null);

                if (!uGUISelected)
                {
                    ret.AddRange(touchInputGetter.GetData());
                    ret.AddRange(mouseInputGetter.GetData());
                }
                return ret;
            }

            public void Update()
            {
                mouseInputGetter.Update();
            }


            public class SimpleTouchData
            {
                public int fingerId;
                public float deltaTime;
                public Vector2 position;

                public SimpleTouchData(int fingerId, float deltaTime, Vector2 position)
                {
                    this.fingerId = fingerId;
                    this.deltaTime = deltaTime;
                    this.position = position;
                }
            }
            private class MouseInputGetter
            {
                float prevTime = 0.0f;
                bool prevHitLeft = false;
                bool prevHitRight = false;
                SimpleTouchData nextDataLeft;
                SimpleTouchData nextDataRight;

                public List<SimpleTouchData> GetData()
                {
                    var ret = new List<SimpleTouchData>();
                    if (nextDataLeft != null) ret.Add(nextDataLeft);
                    if (nextDataRight != null) ret.Add(nextDataRight);
                    return ret;
                }
                public void Update()
                {
                    UpdateNextData();
                }

                void UpdateNextData()
                {
                    nextDataLeft = null;
                    nextDataRight = null;
                    if (HasMouseInputLeft())
                    {
                        float deltaTime = 0;
                        if (prevHitLeft) deltaTime = Time.time - prevTime;
                        if (!prevHitLeft) deltaTime = Time.deltaTime;

                        nextDataLeft = new SimpleTouchData(-1, deltaTime, Input.mousePosition);
                    }
                    if (HasMouseInputRight())
                    {
                        float deltaTime = 0;
                        if (prevHitRight) deltaTime = Time.time - prevTime;
                        if (!prevHitRight) deltaTime = Time.deltaTime;

                        nextDataRight = new SimpleTouchData(-2, deltaTime, Input.mousePosition);
                    }
                }
                bool HasMouseInputLeft()
                {
                    return Input.touchCount == 0 && Input.GetMouseButton(0);
                }
                bool HasMouseInputRight()
                {
                    return Input.touchCount == 0 && Input.GetMouseButton(1);
                }
            }
            private class TouchInputGetter
            {
                public List<SimpleTouchData> GetData()
                {
                    var ret = new List<SimpleTouchData>();
                    foreach (var a in Input.touches)
                    {
                        ret.Add(new SimpleTouchData(a.fingerId, a.deltaTime, a.position));
                    }
                    return ret;
                }
            }
        }


        // implement class
        class ConnectedInputGetter
        {
            List<ConnectedInputData> connectedInputData = new List<ConnectedInputData>();

            public void DoConnect(List<SimpleTouchGetter.SimpleTouchData> simpleTouchData)
            {
                // refresh
                ClearEndData();

                // update
                DoConnectData(simpleTouchData);
                SetEndData();

                // classify ( after update )
                ClassifyData();
            }
            public List<ConnectedInputData> GetData()
            {
                return connectedInputData;
            }


            void DoConnectData(List<SimpleTouchGetter.SimpleTouchData> simpleTouchData)
            {
                foreach (var data in simpleTouchData)
                {
                    var index = GetSameTouchIndex(data);
                    if (index == -1)
                    {
                        AddNewTouchData(data);
                        index = connectedInputData.Count - 1;
                    }
                    UpdateTouchData(connectedInputData[index], data);
                }
            }
            void SetEndData()
            {
                foreach (var a in connectedInputData)
                {
                    if (a.lastUpdateTime != Time.time) a.SetEnd();
                }
            }
            void ClassifyData()
            {
                foreach (var a in connectedInputData) a.Classify();
            }
            void ClearEndData()
            {
                for (int i = 0; i < connectedInputData.Count; i++)
                {
                    var data = connectedInputData[i];
                    if (data.IsEnd())
                    {
                        connectedInputData.RemoveAt(i);
                        i--;
                    }
                }
            }
            void AddNewTouchData(SimpleTouchGetter.SimpleTouchData data)
            {
                connectedInputData.Add(new ConnectedInputData(data, Time.time, GetHitObjects(data.position)));
            }
            void UpdateTouchData(ConnectedInputData baseData, SimpleTouchGetter.SimpleTouchData data)
            {
                baseData.Update(data, Time.time);
            }
            int GetSameTouchIndex(SimpleTouchGetter.SimpleTouchData data)
            {
                for (int i = 0; i < connectedInputData.Count; i++)
                {
                    if (connectedInputData[i].fingerId == data.fingerId) return i;
                }
                return -1;
            }
            List<SmartPhoneTouchReciever> GetHitObjects(Vector2 position)
            {
                var hitDetecter = new HitObjectDetecter2D();
                return hitDetecter.GetHitObject2D<SmartPhoneTouchReciever>(position);
            }

            public class ConnectedInputData
            {
                public int fingerId;
                public float deltaTime = 0, lastUpdateTime;
                public List<Vector2> screenPositions;
                public List<Vector2> worldPositions;
                public SmartPhoneTouchMode touchMode;
                public SmartPhoneTouchReciever reciever;

                public List<SmartPhoneTouchReciever> hitObjects;
                bool isEnd = false;
                bool isStartNow = false;

                public ConnectedInputData(SimpleTouchGetter.SimpleTouchData data, float time, List<SmartPhoneTouchReciever> hitObjects)
                {
                    screenPositions = new List<Vector2>();
                    worldPositions = new List<Vector2>();
                    fingerId = data.fingerId;
                    this.hitObjects = hitObjects;

                    this.Update(data, time);
                }

                public bool IsEnd()
                {
                    return isEnd;
                }
                public bool IsStartNow()
                {
                    return isStartNow;
                }
                public void SetEnd()
                {
                    isEnd = true;
                    isStartNow = false;
                }

                public void Update(SimpleTouchGetter.SimpleTouchData data, float time)
                {
                    if (this.lastUpdateTime == time) return;

                    this.deltaTime += data.deltaTime;
                    this.screenPositions.Add(data.position);
                    this.worldPositions.Add(GetWorldPoint(data.position));
                    this.lastUpdateTime = time;
                    this.isStartNow = false;
                }

                Vector2 GetWorldPoint(Vector2 positoin)
                {
                    Vector3 camdata = Camera.main.WorldToScreenPoint(new Vector3(0, 0, 0));

                    Vector3 data = new Vector3(positoin.x, positoin.y, camdata.z);
                    return Camera.main.ScreenToWorldPoint(data);
                }

                public void Classify()
                {
                    var classifier = new SmartPhoneTouchClassifier();
                    if (touchMode == SmartPhoneTouchMode.None)
                    {
                        var result = classifier.Classify(screenPositions, deltaTime, isEnd, hitObjects);
                        if (result.inedex != -1)
                        {
                            this.touchMode = result.touchMode;
                            this.reciever = hitObjects[result.inedex];
                            this.isStartNow = true;
                        }
                    }
                }
            }
        }


        // implement class
        class DataSender
        {
            public void DoSend(List<ConnectedInputGetter.ConnectedInputData> connectedInputData)
            {
                foreach (var a in connectedInputData)
                {
                    var argData = ConvertData(a);

                    if (a.touchMode == SmartPhoneTouchMode.Tap)
                    {
                        a.reciever.CallTap(argData);
                    }
                    if (a.touchMode == SmartPhoneTouchMode.Flick)
                    {
                        a.reciever.CallFlick(argData);
                    }
                    if (a.touchMode == SmartPhoneTouchMode.LongTouch)
                    {
                        if (a.IsStartNow()) a.reciever.CallLongTouchStart(argData);
                        a.reciever.CallLongTouchUpdate(argData);
                        if (a.IsEnd()) a.reciever.CallLongTouchEnd(argData);
                    }
                }
            }

            private SmartPhoneTouchData ConvertData(ConnectedInputGetter.ConnectedInputData data)
            {
                var ret = new SmartPhoneTouchData(data.fingerId, data.deltaTime,
                    data.screenPositions, data.worldPositions, data.touchMode);
                return ret;
            }
        }
    }
}