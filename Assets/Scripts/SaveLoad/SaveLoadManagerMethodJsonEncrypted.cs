using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace SaveLoad
{
    public class SaveLoadManagerMethodJsonEncrypted : SaveLoadManagerEncryptor, ISaveLoadManagerMethod
    {
        public void Save(object saveObject, FileStream saveFile)
        {
			string json = JsonUtility.ToJson(saveObject);
			using (var memoryStream = new MemoryStream())
				using (var streamWriter = new StreamWriter(memoryStream))
				{
					streamWriter.Write(json);
					streamWriter.Flush();
					memoryStream.Position = 0;
					Encrypt(memoryStream, saveFile, Key);
				}
			saveFile.Close();
        }

        public object Load(Type objectType, FileStream saveFile)
        {
			object savedObject = null;
			using (var memoryStream = new MemoryStream())
				using (var streamReader = new StreamReader(memoryStream))
				{
					try
					{
						Decrypt(saveFile, memoryStream, Key);
					}
					catch (CryptographicException ce)
					{
						Debug.LogError("[MMSaveLoadManager] Encryption key error: " + ce.Message);
						return null;
					}
					memoryStream.Position = 0;
					savedObject = JsonUtility.FromJson(streamReader.ReadToEnd(), objectType);
				}
			saveFile.Close();
			return savedObject;

        }
    }
}