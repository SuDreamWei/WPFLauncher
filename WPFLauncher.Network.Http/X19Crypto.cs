using System;
using System.Linq;
using System.Text;
using Mcl.Core.Extensions;
using Mcl.Core.Utils;

namespace WPFLauncher.Network.Http;

internal class X19Crypto
{
	public static string[] _keys = new string[16]
	{
		"MK6mipwmOUedplb6", "OtEylfId6dyhrfdn", "VNbhn5mvUaQaeOo9", "bIEoQGQYjKd02U0J", "fuaJrPwaH2cfXXLP", "LEkdyiroouKQ4XN1", "jM1h27H4UROu427W", "DhReQada7gZybTDk", "ZGXfpSTYUvcdKqdY", "AZwKf7MWZrJpGR5W",
		"amuvbcHw38TcSyPU", "SI4QotspbjhyFdT0", "VP4dhjKnDGlSJtbB", "UXDZx4KhZywQ2tcn", "NIK73ZNvNqzva4kd", "WeiW7qU766Q1YQZI"
	};

	public static string PickKey(byte query)
	{
		return _keys[(query >> 4) & 0xF];
	}

	public static byte[] HttpEncrypt(string url, string body, out string keyStr)
	{
		keyStr = "";
		byte[] bytes = Encoding.UTF8.GetBytes(body);
		try
		{
			byte[] array = new byte[(int)Math.Ceiling((double)(bytes.Length + 16) / 16.0) * 16];
			Array.Copy(bytes, array, bytes.Length);
			byte[] bytes2 = Encoding.ASCII.GetBytes(StringExtensions.RandStringRunes(16));
			for (int i = 0; i < bytes2.Length; i++)
			{
				array[i + bytes.Length] = bytes2[i];
			}
			byte b = (byte)((new Random().Next(0, 15) << 4) | 2);
			byte[] bytes3 = Encoding.ASCII.GetBytes(StringExtensions.RandStringRunes(16));
			keyStr = PickKey(b);
			byte[] array2 = AESHelper.AESEncrypt128Ex(array, Encoding.UTF8.GetBytes(keyStr), bytes3);
			byte[] array3 = new byte[16 + array2.Length + 1];
			Array.Copy(bytes3, 0, array3, 0, 16);
			Array.Copy(array2, 0, array3, 16, array2.Length);
			array3[^1] = b;
			return array3;
		}
		catch
		{
			return new byte[0];
		}
	}

	public static string HttpDecrypt(byte[] body, out string keyStr)
	{
		if (body == null || body.Length == 0)
		{
			keyStr = string.Empty;
			return string.Empty;
		}
		keyStr = string.Empty;
		return ParseLoginResponse(body, out keyStr);
	}

	public static string ParseLoginResponse(byte[] body, out string keyStr)
	{
		if (body == null || body.Length == 0)
		{
			keyStr = string.Empty;
			return string.Empty;
		}
		if (body.Length < 18)
		{
			throw new ArgumentException("Input body too short");
		}
		keyStr = PickKey(body[^1]);
		byte[] array = AESHelper.AESDecrypt128Ex(body.Skip(16).Take(body.Length - 17).ToArray(), Encoding.UTF8.GetBytes(keyStr), body.Take(16).ToArray());
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

	public static string ComputeDynamicToken(string urlStr, string bodyStr)
	{
		string token = Var.CurrentAccount.Token;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(HashHelper.Md5(Encoding.UTF8.GetBytes(token)));
		stringBuilder.Append(bodyStr);
		stringBuilder.Append("0eGsBkhl");
		stringBuilder.Append(urlStr.TrimEnd(new char[1] { '?' }));
		byte[] bytes = Encoding.UTF8.GetBytes(HashHelper.Md5(Encoding.UTF8.GetBytes(stringBuilder.ToString())));
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

	public static string GetH5Token()
	{
		return HashHelper.Md5(Encoding.UTF8.GetBytes(Var.CurrentAccount.Token));
	}
}
