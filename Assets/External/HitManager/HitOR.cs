using UnityEngine;
using System.Collections;

public class HitOR : MonoBehaviour {

    virtual public bool OR(Collider2D collider) { return true; }
    virtual public bool IgnoreOR(Collider2D collider) { return OR(collider); }
}
