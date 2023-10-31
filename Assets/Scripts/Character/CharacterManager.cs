using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

namespace ED
{
    public class CharacterManager : NetworkBehaviour
    {
        public CharacterNetworkManager networkManager;
        [HideInInspector]public Animator animator;
        [HideInInspector]public CharacterController characterController;

        [Header("Flags")]
        public bool isPerformingAction = false;
        public bool canMove = true;
        public bool canRotate = true;
        public bool applyRootMotion = false;
        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);
            characterController = GetComponent<CharacterController>();
            networkManager = GetComponent<CharacterNetworkManager>();
            animator=GetComponent<Animator>();
        }

        protected virtual void Update()
        {
            // IF THIS CHARACTER IS BEING CONTROL FROM OUR SIDE, THEN ASSIGN ITS NETWORK POSITION TO THE
            //POSITION OF OUR TRANSFORM
            if (IsOwner)
            {
                networkManager.networkPos.Value = transform.position;
                networkManager.networkRotation.Value = transform.rotation;
            }
            //IF THIS CHARACTER IS BEING CONTROLLED FROM ELSE WHERE, THEN ASSIGN ITS POSITION HERE LOCALLY
            // BY THE POSITION OF ITS NETWORK TRANSFORM
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, 
                    networkManager.networkPos.Value,
                    ref networkManager.networkPositionVelocity, 
                    networkManager.networkPositionSmoothTime);

                transform.rotation = Quaternion.Slerp(transform.rotation, networkManager.networkRotation.Value,
                    networkManager.networkRotationSmoothTime);
            }
        }

        protected virtual void LateUpdate()
        {

        }
    }
}
