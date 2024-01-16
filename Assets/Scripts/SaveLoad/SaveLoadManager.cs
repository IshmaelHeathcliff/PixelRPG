using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveLoad
{
    public static class SaveLoadManager
    {
        public static ISaveLoadManagerMethod SaveLoadMethod { get; set; } = new SaveLoadManagerMethodJson();

        const string BaseFolderName = "/Data/";
        const string DefaultFolderName = "Save";

		static string DetermineSavePath(string folderName = DefaultFolderName)
		{
			string savePath;
			if (Application.platform == RuntimePlatform.IPhonePlayer) 
			{
				savePath = Application.persistentDataPath + BaseFolderName;
			} 
			else 
			{
				savePath = Application.persistentDataPath + BaseFolderName;
			}
            
			#if UNITY_EDITOR
			savePath = Application.dataPath + BaseFolderName;
			#endif

			savePath = savePath + folderName + "/";
			return savePath;
		}

		static string DetermineSaveFileName(string fileName)
		{
			return fileName;
		}

		public static void Save(object saveObject, string fileName, string folderName = DefaultFolderName)
		{
			string savePath = DetermineSavePath(folderName);
			string saveFileName = DetermineSaveFileName(fileName);
            
			if (!Directory.Exists(savePath))
			{
				Directory.CreateDirectory(savePath);
			}

			var saveFile = File.Create(savePath + saveFileName);

			SaveLoadMethod.Save(saveObject, saveFile);
			saveFile.Close();
		}


		public static T Load<T>(string fileName, string folderName = DefaultFolderName, JsonConverter converter = null)
		{
			string savePath = DetermineSavePath(folderName);
			string saveFileName = savePath + DetermineSaveFileName(fileName);

            if (!Directory.Exists(savePath) || !File.Exists(saveFileName))
			{
				return default;
			}

			var saveFile = File.Open(saveFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			var returnObject = SaveLoadMethod.Load<T>(saveFile, converter);
			saveFile.Close();

			return returnObject;
		}


		public static void DeleteSave(string fileName, string folderName = DefaultFolderName)
		{
			string savePath = DetermineSavePath(folderName);
			string saveFileName = DetermineSaveFileName(fileName);
			if (File.Exists(savePath + saveFileName))
			{
				File.Delete(savePath + saveFileName);
			}	
			if (File.Exists(savePath + saveFileName + ".meta"))
			{
				File.Delete(savePath + saveFileName + ".meta");
			}			
		}

		public static void DeleteSaveFolder(string folderName = DefaultFolderName)
		{
			string savePath = DetermineSavePath(folderName);
			if (Directory.Exists(savePath))
			{
				DeleteDirectory(savePath);
			}
		}
		
		public static void DeleteAllSaveFiles()
		{
			string savePath = DetermineSavePath("");

			savePath = savePath.Substring(0, savePath.Length - 1);
			if (savePath.EndsWith("/"))
			{
				savePath = savePath.Substring(0, savePath.Length - 1);
			}

			if (Directory.Exists(savePath))
			{
				DeleteDirectory(savePath);
			}
		}

		public static void DeleteDirectory(string targetDir)
		{
			string[] files = Directory.GetFiles(targetDir);
			string[] dirs = Directory.GetDirectories(targetDir);

			foreach (string file in files)
			{
				File.SetAttributes(file, FileAttributes.Normal);
				File.Delete(file);
			}

			foreach (string dir in dirs)
			{
				DeleteDirectory(dir);
			}

			Directory.Delete(targetDir, false);

			if (File.Exists(targetDir + ".meta"))
			{
				File.Delete(targetDir + ".meta");
			}
		}
	}
}
