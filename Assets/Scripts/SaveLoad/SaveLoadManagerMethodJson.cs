using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace SaveLoad
{
    public class SaveLoadManagerMethodJson: ISaveLoadManagerMethod
    {
        public void Save(object saveObject, FileStream saveFile)
        {
			string json = JsonUtility.ToJson(saveObject);
			var streamWriter = new StreamWriter(saveFile);
			streamWriter.Write(json);
			streamWriter.Close();
			saveFile.Close();

        }

        public object Load(Type objectType, FileStream saveFile)
        {
            var streamReader = new StreamReader(saveFile, Encoding.UTF8);
			string json = streamReader.ReadToEnd();
			object savedObject = JsonUtility.FromJson(json, objectType);
            streamReader.Close();
			saveFile.Close();
			return savedObject;
        }
    }
}