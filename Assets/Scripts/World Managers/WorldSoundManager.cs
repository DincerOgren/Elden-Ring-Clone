using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class WorldSoundManager : MonoBehaviour
    {
        public static WorldSoundManager instance;

        public AudioClip rollSfx;

        private void Awake()
        {
            if (instance == null )
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
        }
    }
}
