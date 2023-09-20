using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;

namespace SaveLoad
{
    // BinaryFormatter有安全风险，不推荐使用
    public class SaveLoadManagerMethodBinaryEncrypted : SaveLoadManagerEncryptor, ISaveLoadManagerMethod
    {
        public void Save(object saveObject, FileStream saveFile)
        {
			var formatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, saveObject);
			    memoryStream.Position = 0;
			    Encrypt(memoryStream, saveFile, Key);
			    saveFile.Flush();
            }
			saveFile.Close();

        }

        public object Load(Type objectType, FileStream saveFile)
        {
            var formatter = new BinaryFormatter();
            object savedObject;
            
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    Decrypt(saveFile, memoryStream, Key);
                }
                catch (CryptographicException ce)
                {
                    Debug.LogError("Encryption key error: " + ce.Message);
                    return null;
                }

                memoryStream.Position = 0;
                savedObject = formatter.Deserialize(memoryStream);
            }

            saveFile.Close();
			return savedObject;
        }
    }
}