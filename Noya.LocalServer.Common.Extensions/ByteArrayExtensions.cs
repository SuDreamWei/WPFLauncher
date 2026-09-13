using System;
using System.Text;

namespace Noya.LocalServer.Common.Extensions;

public static class ByteArrayExtensions
{
	public static string ToBinary(this byte[] buffer)
	{
		StringBuilder stringBuilder = new StringBuilder(buffer.Length * 8);
		for (int i = 0; i < buffer.Length; i++)
		{
			string text = Convert.ToString(buffer[i], 2);
			for (int j = 0; j < 8 - text.Length; j++)
			{
				stringBuilder.Append('0');
			}
			stringBuilder.Append(text);
		}
		return stringBuilder.ToString();
	}
}
