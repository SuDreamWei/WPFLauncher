using System.IO;

namespace Login.NetEase.Utils;

public static class FileHelper
{
	public static byte[] FileToByte(string fileUrl)
	{
		try
		{
			using FileStream fileStream = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
			return array;
		}
		catch
		{
			return null;
		}
	}

	public static bool ByteToFile(byte[] byteArray, string fileName)
	{
		bool result = false;
		try
		{
			using FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
			fileStream.Write(byteArray, 0, byteArray.Length);
			result = true;
		}
		catch
		{
			result = false;
		}
		return result;
	}
}
