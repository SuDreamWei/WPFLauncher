using System;
using System.Linq;
using System.Text;
using Noya.LocalServer.Common.Cryptography;
using Noya.LocalServer.Common.Extensions;
using Noya.LocalServer.Common.Utilities;

namespace Mark;

public class x19Crypt
{
	private static readonly string[] _keys;

	private static readonly string[] _keys_g79v3;

	private static readonly string[] _keys_g79v12;

	public static byte[] PickKey(byte query)
	{
		return Encoding.UTF8.GetBytes(_keys[(query >> 4) & 0xF]);
	}

	public static byte[] PickKey_g79v3(byte query)
	{
		return Encoding.UTF8.GetBytes(_keys_g79v3[(query >> 4) & 0xF]);
	}

	public static byte[] PickKey_g79v12(byte query)
	{
		return _keys_g79v12[(query >> 4) & 0xF].HexToBytes();
	}

	public static string ParseLoginResponse(byte[] body)
	{
		if (body.Length < 18)
		{
			throw new ArgumentException("Input body too short");
		}
		byte[] array = AESHelper.AES_CBC_Decrypt(PickKey(body[^1]), body.Skip(16).Take(body.Length - 17).ToArray(), body.Take(16).ToArray());
		int num = 0;
		int num2 = array.Length - 1;
		while (num < 16)
		{
			if (array[num2] != 0)
			{
				num++;
			}
			num2--;
		}
		return Encoding.UTF8.GetString(array.Take(num2 + 1).ToArray());
	}

	public static byte[] HttpEncrypt(byte[] bodyIn)
	{
		try
		{
			byte[] array = new byte[(int)Math.Ceiling((double)(bodyIn.Length + 16) / 16.0) * 16];
			Array.Copy(bodyIn, array, bodyIn.Length);
			byte[] bytes = Encoding.ASCII.GetBytes(StringExtensions.RandStringRunes(16));
			for (int i = 0; i < bytes.Length; i++)
			{
				array[i + bodyIn.Length] = bytes[i];
			}
			byte b = (byte)((new Random().Next(0, 15) << 4) | 2);
			byte[] bytes2 = Encoding.ASCII.GetBytes(StringExtensions.RandStringRunes(16));
			byte[] array2 = AESHelper.AES_CBC_Encrypt(PickKey(b), array, bytes2);
			byte[] array3 = new byte[16 + array2.Length + 1];
			Array.Copy(bytes2, 0, array3, 0, 16);
			Array.Copy(array2, 0, array3, 16, array2.Length);
			array3[^1] = b;
			return array3;
		}
		catch
		{
			return new byte[0];
		}
	}

	public static byte[] HttpEncrypt_g79v3(byte[] bodyIn)
	{
		try
		{
			byte[] array = new byte[(int)Math.Ceiling((double)(bodyIn.Length + 16) / 16.0) * 16];
			Array.Copy(bodyIn, array, bodyIn.Length);
			byte[] bytes = Encoding.ASCII.GetBytes(StringExtensions.RandStringRunes(16));
			for (int i = 0; i < bytes.Length; i++)
			{
				array[i + bodyIn.Length] = bytes[i];
			}
			byte b = (byte)((new Random().Next(0, 15) << 4) | 3);
			byte[] bytes2 = Encoding.ASCII.GetBytes(StringExtensions.RandStringRunes(16));
			byte[] array2 = AESHelper.AES_CBC_Encrypt(PickKey_g79v3(b), array, bytes2);
			byte[] array3 = new byte[16 + array2.Length + 1];
			Array.Copy(bytes2, 0, array3, 0, 16);
			Array.Copy(array2, 0, array3, 16, array2.Length);
			array3[^1] = b;
			return array3;
		}
		catch
		{
			return new byte[0];
		}
	}

	public static string ParseLoginResponse_g79v3(byte[] body)
	{
		if (body.Length < 18)
		{
			throw new ArgumentException("Input body too short");
		}
		byte[] array = AESHelper.AES_CBC_Decrypt(PickKey_g79v3(body[^1]), body.Skip(16).Take(body.Length - 17).ToArray(), body.Take(16).ToArray());
		int num = 0;
		int num2 = array.Length - 1;
		while (num < 16)
		{
			if (array[num2] != 0)
			{
				num++;
			}
			num2--;
		}
		return Encoding.UTF8.GetString(array.Take(num2 + 1).ToArray());
	}

	public static byte[] HttpEncrypt_g79v12(byte[] bodyIn)
	{
		try
		{
			byte[] array = new byte[(int)Math.Ceiling((double)(bodyIn.Length + 16) / 16.0) * 16];
			Array.Copy(bodyIn, array, bodyIn.Length);
			byte[] bytes = Encoding.ASCII.GetBytes(StringExtensions.RandStringRunes(16));
			for (int i = 0; i < bytes.Length; i++)
			{
				array[i + bodyIn.Length] = bytes[i];
			}
			byte b = (byte)((new Random().Next(0, 15) << 4) | 0xC);
			byte[] bytes2 = Encoding.ASCII.GetBytes(StringExtensions.RandStringRunes(16));
			byte[] array2 = AESHelper.AES_CBC_Encrypt(PickKey_g79v12(b), array, bytes2);
			byte[] array3 = new byte[16 + array2.Length + 1];
			Array.Copy(bytes2, 0, array3, 0, 16);
			Array.Copy(array2, 0, array3, 16, array2.Length);
			array3[^1] = b;
			return array3;
		}
		catch
		{
			return new byte[0];
		}
	}

