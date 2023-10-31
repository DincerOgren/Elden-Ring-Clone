using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ED
{
    [System.Serializable]
    public class SaveFileDataWriter
    {
        public string saveDirectoryPath = "";
        public string saveFileName = "";

        public bool CheckIfFileExsist()
        {
            return File.Exists(Path.Combine(saveDirectoryPath, saveFileName,saveFileName));
        }

        public void DeleteSaveFile()
        {
            File.Delete(Path.Combine(saveDirectoryPath, saveFileName));
        }

        // USE THE CREATE SAVE FILE
        public void CreateNewCharacterSaveFile(CharacterSaveData saveData)
        {
            string saveFileDirPath = Path.Combine(saveDirectoryPath, saveFileName);
            string savePath = Path.Combine(saveFileDirPath, saveFileName);
            try
            {
                //?
                if (!Directory.Exists(saveFileDirPath))
                {
                    Directory.CreateDirectory(saveFileDirPath);
                }

                Debug.Log("Creating the save file + " + savePath);



                //SERIALAZIE THE C# DATA TO JSON

                string dataStore = JsonUtility.ToJson(saveData, true);


                //WRITE THE FILE IN OUR SYSTEM
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(stream))
                    {
                        fileWriter.Write(dataStore);
                    }
                }


            }
            catch (Exception ex)
            {
                Debug.Log("ERROR WHILE TRYING TO SAVE CHARACTER DATA, GAME NOT SAVED, " + savePath + "\n" + ex);
                throw;
            }
        }

        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData characterData = null;

            string saveFileDirPath = Path.Combine(saveDirectoryPath, saveFileName);
            string loadPath = Path.Combine(saveFileDirPath, saveFileName);

            if (File.Exists(loadPath))
            {

                try
                {
                    string dataToLoad = "";

                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // DESERIALIZE FROM JSON TO UNITY FILE
                    characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);

                }
                catch (Exception ex)
                {
                    Debug.Log("ERROR WHILE TRYING TO LOAD CHARACTER DATA FROM SAVE FILE," + loadPath + "\n" + ex);
                    throw;
                }


            }

            return characterData;
        }
    }
}
