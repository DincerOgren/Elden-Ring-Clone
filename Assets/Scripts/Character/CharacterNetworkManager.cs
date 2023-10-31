using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


namespace ED
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        CharacterManager character;

        [Header("Position")]
        public NetworkVariable<Vector3> networkPos = new NetworkVariable<Vector3>(Vector3.zero, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Owner);

        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity ,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
       

        public Vector3 networkPositionVelocity;

        public float networkPositionSmoothTime = 0.1f;
        public float networkRotationSmoothTime = 0.1f;

        [Header("Animatior")]
        public NetworkVariable<float> animatorHorizontalParameter=new NetworkVariable<float>(0, 
            NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

        public NetworkVariable<float> animatorVerticalParameter = new NetworkVariable<float>(0, 
            NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

        public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        [Header("Flags")]
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        [Header("Stats")]
        public NetworkVariable<int> endurance = new NetworkVariable<int>(1,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        // SERVERRPC IS A FUNCTION CALLED FROM A CLIENT, TO THE SERVER
        [ServerRpc]
        public void NotifyAllClientsServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            if (IsServer)
            {
                PlayActionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
            }

        }

        //CLIENTRPC IS SENT TO ALL CLIENTS PRESENT, FROM THE SERVER
        [ClientRpc]
        public void PlayActionAnimationForAllClientsClientRpc(ulong clientID,string animationID,bool applyRootMotion)
        {
            if (clientID!=NetworkManager.Singleton.LocalClientId)
            {
                PerformActionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        public void PerformActionAnimationFromServer(string animationID,bool applyRootMotion)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);
        }
    }
}