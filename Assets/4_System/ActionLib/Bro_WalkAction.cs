using UnityEngine;
using System.Collections;

public class Bro_WalkAction : MonoBehaviour
{
    
    public bool IsRunning { get; private set; }

    public new Rigidbody2D rigidbody;
    public Bro_ActionFrictionModule moveFriction = new Bro_ActionFrictionModule(new Vector2(50, 0));
    public Bro_ActionFrictionModule stopFriction = new Bro_ActionFrictionModule(new Vector2(100, 0));


    public float walkPowerMax = 1000.0f;

    const float EPS = 0.00001f;
    float par;

    void Update()
    {
        if(IsRunning)
        {
            Vector2 friction = moveFriction.GetFriction(rigidbody.velocity);
            if (Mathf.Abs(par) < EPS) friction = stopFriction.GetFriction(rigidbody.velocity);

            Vector2 power = friction + new Vector2(walkPowerMax * par, 0);
            rigidbody.AddForce(Time.deltaTime * power);
        }
    }
    
    public void SetWalk(float par)
    {
        this.par = par;
        if(!IsRunning)
        {
            IsRunning = true;
        }
    }
    
    public void ReleaseWalk()
    {
        IsRunning = false;
    }
}
