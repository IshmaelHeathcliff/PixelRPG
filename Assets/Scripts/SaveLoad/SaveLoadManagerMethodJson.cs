using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SaveLoad
{
    public class SaveLoadManagerMethodJson: ISaveLoadManagerMethod
    {
        public void Save(object saveObject, FileStream saveFile)
        {
			// string json = JsonUtility.ToJson(saveObject, true);
			var jsonSerializerSettings = new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(saveObject, jsonSerializerSettings);
			var streamWriter = new StreamWriter(saveFile);
			streamWriter.Write(json);
			streamWriter.Close();
			saveFile.Close();

        }

        public T Load<T>(FileStream saveFile, JsonConverter converter = null)
        {
            var streamReader = new StreamReader(saveFile, Encoding.UTF8);
			string json = streamReader.ReadToEnd();
			// object savedObject = JsonUtility.FromJson(json, objectType);
			var jsonSerializerSettings = new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All,
            };
            var savedObject = converter == null ? JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings) : 
                JsonConvert.DeserializeObject<T>(json, converter);
            streamReader.Close();
			saveFile.Close();
			return savedObject;
        }
    }
}