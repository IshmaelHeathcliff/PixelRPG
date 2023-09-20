using UnityEngine;

namespace SaveLoad
{
    public class SaveLoadManagerMethod : MonoBehaviour
    {
		[Header("Save and load method")]

		[Tooltip("the method to use to save to file")]
		public SaveLoadManagerMethods SaveLoadMethod = SaveLoadManagerMethods.Binary;
		/// the key to use to encrypt the file (if using an encryption method)
		[Tooltip("the key to use to encrypt the file (if using an encryption method)")]
		public string EncryptionKey = "ThisIsTheKey";

		protected ISaveLoadManagerMethod _saveLoadManagerMethod;

		/// <summary>
		/// On Awake, we set the MMSaveLoadManager's method to the chosen one
		/// </summary>
		protected virtual void Awake()
		{
			SetSaveLoadMethod();
		}
		
		/// <summary>
		/// Creates a new MMSaveLoadManagerMethod and passes it to the MMSaveLoadManager
		/// </summary>
		public virtual void SetSaveLoadMethod()
		{
			switch(SaveLoadMethod)
			{
				case SaveLoadManagerMethods.Binary:
					_saveLoadManagerMethod = new SaveLoadManagerMethodBinary();
					break;
				case SaveLoadManagerMethods.BinaryEncrypted:
					_saveLoadManagerMethod = new SaveLoadManagerMethodBinaryEncrypted();
					((SaveLoadManagerEncryptor)_saveLoadManagerMethod).Key = EncryptionKey;
					break;
				case SaveLoadManagerMethods.Json:
					_saveLoadManagerMethod = new SaveLoadManagerMethodJson();
					break;
				case SaveLoadManagerMethods.JsonEncrypted:
					_saveLoadManagerMethod = new SaveLoadManagerMethodJsonEncrypted();
					((SaveLoadManagerEncryptor)_saveLoadManagerMethod).Key = EncryptionKey;
					break;
			}
			SaveLoadManager.SaveLoadMethod = _saveLoadManagerMethod;
		}
        
    }
}