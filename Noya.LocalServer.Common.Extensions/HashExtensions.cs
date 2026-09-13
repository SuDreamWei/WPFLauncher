using System.IO;
using System.Security.Cryptography;

namespace Noya.LocalServer.Common.Extensions;

public static class HashExtensions
{
	public static byte[] CompleteMD5FromFile(this MD5 md5, string filePath)
	{
		using FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		return md5.ComputeHash(inputStream);
	}
}
