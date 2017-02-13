using UnityEngine;
using System.Collections;

public class Bro_PlayerActionManager : MonoBehaviour
{
    public Bro_PlayerColliderManager colliderManager;
    public Bro_WalkAction walkAction;
    public Bro_JumpAction jumpAction;
    

    void Update()
    {
        NormalInputUpdate();
    }
    

    void NormalInputUpdate()
    {
        Vector2 analogpad = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool hasJumpCommand = Input.GetButton("Fire1");


        if (colliderManager.IsGround)
        {
            walkAction.SetWalk(analogpad.x);

            if (hasJumpCommand)
            {
                jumpAction.SetJump();
                walkAction.ReleaseWalk();
            }
        }
        
        if(jumpAction.IsRunning)
        {
            if (hasJumpCommand) jumpAction.SetJump();
            else jumpAction.ReleaseJump();
        }
    }
    
}
