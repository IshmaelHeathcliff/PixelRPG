using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SaveLoad
{
    public interface ISaveLoadManagerMethod
    {
        public void Save(object saveObject, FileStream saveFile);

        public object Load(Type objectType, FileStream saveFile);
    }
    
	public enum SaveLoadManagerMethods { Json, JsonEncrypted, Binary, BinaryEncrypted };

	public abstract class SaveLoadManagerEncryptor
	{
		public string Key { get; set; } = "yourDefaultKey";

        protected string saltText = "SaltTextGoesHere";

        protected virtual void Encrypt(Stream inputStream, Stream outputStream, string sKey)
		{
			var algorithm = new AesManaged();
			var key = new Rfc2898DeriveBytes(sKey, Encoding.ASCII.GetBytes(saltText), 1000, HashAlgorithmName.MD5);

			algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
			algorithm.IV = key.GetBytes(algorithm.BlockSize / 8);

			var cryptoStream = new CryptoStream(inputStream, algorithm.CreateEncryptor(), CryptoStreamMode.Read);
			cryptoStream.CopyTo(outputStream);
		}

		protected virtual void Decrypt(Stream inputStream, Stream outputStream, string sKey)
		{
			var algorithm = new AesManaged();
			var key = new Rfc2898DeriveBytes(sKey, Encoding.ASCII.GetBytes(saltText));

			algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
			algorithm.IV = key.GetBytes(algorithm.BlockSize / 8);

			var cryptoStream = new CryptoStream(inputStream, algorithm.CreateDecryptor(), CryptoStreamMode.Read);
			cryptoStream.CopyTo(outputStream);
		}
	}
}
