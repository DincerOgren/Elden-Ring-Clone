using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED {
    [System.Serializable]
    public class CharacterSaveData
    {
        [Header("Character Name")]
        public string characterName = "";

        [Header("Time Played")]
        public float secondsPlayed;

        [Header("Character World Position")]
        public float xPos;
        public float yPos;
        public float zPos;
    }
}
