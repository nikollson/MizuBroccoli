using UnityEngine;
using System.Collections;
using System;

namespace Library
{
    public class SmartPhoneTouchReciever : MonoBehaviour
    {
        public bool checkTap = true;
        public bool checkFlick = true;
        public bool checkLongTouch = true;

        public Action<SmartPhoneTouchData> TapEvent;
        public Action<SmartPhoneTouchData> FlickEvent;
        public Action<SmartPhoneTouchData> LongTouchStartEvent;
        public Action<SmartPhoneTouchData> LongTouchEndEvent;
        public Action<SmartPhoneTouchData> LongTouchUpdateEvent;


        public void CallTap(SmartPhoneTouchData data)
        {
            DoAction(TapEvent, data);
        }

        public void CallFlick(SmartPhoneTouchData data)
        {
            DoAction(FlickEvent, data);
        }
        public void CallLongTouchStart(SmartPhoneTouchData data)
        {
            DoAction(LongTouchStartEvent, data);
        }
        public void CallLongTouchEnd(SmartPhoneTouchData data)
        {
            DoAction(LongTouchEndEvent, data);
        }
        public void CallLongTouchUpdate(SmartPhoneTouchData data)
        {
            DoAction(LongTouchUpdateEvent, data);
        }


        
        private void DoAction(Action<SmartPhoneTouchData> function, SmartPhoneTouchData data)
        {
            if (function != null) function(data);
        }
    }
}
