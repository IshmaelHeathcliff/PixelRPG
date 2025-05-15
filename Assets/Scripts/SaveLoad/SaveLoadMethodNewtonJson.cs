using System;
using System.IO;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;

using UnityEngine;
using Tool;

namespace SaveLoad
{
    public class SaveLoadMethodNewtonJson : ISaveLoadMethod
    {
        JsonSerializerSettings _settings;

        public SaveLoadMethodNewtonJson()
        {
            _settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };

            _settings.Converters.Add(new Vector2IntConverter());
            TypeDescriptor.AddAttributes(typeof(Vector2Int), new TypeConverterAttribute(typeof(Vector2IntTypeConverter)));
        }

        public void Save(object saveObject, FileStream saveFile)
        {
            string json = JsonConvert.SerializeObject(saveObject, _settings);
            var streamWriter = new StreamWriter(saveFile);
            streamWriter.Write(json);
            streamWriter.Close();
            saveFile.Close();

        }

        public T Load<T>(FileStream saveFile)
        {
            var streamReader = new StreamReader(saveFile, Encoding.UTF8);
            string json = streamReader.ReadToEnd();


            T savedObject = JsonConvert.DeserializeObject<T>(json, _settings);
            streamReader.Close();
            saveFile.Close();
            return savedObject;
        }
    }

    public class Vector2IntConverter : JsonConverter<Vector2Int>
    {
        public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
        {
            // 将 Vector2Int 转换为 "(x,y)" 格式的字符串
            writer.WriteValue(value.ToString());
        }
        public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return Vector2IntExtension.Parse(reader.Value.ToString());
        }
    }
}
