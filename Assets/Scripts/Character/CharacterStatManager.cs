using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class CharacterStatManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Stamina Regeneration")]
        [SerializeField] float staminaRegenerationAmount = 2;
        private float staminaRegenerationTimer=0;
        private float staminaTickTimer = 0;
        [SerializeField] float staminaRegenerationDelay = 2;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public int CalculateStaminaBasedOnLevel(int endurance)
        {
            float stamina = 0;

            // CREATE AN EQUATION FOR HOW U WANT YOUR STAMINA TO BE CALCULATED

            stamina = endurance * 10;

            return Mathf.RoundToInt(stamina);
        }

        public virtual void RegenerateStamina()
        {
            if (!character.IsOwner)
                return;

            if (character.networkManager.isSprinting.Value)
                return;

            if (character.isPerformingAction)
                return;


            staminaRegenerationTimer += Time.deltaTime;

            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (character.networkManager.currentStamina.Value < character.networkManager.maxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;

                    if (staminaTickTimer >= 0.1f)
                    {
                        staminaTickTimer = 0;
                        character.networkManager.currentStamina.Value +=staminaRegenerationAmount;
                        character.networkManager.currentStamina.Value = 
                            Mathf.Min(character.networkManager.currentStamina.Value, character.networkManager.maxStamina.Value);
                    }
                }
            }
        }

        public virtual void ResetStaminaRegenerationTimer(float oldValue,float newValue)
        {
            if (oldValue > newValue)
            {
                staminaRegenerationTimer = 0;
            }
        }
    }
}
