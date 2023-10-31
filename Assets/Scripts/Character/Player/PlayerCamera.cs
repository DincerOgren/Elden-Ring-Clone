using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        public Camera cameraObject;
        public PlayerManager player;
        

        [Header("Camera Settings ")]
        private float cameraSmoothSpeed = 1; // THE BIGGER THIS NUMBER, THE LONGER FOR THE CAMERA TO REACH ITS POSITION
        [SerializeField] float leftAndRightRotationSpeed = 220;
        [SerializeField] float upAndDownRotationSpeed = 220;
        [SerializeField] float minimumPivot=-30; //Lowest look rotation
        [SerializeField] float maximumPivot=60; //Highest look rotation
        [SerializeField] float cameraCollisionRadius=.2f;
        [SerializeField] LayerMask collideWithLayers;


        [Header("Camera Values")]
        private Vector3 cameraVelocity;
        private Vector3 cameraObjectPosition;
        [SerializeField] float leftAndRightLookAngle;
        [SerializeField] float upAndDownLookAngle;
        [SerializeField] float mouseSensMultiplier = .2f;
        [SerializeField] Transform cameraPivotTransform;
        private float cameraZPosition; //VAlues used by camera collision
        private float targetCameraPos;
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
            cameraZPosition = cameraObject.transform.localPosition.z;
        }


        public void HandleAllCameraMovement()
        {
            if (player == null)
            {
                return; 
            }

            //Follow the player
            FollowTarget();
            //rotate around player
            HandleCameraRotations();
            //collide with objects
            HandleCollisions();

        }

        private void FollowTarget()
        {
            Vector3 targetCamPosition = Vector3.SmoothDamp(transform.position, player.transform.position, 
                ref cameraVelocity, 
                cameraSmoothSpeed * Time.deltaTime);

            transform.position = targetCamPosition;

        }
        
        private void HandleCameraRotations()
        {
            //IF LOCKED ON, FORCE ROTATION TOWARDS TARGET
            // ELSE REGULARLY

            //NORMAL ROTATIONS






            //--------------------------------------------------
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed*mouseSensMultiplier) * Time.deltaTime;
            upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed*mouseSensMultiplier) * Time.deltaTime;
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation;
            Quaternion targetRotation;

            cameraRotation = Vector3.zero;
            cameraRotation.y = leftAndRightLookAngle;
            if (leftAndRightLookAngle >= 360f || leftAndRightLookAngle <= -360f)
            {
                leftAndRightLookAngle = 0f;
            }
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;


        }

        private void HandleCollisions()
        {
            targetCameraPos = cameraZPosition;

            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position,cameraCollisionRadius,direction,out RaycastHit hit,Mathf.Abs(targetCameraPos),collideWithLayers))
            {
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetCameraPos = -(distanceFromHitObject - cameraCollisionRadius);
            }

            if (Mathf.Abs(targetCameraPos) < cameraCollisionRadius)
            {
                targetCameraPos = -cameraCollisionRadius; 
            }

            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraPos, 0.2f);
            cameraObject.transform.localPosition = cameraObjectPosition; 
        }

    }
}