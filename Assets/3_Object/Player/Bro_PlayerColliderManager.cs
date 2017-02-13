using UnityEngine;
using System.Collections;

public class Bro_PlayerColliderManager : MonoBehaviour {

    public HitManager footCollider;

    public bool IsGround { get { return footCollider.IsHit; } }
    
}
