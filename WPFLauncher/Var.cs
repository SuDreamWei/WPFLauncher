using System.IO;
using System.Reflection;
using Microsoft.Win32;
using WPFLauncher.DataTypes;

namespace WPFLauncher;

public class Var
{
	public static string WPFLauncherVersion = "1.15.17.43047";

	public static string CurrentPath = Directory.GetCurrentDirectory();

	public static Account CurrentAccount = new Account();

	public static string NeteaseRegPath = "\\SOFTWARE\\Netease\\MCLauncher";

	public static string NeteaseDownloadPath = "";

	public static string NeteaseBedrockPath = "";

	public static void InitializeNeteaseReg()
	{
		using RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(NeteaseRegPath);
		string text = registryKey.GetValue("DownloadPath")?.ToString();
		if (text == null)
		{
			text = Path.Combine(Path.GetPathRoot(Assembly.GetExecutingAssembly().Location), "MCLDownload");
			registryKey.SetValue("DownloadPath", text);
		}
		else
		{
			if (!Path.IsPathRooted(text))
			{
				text = Path.Combine(Path.GetPathRoot(Assembly.GetExecutingAssembly().Location), "MCLDownload");
				registryKey.SetValue("DownloadPath", text);
			}
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
		}
		string text2 = registryKey.GetValue("MinecraftBENeteasePath")?.ToString();
		if (text2 == null)
		{
			text2 = Path.Combine(Path.GetPathRoot(Assembly.GetExecutingAssembly().Location), "MCLDownload", "MinecraftBENetease");
			registryKey.SetValue("MinecraftBENeteasePath", text2);
		}
		else
		{
			if (!Path.IsPathRooted(text2))
			{
				text2 = Path.Combine(Path.GetPathRoot(Assembly.GetExecutingAssembly().Location), "MCLDownload", "MinecraftBENetease");
				registryKey.SetValue("MinecraftBENeteasePath", text2);
			}
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
		}
		NeteaseDownloadPath = text2;
		NeteaseDownloadPath = text;
	}
}
