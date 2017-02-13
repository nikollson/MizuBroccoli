using UnityEngine;
using System.Collections;

public class Bro_JumpAction : MonoBehaviour {

    public bool IsRunning { get; private set; }

    public new Rigidbody2D rigidbody;

    public Vector2 startPower = new Vector2(0, 10);
    public Vector2 continuePower = new Vector2(0, 300);

    public Bro_ActionFrictionModule airFriction = new Bro_ActionFrictionModule(new Vector2(10, 10));

    void Update()
    {
        if(IsRunning)
        {
            Vector2 friction = airFriction.GetFriction(rigidbody.velocity);
            Vector2 power = continuePower + friction;
            rigidbody.AddForce(Time.deltaTime * power);
        }
    }

    public void SetJump()
    {
        if(!IsRunning)
        {
            Vector2 power = startPower;
            rigidbody.AddForce(power, ForceMode2D.Impulse);
        }
        IsRunning = true;
    }

    public void ReleaseJump()
    {
        IsRunning = false;
    }
}
