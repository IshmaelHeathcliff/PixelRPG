using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
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

        public T Load<T>(FileStream saveFile, JsonConverter converter = null)
        {
            T savedObject;
			using (var memoryStream = new MemoryStream())
				using (var streamReader = new StreamReader(memoryStream))
				{
					try
					{
						Decrypt(saveFile, memoryStream, Key);
					}
					catch (CryptographicException ce)
					{
						Debug.LogError("[SaveLoadManager] Encryption key error: " + ce.Message);
						return default;
					}
					memoryStream.Position = 0;
                    string json = streamReader.ReadToEnd();
                    savedObject = converter == null ? JsonConvert.DeserializeObject<T>(json) : 
                                        JsonConvert.DeserializeObject<T>(json, converter);
				}
			saveFile.Close();
			return savedObject;

        }
    }
}