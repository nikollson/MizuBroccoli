using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Library
{
    public class HitObjectDetecter2D
    {
        public List<GameObject> GetHitObject2D(Vector2 screenPosition)
        {
            Camera c = Camera.main;
            Vector2 sp = screenPosition;
            if (c == null) Debug.Log("haha2");
            if (sp == null) Debug.Log("uoifwoi");
            Vector2 worldPoint = c.ScreenToWorldPoint(sp);
            RaycastHit2D[] hitResult = Physics2D.RaycastAll(worldPoint, Vector2.zero);

            var ret = new List<GameObject>();

            foreach (var a in hitResult)
            {
                ret.Add(a.collider.gameObject);
            }

            return ret;
        }

        public List<T> GetHitObject2D<T>(Vector2 screenPosition) where T : MonoBehaviour
        {
            var objects = GetHitObject2D(screenPosition);
            var ret = new List<T>();

            foreach (var a in objects)
            {
                T script = a.GetComponent<T>();
                if (script != null) ret.Add(script);
            }
            return ret;
        }
    }
}