using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatManager playerStatManager;
        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatManager = GetComponent<PlayerStatManager>();
        }

        protected override void Update()
        {
            base.Update();
            if (!IsOwner)
                return;

            playerLocomotionManager.HandleAllMovement();
            //REGEN STAMINA
            playerStatManager.RegenerateStamina();
        }

        protected override void LateUpdate()
        {
            if (!IsOwner) return;
            base.LateUpdate();

            PlayerCamera.instance.HandleAllCameraMovement();

        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;

                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged += playerStatManager.ResetStaminaRegenerationTimer;

                // THIS WILL BE moved when save

                playerNetworkManager.maxStamina.Value = playerStatManager.CalculateStaminaBasedOnLevel(playerNetworkManager.endurance.Value);
                playerNetworkManager.currentStamina.Value = playerStatManager.CalculateStaminaBasedOnLevel(playerNetworkManager.endurance.Value);
                PlayerUIManager.instance.playerUIHudManager.SetNewMaxStaminaValue(playerNetworkManager.maxStamina.Value);
            }
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData characterSaveData)
        {
            characterSaveData.characterName = playerNetworkManager.characterName.Value.ToString();
            characterSaveData.xPos = transform.position.x;
            characterSaveData.yPos = transform.position.y;
            characterSaveData.zPos = transform.position.z;
        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData characterSaveData)
        {
            playerNetworkManager.characterName.Value = characterSaveData.characterName;
            Vector3 myPos = new(characterSaveData.xPos, characterSaveData.yPos, characterSaveData.zPos);
            transform.position = myPos;
        }
    }
}