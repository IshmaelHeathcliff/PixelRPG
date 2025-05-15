using System.IO;
using System.Text;
using UnityEngine;

namespace SaveLoad
{
    public class SaveLoadMethodJsonUtility : ISaveLoadMethod
    {
        public void Save(object saveObject, FileStream saveFile)
        {
            string json = JsonUtility.ToJson(saveObject, true);
            var streamWriter = new StreamWriter(saveFile);
            streamWriter.Write(json);
            streamWriter.Close();
            saveFile.Close();

        }

        public T Load<T>(FileStream saveFile)
        {
            var streamReader = new StreamReader(saveFile, Encoding.UTF8);
            string json = streamReader.ReadToEnd();
            T savedObject = JsonUtility.FromJson<T>(json);
            streamReader.Close();
            saveFile.Close();
            return savedObject;
        }
    }
}
