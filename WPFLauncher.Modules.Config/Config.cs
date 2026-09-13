using System.IO;
using WPFLauncher.Modules.Utils;

namespace WPFLauncher.Modules.Config;

public class Config
{
	public static class ConfigName
	{
		public static string Account = "accounts.json";
	}

	public static string ConfigDirectory = Path.Combine(Var.CurrentPath, "Config");

	public static string ReadConfig(string configName)
	{
		if (!Directory.Exists(ConfigDirectory))
		{
			Directory.CreateDirectory(ConfigDirectory);
			return "";
		}
		string path = Path.Combine(ConfigDirectory, configName);
		if (!File.Exists(path))
		{
			return "";
		}
		return File.ReadAllText(path);
	}

	public static void ExceptionConfig(string configName, string reason = "")
	{
		string text = configName + ".json_" + Tools.GetCurrentTimestamp() + ".bak";
		File.Copy(Path.Combine(ConfigDirectory, configName), Path.Combine(ConfigDirectory, text));
		File.Delete(Path.Combine(ConfigDirectory, "settings.json"));
		Function.ShowDialog("我们在处理你的json文件时发生错误(我们已将原设置备份并且重新初始化了设置配置,文件名:" + text + " in config folder):Reason: \n" + reason, "错误");
	}
}
