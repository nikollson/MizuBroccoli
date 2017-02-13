using UnityEngine;
using System.Collections;

[System.Serializable]
public class Bro_ActionFrictionModule
{
    public Vector2 frictionRate = new Vector2(50, 50);

    public Bro_ActionFrictionModule() { }
    public Bro_ActionFrictionModule(Vector2 frictionRate)
    {
        this.frictionRate = frictionRate;
    }

    public Vector2 GetFriction(Vector2 velocity)
    {
        return new Vector2(-velocity.x * frictionRate.x, -velocity.y * frictionRate.y);
    }
}
