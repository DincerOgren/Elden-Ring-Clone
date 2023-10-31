using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class PlayerAnimatorManager : CharacterAnimationManager
    {
        PlayerManager player;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        private void OnAnimatorMove()
        {
            if(player.applyRootMotion)
            {
                Vector3 speed = player.animator.deltaPosition;
                player.characterController.Move(speed);
                player.transform.rotation *= player.animator.deltaRotation;
            }
        }
    }
}
