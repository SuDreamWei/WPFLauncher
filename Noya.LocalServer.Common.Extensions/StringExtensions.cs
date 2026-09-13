using System;
using System.Globalization;
using System.Linq;

namespace Noya.LocalServer.Common.Extensions;

public static class StringExtensions
{
	public static string RandStringRunes(int length)
	{
		Random random = new Random();
		return new string((from s in Enumerable.Repeat("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
			select s[random.Next(s.Length)]).ToArray());
	}

	public static string RandomLetter(int length)
	{
		Random random = new Random();
		return new string((from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", length)
			select s[random.Next(s.Length)]).ToArray());
	}

	public static uint SafeParseToUInt32(this string numStr)
	{
		uint.TryParse(numStr, out var result);
		return result;
	}

	public static byte[] HexToBytes(this string hex)
	{
		if (string.IsNullOrEmpty(hex))
		{
			return null;
		}
		byte[] array = new byte[hex.Length / 2];
		for (int i = 0; i < array.Length; i++)
		{
			try
			{
				array[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
			}
			catch (Exception innerException)
			{
				throw new FormatException("hex is not a valid hex number!", innerException);
			}
		}
		return array;
	}
}
