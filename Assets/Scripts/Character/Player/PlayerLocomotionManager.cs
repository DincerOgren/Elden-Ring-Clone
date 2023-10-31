using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {

        PlayerManager player;

        [HideInInspector]
        public float horizontalMovement,
            verticalMovement,
            moveAmount;

        [Header("Movement Settings")]
        private Vector3 movementDirection;
        private Vector3 targetRotationDirection;
        [SerializeField]
        float walkingSpeed = 2f,
            runningSpeed = 4.5f,
            sprintingSpeed = 8f,
            rotationSpeed = 5f;


        [Header("Dodge")]
        Vector3 rollDirection;

        [Header("Stamina Cost")]
        [SerializeField] float sprintStaminaCost = 2;
        [SerializeField] float dodgeStaminaCost = 25;


        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();

            if (player.IsOwner)
            {
                player.networkManager.moveAmount.Value = moveAmount;
                player.networkManager.animatorHorizontalParameter.Value = horizontalMovement;
                player.networkManager.animatorVerticalParameter.Value = verticalMovement;
            }
            else
            {
                moveAmount = player.networkManager.moveAmount.Value;
                horizontalMovement = player.networkManager.animatorHorizontalParameter.Value;
                verticalMovement = player.networkManager.animatorVerticalParameter.Value;

                //IF NOT LOCKED ON
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount,player.networkManager.isSprinting.Value);
            }
        }

        public void HandleAllMovement()
        {
            HandleGroundenMovement();
            HandleRotation();
        }

        private void GetVerticalAndHorizontalInput()
        {
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
            verticalMovement = PlayerInputManager.instance.verticalInput;
            moveAmount = PlayerInputManager.instance.moveAmount;
            //CLAMP THE MOVEMENTS
        }
        private void HandleGroundenMovement()
        {
            if (!player.canMove) return;
            GetVerticalAndHorizontalInput();

            movementDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            movementDirection = movementDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            movementDirection.Normalize();
            movementDirection.y = 0;

            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.characterController.Move(sprintingSpeed * Time.deltaTime * movementDirection);
            }
            else
            {
                if (PlayerInputManager.instance.moveAmount > 0.5f)
                {
                    player.characterController.Move(runningSpeed * Time.deltaTime * movementDirection);
                }
                else if (PlayerInputManager.instance.moveAmount >= 0.5f)
                {
                    player.characterController.Move(walkingSpeed * Time.deltaTime * movementDirection);
                }
            }
           
        }

        private void HandleRotation()
        {
            if (!player.canRotate) { return; }

            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection = targetRotationDirection +
                PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;

            targetRotationDirection.y = 0;
            targetRotationDirection.Normalize();
            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newTurnRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newTurnRotation,
                rotationSpeed * Time.deltaTime);

            transform.rotation = targetRotation;
        }

        public void HandleSprinting()
        {
            if (player.isPerformingAction)
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }

            // if out of stamina sprinting false
            if (player.playerNetworkManager.currentStamina.Value <= 0)
            {
                player.playerNetworkManager.isSprinting.Value = false;
                return;
            }
            //if we are moving sprinting true
            if (moveAmount >= 0.5f)
            {
                player.playerNetworkManager.isSprinting.Value = true;
            }
            //if stationary sprinting false
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }

            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.playerNetworkManager.currentStamina.Value -=  sprintStaminaCost * Time.deltaTime;
            }
        }

        public void AttemtToDodge()
        {
            if(player.isPerformingAction) { return; }


            if (player.networkManager.currentStamina.Value <= 0)
            {
                return;
            }
            // IF we are moving perform a roll
            if (moveAmount > 0)
            {

                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
                rollDirection.y = 0;
                rollDirection.Normalize();

                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                transform.rotation = playerRotation;
                //Perform a roll
                player.playerAnimatorManager.PlayerTargetActionAnimation("Roll_Action", true,true);
            }
            //If we are stationary do a backstep
            else
            {
                //Perform backstep
                return;
                //IF YOU FIND ANY BACKSTEP ANIMATION YOU CAN ADD HERE EASILY
            }

            player.networkManager.currentStamina.Value -= dodgeStaminaCost;
        }


    }
}
