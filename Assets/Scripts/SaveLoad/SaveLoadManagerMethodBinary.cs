using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SaveLoad
{
    // BinaryFormatter有安全风险，不推荐使用
    public class SaveLoadManagerMethodBinary : ISaveLoadManagerMethod
    {
        public void Save(object saveObject, FileStream saveFile)
        {
			var formatter = new BinaryFormatter();
			formatter.Serialize(saveFile, saveObject);
			saveFile.Close();
        }

        public object Load(Type objectType, FileStream saveFile)
        {
            var formatter = new BinaryFormatter();
			object savedObject = formatter.Deserialize(saveFile);
			saveFile.Close();
			return savedObject;
        }
    }
}