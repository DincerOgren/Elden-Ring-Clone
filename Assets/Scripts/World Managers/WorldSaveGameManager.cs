using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ED
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        [SerializeField] PlayerManager player;
        public static WorldSaveGameManager instance;

        [Header("Save/Load")]
        [SerializeField] bool saveGame, loadGame;


        [Header("World Scene Index")]
        [SerializeField] int worldSceneIndex = 1;

        [Header("Save File Writer")]
        [SerializeField] SaveFileDataWriter saveFileWriter;

        [Header("Current Character Data")]
        public CharacterSlot currentCharacterSlotBeingUsed;
        public CharacterSaveData currentCharacterData;
        public string saveFileName;

        [Header("Character Slots")]
        public CharacterSaveData characterSlot01;
        public CharacterSaveData characterSlot02;
        public CharacterSaveData characterSlot03;
        public CharacterSaveData characterSlot04;
        public CharacterSaveData characterSlot05;
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
            LoadAllCharacterProfiles();
        }

        private void Update()
        {
            if (saveGame)
            {
                saveGame = false;
                SaveGame();
            }

            if (loadGame)
            {
                loadGame = false;
                LoadGame();
            }
        }
        public string DecideWhichFileNameBasedOnCharacterSlodBeingUsed(CharacterSlot slot)
        {
            string fileName = "";
            switch (slot)
            {
                case CharacterSlot.CharacterSlot_01:
                    fileName = "CharacterSlot_01";
                    break;
                case CharacterSlot.CharacterSlot_02:
                    fileName = "CharacterSlot_02";

                    break;
                case CharacterSlot.CharacterSlot_03:
                    fileName = "CharacterSlot_03";

                    break;
                case CharacterSlot.CharacterSlot_04:
                    fileName = "CharacterSlot_04";

                    break;
                case CharacterSlot.CharacterSlot_05:
                    fileName = "CharacterSlot_05";

                    break;
                default:
                    break;
            }

            return fileName;
        }

        public void CreateNewGame()
        {
            //CREATE A NEW FILE, BASED ON CHARACTER SLOT BEING USED
            saveFileName = DecideWhichFileNameBasedOnCharacterSlodBeingUsed(currentCharacterSlotBeingUsed);

            currentCharacterData=new CharacterSaveData();

        }

        public void LoadGame()
        {
            //LOAD A NEW FILE, BASED ON CHARACTER SLOT BEING USED
            saveFileName = DecideWhichFileNameBasedOnCharacterSlodBeingUsed(currentCharacterSlotBeingUsed);

            saveFileWriter = new SaveFileDataWriter();
            //GENERALLY WORKS ON EVERY MACHINE TYPE
            saveFileWriter.saveDirectoryPath = Application.persistentDataPath;
            saveFileWriter.saveFileName = saveFileName;

            currentCharacterData = saveFileWriter.LoadSaveFile();

            StartCoroutine(LoadWorldScene());
        }

        public void SaveGame()
        {
            //SAVE THE CURRENT FILE, BASED ON CHARACTER SLOT BEING USED
            saveFileName = DecideWhichFileNameBasedOnCharacterSlodBeingUsed(currentCharacterSlotBeingUsed);
            saveFileWriter = new SaveFileDataWriter();

            saveFileWriter.saveDirectoryPath = Application.persistentDataPath;
            saveFileWriter.saveFileName = saveFileName;

            //PASS THE PLAYER INFO, FROM GAME TO SAVE FILE
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            saveFileWriter.CreateNewCharacterSaveFile(currentCharacterData);
        }

        //LOAD ALL GAME PROFILES WHEN START
        private void LoadAllCharacterProfiles()
        {
            saveFileWriter = new SaveFileDataWriter();
            saveFileWriter.saveDirectoryPath = Application.persistentDataPath;

            saveFileWriter.saveFileName = DecideWhichFileNameBasedOnCharacterSlodBeingUsed(CharacterSlot.CharacterSlot_01);
            characterSlot01 = saveFileWriter.LoadSaveFile();

            saveFileWriter.saveFileName = DecideWhichFileNameBasedOnCharacterSlodBeingUsed(CharacterSlot.CharacterSlot_02);
            characterSlot02 = saveFileWriter.LoadSaveFile();

            saveFileWriter.saveFileName = DecideWhichFileNameBasedOnCharacterSlodBeingUsed(CharacterSlot.CharacterSlot_03);
            characterSlot03 = saveFileWriter.LoadSaveFile();

            saveFileWriter.saveFileName = DecideWhichFileNameBasedOnCharacterSlodBeingUsed(CharacterSlot.CharacterSlot_04);
            characterSlot04 = saveFileWriter.LoadSaveFile();

            saveFileWriter.saveFileName = DecideWhichFileNameBasedOnCharacterSlodBeingUsed(CharacterSlot.CharacterSlot_05);
            characterSlot05 = saveFileWriter.LoadSaveFile();
        }

        public IEnumerator LoadWorldScene()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);
            yield return null;
        }
        
        public int GetWorldSceneIndex()
        {
            return worldSceneIndex;
        }
    }
}
