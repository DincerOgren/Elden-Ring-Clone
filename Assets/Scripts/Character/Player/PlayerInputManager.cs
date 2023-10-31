using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ED
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        public PlayerManager player;
        PlayerControls playerControl;

        [Header("Movement Input")]
        [SerializeField] Vector2 movementInput;
        public float verticalInput,
            horizontalInput;

        [Header("Camera Input")]
        [SerializeField] Vector2 cameraInput;
        public float cameraHorizontalInput,
            cameraVerticalInput;
            
        public float moveAmount;


        [Header("Playe Action Inputs")]
        [SerializeField] bool dodgeInput = false;
        [SerializeField] bool sprintInput = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += OnSceneChanged;

            instance.enabled = false;
        }
        private void Update()
        {
            HandleMovementInputs();
            HandleCameraInput();
            HandleDodgeInput();
            HandleSprintInput();
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            //Enable player inputs on loading world
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
            }
            else
            {
                instance.enabled = false;
            }
        }
        private void OnEnable()
        {
            if (playerControl == null)
            {
                playerControl=new PlayerControls();
                playerControl.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControl.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
                playerControl.PlayerActions.Dodge.performed += i => dodgeInput =true;



                playerControl.PlayerActions.Sprint.performed += i => sprintInput = true;
                playerControl.PlayerActions.Sprint.canceled += i => sprintInput = false;
            }
            playerControl.Enable();
        }

        private void OnDestroy()
        {

            SceneManager.activeSceneChanged -= OnSceneChanged;
        }
        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControl.Enable();
                }
                else
                    playerControl.Disable();
            }
        }
        private void HandleMovementInputs()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;
            
            moveAmount=Mathf.Clamp01(Mathf.Abs(verticalInput)+Mathf.Abs(horizontalInput));


            if (moveAmount <= 0.5f && moveAmount > 0)
            {
                moveAmount = .5f;
            }
            else if (moveAmount > 0.5f && moveAmount <= 1)
            {
                moveAmount = 1;
            }

            if (player == null) return;
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount,player.networkManager.isSprinting.Value);
        }

        private void HandleCameraInput()
        {
            cameraHorizontalInput = cameraInput.x;
            cameraVerticalInput= cameraInput.y; 
        }

        private void HandleDodgeInput()
        {
            if (dodgeInput)
            {
                dodgeInput = false;
                //Perform a dodge
                player.playerLocomotionManager.AttemtToDodge();
            }

        }

        private void HandleSprintInput() 
        {
            if (sprintInput)
            {

                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }

    }
}