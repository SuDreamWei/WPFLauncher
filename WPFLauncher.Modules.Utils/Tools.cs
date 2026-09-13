using System;
using Newtonsoft.Json;

namespace WPFLauncher.Modules.Utils;

public class Tools
{
	public static long GetCurrentTimestamp()
	{
		return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}

	public static bool IsVaildJson(string text)
	{
		try
		{
			JsonConvert.DeserializeObject(text);
			return true;
		}
		catch (Exception ex)
		{
			Function.AddLog("无法解析Json: " + ex.Message + "\nstacktrace:" + ex.StackTrace);
			return false;
		}
	}
}
