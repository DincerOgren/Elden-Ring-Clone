using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class CharacterSoundFXManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayRollSoundFX()
        {
            audioSource.PlayOneShot(WorldSoundManager.instance.rollSfx);
        }

    }
}