	public static string HttpDecrypt_g79v12(byte[] body)
	{
		if (body.Length < 18)
		{
			throw new ArgumentException("Input body too short");
		}
		byte[] bytes = AESHelper.AES_CBC_Decrypt(PickKey_g79v12(body[^1]), body.Skip(16).Take(body.Length - 17).ToArray(), body.Take(16).ToArray());
		return Encoding.UTF8.GetString(bytes);
	}

	public static string ParseLoginResponse_g79v12(byte[] body)
	{
		if (body.Length < 18)
		{
			throw new ArgumentException("Input body too short");
		}
		byte[] bytes = AESHelper.AES_CBC_Decrypt(PickKey_g79v12(body[^1]), body.Skip(16).Take(body.Length - 17).ToArray(), body.Take(16).ToArray());
		return Encoding.UTF8.GetString(bytes);
	}

	public static string ComputeDynamicToken(string path, byte[] body, string token)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(HashHelper.SafeCompleteMD5Hex(Encoding.UTF8.GetBytes(token)));
		stringBuilder.Append(Encoding.UTF8.GetString(body));
		stringBuilder.Append("0eGsBkhl");
		stringBuilder.Append(path.TrimEnd(new char[1] { '?' }));
		byte[] bytes = Encoding.UTF8.GetBytes(HashHelper.SafeCompleteMD5Hex(Encoding.UTF8.GetBytes(stringBuilder.ToString())));
		string text = bytes.ToBinary();
		text = text.Substring(6) + text.Substring(0, 6);
		for (int i = 0; i < bytes.Length; i++)
		{
			string text2 = text.Substring(i * 8, 8);
			byte b = 0;
			for (int j = 0; j < 8; j++)
			{
				if (text2[7 - j] == '1')
				{
					b = (byte)(b | (1 << j));
				}
			}
			bytes[i] = (byte)(b ^ bytes[i]);
		}
		return Convert.ToBase64String(bytes).Substring(0, 16).Replace("+", "m")
			.Replace("/", "o") + "1";
	}

	public static byte[] DecryptModJson(byte[] array, byte[] key, string UUID)
	{
		if (!(Encoding.ASCII.GetString(array.Skip(4).Take(36).ToArray()) == UUID))
		{
			return null;
		}
		array = array.Skip(64).ToArray();
		if (array.Length % 16 == 0)
		{
			return AESHelper.AES_CFB_Decrypt(key, array, key);
		}
		if (array.Length < 16)
		{
			int num = 16 - array.Length;
			byte[] array2 = new byte[array.Length + num];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i];
			}
			return AESHelper.AES_CFB_Decrypt(key, array2, key).Take(array.Length).ToArray();
		}
		int num2 = 16 - array.Length % 16;
		byte[] array3 = new byte[array.Length + num2];
		for (int j = 0; j < array.Length; j++)
		{
			array3[j] = array[j];
		}
		return AESHelper.AES_CFB_Decrypt(key, array3, key).Take(array.Length).ToArray();
	}

	static x19Crypt()
	{
		_keys = new string[16]
		{
			"MK6mipwmOUedplb6", "OtEylfId6dyhrfdn", "VNbhn5mvUaQaeOo9", "bIEoQGQYjKd02U0J", "fuaJrPwaH2cfXXLP", "LEkdyiroouKQ4XN1", "jM1h27H4UROu427W", "DhReQada7gZybTDk", "ZGXfpSTYUvcdKqdY", "AZwKf7MWZrJpGR5W",
			"amuvbcHw38TcSyPU", "SI4QotspbjhyFdT0", "VP4dhjKnDGlSJtbB", "UXDZx4KhZywQ2tcn", "NIK73ZNvNqzva4kd", "WeiW7qU766Q1YQZI"
		};
		_keys_g79v3 = new string[16]
		{
			"75yWE1DMlhP6JZre", "NtDdtr7zaCO7MGqK", "5P3gbvwC2x2qVsXK", "Qgg0y2foklzV8W2P", "ItCyfnGMte15pFXe", "bp8UGVtOcS4Cc0VS", "ZRoxt2LItMBL2Rko", "EyVV2FUOWSU3pfEE", "L9molWm6kVuE6c6m", "oPDdpwvjN2YgZzE8",
			"K5rvy5Jb2S1J4SpX", "IYDhVUqFPlVjA7to", "LCR32BrjIVqkaYbS", "RWAss9Mri8bThLgF", "cdxDfuavFR1Frds5", "euKUQqtpUkUKF5aY"
		};
		_keys_g79v12 = new string[16]
		{
			"60F1E0D1FD635362430747215CF1C2FF", "EA5B62D27D0338374852C4B9469D7AC6", "17238D55501C5F020B155FB3303591E6", "8C5CEAE0F443E006A050266F73ADD5B0", "1C02CE22FB22F0E72060217418F351F3", "9A01773FEBB0CFE0EBDBF37F4D23C27F", "43F32300BF252CC320E2572ACE766367", "07F161011B3101F1ED0301735631E734", "0454E7707A5F37565601E100406060AF", "647554BAD3100C43C16660F002CC10F3",
			"E157213170F842382032564265B0B043", "914FC59311B04151393EF6896A847636", "0710C0205D224237025323265C145FA1", "054E6F01165267025C3111F562A921E9", "722D1789E792E2CA0D5322211FD0F5AE", "91F7C751FCF671F34943430772341799"
		};
	}
}
