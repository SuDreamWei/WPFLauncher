using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Noya.LocalServer.Common.Cryptography;

public class AESHelper
{
	public static byte[] AES_CBC_Decrypt(byte[] key, byte[] data, byte[] iv)
	{
		using Aes aes = Aes.Create();
		aes.KeySize = key.Length * 8;
		aes.BlockSize = 128;
		aes.Key = key;
		aes.IV = iv;
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.None;
		using ICryptoTransform cryptoTransform = aes.CreateDecryptor();
		byte[] array = new byte[data.Length];
		cryptoTransform.TransformBlock(data, 0, data.Length, array, 0);
		return array;
	}

	public static byte[] AES_CBC_Encrypt(byte[] key, byte[] data, byte[] iv)
	{
		using Aes aes = Aes.Create();
		aes.KeySize = key.Length * 8;
		aes.BlockSize = 128;
		aes.Key = key;
		aes.IV = iv;
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.None;
		using ICryptoTransform cryptoTransform = aes.CreateEncryptor();
		byte[] array = new byte[data.Length];
		cryptoTransform.TransformBlock(data, 0, data.Length, array, 0);
		return array;
	}

	public static byte[] AES_CBC256_Encrypt(byte[] key, byte[] toEncrypt, byte[] iv)
	{
		int num = 16 - toEncrypt.Length % 16;
		if (num == 16)
		{
			num = 0;
		}
		int num2;
		if (toEncrypt.Length >= 16)
		{
			num2 = toEncrypt.Length / 16;
			if (num != 0)
			{
				num2++;
			}
		}
		else
		{
			num2 = 1;
		}
		byte[] array = new byte[num2 * 16];
		Array.Copy(toEncrypt, array, toEncrypt.Length);
		for (int i = 0; i < num; i++)
		{
			array[i + toEncrypt.Length] = (byte)num;
		}
		return new RijndaelManaged
		{
			Key = key,
			IV = iv,
			Mode = CipherMode.CBC,
			Padding = PaddingMode.None
		}.CreateEncryptor().TransformFinalBlock(array, 0, array.Length);
	}

	public static ICryptoTransform getCipherInstance(byte[] Key, bool encrypt = true)
	{
		if (Key.Length < 16)
		{
			Array.Resize(ref Key, 16);
		}
		else if (Key.Length < 24)
		{
			Array.Resize(ref Key, 24);
		}
		else if (Key.Length < 32)
		{
			Array.Resize(ref Key, 32);
		}
		else
		{
			Array.Resize(ref Key, 32);
		}
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.Key = Key;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		rijndaelManaged.BlockSize = 128;
		if (encrypt)
		{
			return rijndaelManaged.CreateEncryptor();
		}
		return rijndaelManaged.CreateDecryptor();
	}

	public static byte[] encrypt(byte[] key, byte[] source)
	{
		return getCipherInstance(key).TransformFinalBlock(source, 0, source.Length);
	}

	public static byte[] decrypt(byte[] key, byte[] source)
	{
		return getCipherInstance(key, encrypt: false).TransformFinalBlock(source, 0, source.Length);
	}

	public static byte[] AES_CFB_Decrypt(byte[] key, byte[] data, byte[] iv)
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream(data);
			using Aes aes = Aes.Create();
			aes.Key = key;
			aes.IV = iv;
			aes.Mode = CipherMode.CFB;
			aes.Padding = PaddingMode.Zeros;
			CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
			try
			{
				byte[] array = new byte[data.Length + 32];
				int num = cryptoStream.Read(array, 0, data.Length + 32);
				byte[] array2 = new byte[num];
				Array.Copy(array, 0, array2, 0, num);
				return array2;
			}
			finally
			{
				cryptoStream.Close();
				memoryStream.Close();
				aes.Clear();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return null;
		}
	}

	public static byte[] AES_ECB_Encrypt(byte[] key, byte[] data)
	{
		using Aes aes = Aes.Create();
		aes.KeySize = key.Length * 8;
		aes.BlockSize = 128;
		aes.Key = key;
		aes.Mode = CipherMode.ECB;
		aes.Padding = PaddingMode.None;
		using ICryptoTransform cryptoTransform = aes.CreateEncryptor();
		byte[] array = new byte[data.Length];
		cryptoTransform.TransformBlock(data, 0, data.Length, array, 0);
		return array;
	}

	public static string BytesToHex(byte[] bytes)
	{
		string text = "";
		if (bytes != null)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				text += bytes[i].ToString("X2");
			}
		}
		return text;
	}

	public static byte[] HexToBytes(string hex)
	{
		return (from x in Enumerable.Range(0, hex.Length)
			where x % 2 == 0
			select Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
	}

	public static byte[] GetIv(int n)
	{
		char[] array = new char[60]
		{
			'a', 'b', 'd', 'c', 'e', 'f', 'g', 'h', 'i', 'j',
			'k', 'l', 'm', 'n', 'p', 'r', 'q', 's', 't', 'u',
			'v', 'w', 'z', 'y', 'x', '0', '1', '2', '3', '4',
			'5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E',
			'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'Q',
			'P', 'R', 'T', 'S', 'V', 'U', 'W', 'X', 'Y', 'Z'
		};
		StringBuilder stringBuilder = new StringBuilder();
		Random random = new Random(DateTime.Now.Millisecond);
		for (int i = 0; i < n; i++)
		{
			stringBuilder.Append(array[random.Next(0, array.Length)].ToString());
		}
		return Encoding.UTF8.GetBytes(stringBuilder.ToString());
	}

	public static byte[] GetDefaultIv()
	{
		return Encoding.UTF8.GetBytes("1234567890123456");
	}

	public static byte[] AESEncryptECB128(byte[] data, byte[] keyBytes, byte[] ivBytes)
	{
		return new RijndaelManaged
		{
			Mode = CipherMode.ECB,
			Padding = PaddingMode.PKCS7,
			KeySize = 128,
			BlockSize = 128,
			Key = keyBytes,
			IV = ivBytes
		}.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
	}

	public static byte[] AESDecryptECB128(byte[] data, byte[] keyBytes, byte[] ivBytes)
	{
		return new RijndaelManaged
		{
			Mode = CipherMode.ECB,
			Padding = PaddingMode.PKCS7,
			KeySize = 128,
			BlockSize = 128,
			Key = keyBytes,
			IV = ivBytes
		}.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
	}

	public static byte[] AESEncrypt128Ex(byte[] data, byte[] keyBytes, byte[] ivBytes)
	{
		return new RijndaelManaged
		{
			Mode = CipherMode.CBC,
			Padding = PaddingMode.Zeros,
			KeySize = 128,
			BlockSize = 128,
			Key = keyBytes,
			IV = ivBytes
		}.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
	}

	public static byte[] AESDecrypt128Ex(byte[] data, byte[] keyBytes, byte[] ivBytes)
	{
		return new RijndaelManaged
		{
			Mode = CipherMode.CBC,
			Padding = PaddingMode.Zeros,
			KeySize = 128,
			BlockSize = 128,
			Key = keyBytes,
			IV = ivBytes
		}.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
	}
}
