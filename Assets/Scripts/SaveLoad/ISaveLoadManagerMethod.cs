using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace SaveLoad
{
    public interface ISaveLoadManagerMethod
    {
        void Save(object saveObject, FileStream saveFile);

        T Load<T>(FileStream saveFile, JsonConverter converter = null);
    }
    
	public enum SaveLoadManagerMethods { Json, JsonEncrypted };

	public abstract class SaveLoadManagerEncryptor
	{
		public string Key { get; set; } = "yourDefaultKey";

        protected string SaltText = "SaltTextGoesHere";

        protected virtual void Encrypt(Stream inputStream, Stream outputStream, string sKey)
		{
			var algorithm = new AesManaged();
            var key = new Rfc2898DeriveBytes(sKey, Encoding.ASCII.GetBytes(SaltText), 1000, HashAlgorithmName.SHA256);

			algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
			algorithm.IV = key.GetBytes(algorithm.BlockSize / 8);

			var cryptoStream = new CryptoStream(inputStream, algorithm.CreateEncryptor(), CryptoStreamMode.Read);
			cryptoStream.CopyTo(outputStream);
		}

		protected virtual void Decrypt(Stream inputStream, Stream outputStream, string sKey)
		{
			var algorithm = new AesManaged();
			var key = new Rfc2898DeriveBytes(sKey, Encoding.ASCII.GetBytes(SaltText));

			algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
			algorithm.IV = key.GetBytes(algorithm.BlockSize / 8);

			var cryptoStream = new CryptoStream(inputStream, algorithm.CreateDecryptor(), CryptoStreamMode.Read);
			cryptoStream.CopyTo(outputStream);
		}
	}
}
