using ED;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEditor.Rendering;

namespace ED
{
    public class CharacterAnimationManager : MonoBehaviour
    {
        public float animationDampTime = 0.1f;
        CharacterManager character;

        int horizontal;
        int vertical;
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
            horizontal = Animator.StringToHash("Horizontal");
            vertical = Animator.StringToHash("Vertical");
        }
        public void UpdateAnimatorMovementParameters(float horizontal, float vertical,bool isSprinting)
        {
            float verticalAmount = vertical;
            float horizontalAmount = horizontal;

            if (character.networkManager.isSprinting.Value)
            {
                verticalAmount = 2;
            }

            character.animator.SetFloat(this.horizontal, horizontalAmount, animationDampTime, Time.deltaTime);
            character.animator.SetFloat(this.vertical, verticalAmount, animationDampTime, Time.deltaTime);
        }

        public void PlayerTargetActionAnimation(string actionName, bool isPerformingActionFlag, bool applyRootMotion = true,
            bool canMove=false, bool canRotate = false)
        {

            character.applyRootMotion=applyRootMotion;
            character.animator.CrossFade(actionName, 0.2f);
            //CAN BE USED TO STOP CHARACTER FROM ATTEMTING NEW ACTIONS 
            //FOR EXAMPLE, IF YOU GET DAMAGED AND BEGIN PERFORMING A DAMAGE ANIMATION
            //THIS FLAG WILL TURN TRUE IF YOU ARE STUNNED 
            //WE CAN THEN CHECK FOR THIS BEFORE ATTEMPTING NEW ACTION
            character.isPerformingAction = isPerformingActionFlag;
            character.canMove = canMove;
            character.canRotate = canRotate;

            character.networkManager.NotifyAllClientsServerRpc(NetworkManager.Singleton.LocalClientId, actionName, applyRootMotion);
        }
    }
}
