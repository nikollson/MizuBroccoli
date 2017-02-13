using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitManager : MonoBehaviour
{
    public HitManager[] child;

    List<Data> dict = new List<Data>();

    HitAND[] detectAnd;
    HitOR[] detectOr;

    new Collider2D[] colliders;

    virtual public bool IsHit
    {
        get
        {
            RemoveEndData();
            bool res = DoDetect ? dict.Count >= 1 : false;
            if (child != null) foreach (var a in child) if (a != null) res |= a.IsHit;
            // res |= (dict.Count >= 1 && dict[0].collider != null);
            return res;
        }
        /* get
         {
             RemoveEndData();
             bool res = DoDetect ? dict.Count >= 1 : false;
             if (child != null) foreach (var a in child) if (a != null) res |= a.IsHit;
             // res |= (dict.Count >= 1 && dict[0].collider != null);
             onceHit |= res;
             onceHitEver |= res;
             if (mode == Mode.OnceHitEver) return onceHitEver;
             if (mode == Mode.OnceHitRespawn) return onceHit;
             return res;
         }*/
    }
    public List<Data> CollisionData { get { return dict; } }
    public Collider2D HitCollider { get { return IsHit ? dict[0].collider : null; } }
    [HideInInspector] public bool DoDetect = true;
    public List<Collider2D> HitColliders
    {
        get
        {
            var ret = new List<Collider2D>();
            foreach (var a in dict) ret.Add(a.collider);
            return ret;
        }
    }
    public int UpdatedFrame { get; private set; }

    int timeStamp = -1;

    

    void Start()
    {
        detectAnd = this.GetComponents<HitAND>();
        detectOr = this.GetComponents<HitOR>();
        colliders = this.GetComponents<Collider2D>();
    }

    void Stay(Data data)
    {
        UpdatedFrame = Time.frameCount;
        bool hit = false;
        foreach (var a in dict)
        {
            if (a.collider == data.collider)
            {
                a.Update();
                hit = true;
            }
        }
        if (!hit) { dict.Add(data); }
        TimeStamp();
    }
    void Exit(Data data)
    {
        for (int i = 0; i < dict.Count; i++)
        {
            if (dict[i].collider == data.collider) { 
                
                dict.RemoveAt(i); i--;
            }
        }
        TimeStamp();
    }

    void LateUpdate()
    {
        RemoveEndData(true);
    }

    void RemoveEndData(bool debug=false)
    {
        bool isEraseMode = true;
        if (colliders != null)
        {
            foreach (var a in colliders)
            {
                if (a.enabled == true) isEraseMode = false;
            }
        }
        for (int i = 0; i < dict.Count; i++)
        {
            
            if (dict[i].IsEnd(timeStamp) || isEraseMode) { dict.RemoveAt(i); i--; }
        }
    }

    //enter -> exitが1フレームで呼ばれるとき対策
    public void OnTriggerEnter2D(Collider2D other) { OnTriggerStay2D(other); }
    public void OnCollisionEnter2D(Collision2D other) { OnCollisionStay2D(other); }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (IsIgnore(other)) { AddIgnore(other); return; }
        if (IsCurrentIgrnore(other)) { return; }
        //float t1 = Time.realtimeSinceStartup;
        Vector2 centerPoint = GetCenterPoint(other);
        //float v1 = Time.realtimeSinceStartup - t1;
        //float t2 = Time.realtimeSinceStartup;
        Data data = new Data(other, centerPoint, false);
        Stay(data);
        //float v2 = Time.realtimeSinceStartup - t2;
        //if (this.name == "水エリア") Debug.Log(this.name + " " + Time.frameCount + " " + other.name + " " + other.gameObject.name+" "+v1.ToString("0.000000")+" "+v2.ToString("0.000000"));
    }
    public void OnCollisionStay2D(Collision2D other)
    {
        if (IsIgnore(other.collider)) { AddIgnore(other.collider); return; }
        Data data = new Data(other.collider, GetContactPoint(other), true);
        Stay(data);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        Exit(new Data(other, GetCenterPoint(other), false));
    }
    void OnCollisionExit2D(Collision2D other)
    {
        Exit(new Data(other.collider, GetContactPoint(other), true));
    }

    void TimeStamp()
    {
        timeStamp = Time.frameCount;
    }

    Vector2 GetVelocity(Collider2D collider)
    {
        var rigidbody = collider.attachedRigidbody;// .GetComponent<Rigidbody2D>();
        return (rigidbody != null) ? rigidbody.velocity : Vector2.zero;
    }

    Vector2 GetCenterPoint(Collider2D other)
    {
        Vector2 c = GetColliderCenter(other);
        if(colliders!= null) foreach (var a in colliders)
        {
            if (a.enabled == true) return (c + GetColliderCenter(a)) / 2;
        }
        return c;
    }

    Vector2 GetColliderCenter(Collider2D collider)
    {
        if (collider == null || collider.gameObject == null) return Vector2.zero;
        return (Vector2)(collider.gameObject.transform.position) + collider.offset;
    }

    Vector2 GetContactPoint(Collision2D other)
    {
        Vector2 ret = Vector2.zero;
        foreach (var a in other.contacts)
        {
            bool isHit = false;
            foreach (var b in colliders)
            {
                if (a.otherCollider == b) return a.point;
            }
        }
        return ret;
    }

    public bool IsCurrentIgrnore(Collider2D collider)
    {
        bool andOK = true;
        if(detectAnd!= null) foreach (var a in detectAnd) andOK &= a.AND(collider);

        bool orOK = (detectOr == null || detectOr.Length == 0) ? true : false;
        if (detectOr != null) foreach (var a in detectOr) orOK |= a.OR(collider);

        return !andOK || !orOK;
    }

    public bool IsIgnore(Collider2D collider)
    {
        bool andOK = true;
        if (detectAnd != null) foreach (var a in detectAnd) andOK &= a.IgnoreAND(collider);

        bool orOK = (detectOr==null || detectOr.Length == 0) ? true : false;
        if (detectOr != null) foreach (var a in detectOr) orOK |= a.IgnoreOR(collider);

        return !andOK || !orOK;
    }

    public void AddIgnore(Collider2D collider)
    {
        foreach (var a in colliders)
        {
            Physics2D.IgnoreCollision(a, collider);
        }
    }

    // data class

    public class Data
    {
        public Collider2D collider;
        public Vector2 hitPosition;
        public Vector2 hitVelocity;
        public int lastStayFrame;
        public bool isCollision;
        public Data(Collider2D collider, Vector2 hitPosition, bool isCollision)
        {
            this.collider = collider;
            this.hitPosition = hitPosition;
            this.isCollision = isCollision;
            lastStayFrame = Time.frameCount;
        }
        public void Update() { lastStayFrame = Time.frameCount; }
        public bool IsEnd(int stampFrame)
        {
            bool isEnd = false;
            isEnd |= lastStayFrame != stampFrame;
            isEnd |= collider == null;
            if (collider != null)
            {
                isEnd |= collider.gameObject == null || collider.gameObject.activeSelf == false;
                isEnd |= collider.enabled == false;
            }
            return isEnd;
        }
    }
}
