using UnityEngine;
using System.Collections;

public class HitAND : MonoBehaviour
{
    public virtual bool AND(Collider2D collider) { return true; }
    public virtual bool IgnoreAND(Collider2D collider) { return AND(collider); }
}

public class HitAND_IgnoreContainClass<T> : HitAND
{
    public override bool AND(Collider2D collider)
    {
        return StaticAND(collider);
    }
    public static bool StaticAND(Collider2D collider)
    {
        var script = collider.GetComponent<T>();
        return script == null;
    }
}