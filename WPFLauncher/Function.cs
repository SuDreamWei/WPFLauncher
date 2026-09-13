using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Login.NetEase.Utils;
using Mark;
using WPFLauncher.Pages;
using iNKORE.UI.WPF.Modern.Controls;

namespace WPFLauncher;

public class Function
{
	public static void ShowDialog(string content, string title = "信息")
	{
		ContentDialog contentDialog = new ContentDialog();
		contentDialog.Title = title;
		contentDialog.Content = content;
		contentDialog.CloseButtonText = "确定";
		contentDialog.ShowAsync();
	}

	public static void AddLog(string message)
	{
		string value = (new StackTrace().GetFrame(1)?.GetMethod())?.Name ?? "Unknown";
		string text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} {value}] {message}";
		if (LogsPage.ViewModel == null)
		{
			LogsPage.InitializeViewModel();
		}
		LogsPage.ViewModel.Logs += ((LogsPage.ViewModel.Logs.Length == 0) ? "" : "\n");
		LogsPage.ViewModel.Logs += text;
	}

	public static bool UnzipModJsonMCZIP(string path, string key, string uuid)
	{
		try
		{
			string[] allMcZip = GetAllMcZip(path);
			foreach (string text in allMcZip)
			{
				AddLog(text);
				int num = UnzipModJson(key, text, uuid);
				if (num != 0)
				{
					AddLog("Decrypt Failed! code:" + num);
					break;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			AddLog(ex.Message);
			return false;
		}
	}

	public static void ClientLog(string log, ConsoleColor color = ConsoleColor.Gray)
	{
		Console.ForegroundColor = color;
		string message = $"[INFO][{DateTime.Now}]{log}";
		Console.ForegroundColor = ConsoleColor.Gray;
		AddLog(message);
	}

	public static void ClientError(string log, ConsoleColor color = ConsoleColor.Red)
	{
		Console.ForegroundColor = color;
		string message = "[ERROR][" + DateTime.Now.ToString() + "]" + log;
		Console.ForegroundColor = ConsoleColor.Gray;
		AddLog(message);
	}

	public static bool UnzipModJsonPath(string path, string key, string uuid)
	{
		try
		{
			string[] allMcZip = GetAllMcZip(path);
			string[] allFiless = GetAllFiless(path);
			string[] allFiles = GetAllFiles(path, ".mergedmcs");
			string[] array = allFiless;
			foreach (string text in array)
			{
				int num = UnUnzipJson(key, text, uuid);
				if (num != 0)
				{
					AddLog("Decrypt Failed! code:" + num + " ,path: " + text);
				}
			}
			array = allFiles;
			foreach (string text2 in array)
			{
				AddLog(text2);
				ZipFile.ExtractToDirectory(text2, Path.GetDirectoryName(text2.Substring(0, text2.Length - 10)));
				DeleteFile(text2);
			}
			array = allMcZip;
			foreach (string text3 in array)
			{
				AddLog(text3);
				ZipFile.ExtractToDirectory(text3, Path.GetDirectoryName(text3.Substring(0, text3.Length - 6)));
				DeleteFile(text3);
			}
			return true;
		}
		catch (Exception ex)
		{
			AddLog(ex.Message);
			return false;
		}
	}

	public static bool UnUnzipJsonPath(string path, string key, string uuid)
	{
		try
		{
			string[] allFiless = GetAllFiless(path);
			string[] allFiles = GetAllFiles(path, ".mergedmcs");
			string[] array = allFiless;
			foreach (string text in array)
			{
				AddLog(text);
				int num = UnUnzipJson(key, text, uuid);
				if (num != 0)
				{
					Console.WriteLine("Decrypt Failed! code:" + num);
					break;
				}
			}
			array = allFiles;
			foreach (string text2 in array)
			{
				Console.WriteLine(text2);
				ZipFile.ExtractToDirectory(text2, Path.GetDirectoryName(text2.Substring(0, text2.Length - 10)));
			}
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
	}

	public static int UnUnzipJson(string key, string path, string uuid)
	{
		try
		{
			byte[] array = x19Crypt.DecryptModJson(FileContent(path), Encoding.ASCII.GetBytes(key), uuid);
			if (array != null)
			{
				Console.WriteLine(path);
				DeleteFile(path);
				FileHelper.ByteToFile(array, path);
				return 0;
			}
			return 4;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return 1;
		}
	}

	public static int UnzipModJson(string key, string path, string uuid)
	{
		try
		{
			byte[] array = x19Crypt.DecryptModJson(FileContent(path), Encoding.ASCII.GetBytes(key), uuid);
			if (array == null)
			{
				ZipFile.ExtractToDirectory(path, Path.GetDirectoryName(path.Substring(0, path.Length - 6)));
				DeleteFile(path);
				return 0;
			}
			DeleteFile(path);
			FileHelper.ByteToFile(array, path);
			ZipFile.ExtractToDirectory(path, Path.GetDirectoryName(path.Substring(0, path.Length - 6)));
			DeleteFile(path);
			return 0;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return 1;
		}
	}

	public static string[] GetAllMcZip(string path)
	{
		return Directory.GetFiles(path, "*.mczip", SearchOption.AllDirectories);
	}

	public static string[] GetAllpng(string path)
	{
		return Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
	}

	public static string[] GetAlljpeg(string path)
	{
		return Directory.GetFiles(path, "*.jpeg", SearchOption.AllDirectories);
	}

	public static string[] GetAlljpg(string path)
	{
		return Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
	}

	public static string[] GetAlljson(string path)
	{
		return Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
	}

	public static string[] GetAlllang(string path)
	{
		return Directory.GetFiles(path, "*.lang", SearchOption.AllDirectories);
	}

	public static string[] GetAllmaterial(string path)
	{
		return Directory.GetFiles(path, "*.material", SearchOption.AllDirectories);
	}

	public static string[] GetAlltga(string path)
	{
		return Directory.GetFiles(path, "*.tga", SearchOption.AllDirectories);
	}

	public static string[] GetAllFiles(string path, string name)
	{
		return Directory.GetFiles(path, "*" + name, SearchOption.AllDirectories);
	}

	public static string[] GetAllFiless(string path)
	{
		List<string> list = new List<string>();
		List<string> list2 = Directory.GetFiles(path, "*", SearchOption.AllDirectories).ToList();
		for (int i = 0; i < list2.Count; i++)
		{
			if (Directory.Exists(list2[i]))
			{
				list.Concat(GetAllFiless(list2[i]));
			}
			else
			{
				list.Add(list2[i]);
			}
		}
		return list.ToArray();
	}

	public static bool DeleteFile(string fileFullPath)
	{
		if (!File.Exists(fileFullPath))
		{
			return false;
		}
		if (File.GetAttributes(fileFullPath) == FileAttributes.Directory)
		{
			Directory.Delete(fileFullPath, recursive: true);
			return true;
		}
		File.Delete(fileFullPath);
		return true;
	}

	public static string GetDecryptionKey(string device_id, string TextContentKey, string user_id)
	{
		try
		{
			string text = "TG8hVJD3Lt1r86Cv" + user_id + device_id;
			int length = text.Length;
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			byte[] array = Convert.FromBase64String(TextContentKey);
			for (int num = length - 1; num != -1; num--)
			{
				int num2 = num % 16;
				array[num2] ^= bytes[num];
			}
			return Encoding.ASCII.GetString(array);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return null;
		}
	}

	private static byte[] FileContent(string fileName)
	{
		using FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
		try
		{
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, (int)fileStream.Length);
			return array;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static async Task DownloadFile(string url, string filePath, Action<int> progressCallback)
	{
		_ = 3;
		try
		{
			using HttpClient httpClient = new HttpClient();
			using HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
			response.EnsureSuccessStatusCode();
			long totalBytes = response.Content.Headers.ContentLength ?? (-1);
			bool canReportProgress = totalBytes != -1;
			using FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
			using Stream contentStream = await response.Content.ReadAsStreamAsync();
			byte[] buffer = new byte[8192];
			long totalBytesRead = 0L;
			while (true)
			{
				int num;
				int bytesRead = (num = await contentStream.ReadAsync(buffer, 0, buffer.Length));
				if (num <= 0)
				{
					break;
				}
				await fileStream.WriteAsync(buffer, 0, bytesRead);
				totalBytesRead += bytesRead;
				if (canReportProgress && progressCallback != null)
				{
					int obj = (int)((double)totalBytesRead / (double)totalBytes * 100.0);
					progressCallback(obj);
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception("文件下载失败: " + ex.Message, ex);
		}
	}

	public static void ExtractZip(string zipPath, string extractPath, Action<int> progressCallback)
	{
		try
		{
			if (!Directory.Exists(extractPath))
			{
				Directory.CreateDirectory(extractPath);
			}
			using (ZipArchive zipArchive = ZipFile.OpenRead(zipPath))
			{
				int count = zipArchive.Entries.Count;
				int num = 0;
				foreach (ZipArchiveEntry entry in zipArchive.Entries)
				{
					string text = Path.Combine(extractPath, entry.FullName);
					if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\"))
					{
						Directory.CreateDirectory(text);
					}
					else
					{
						Directory.CreateDirectory(Path.GetDirectoryName(text));
						entry.ExtractToFile(text, overwrite: true);
					}
					num++;
					int obj = (int)((double)num / (double)count * 100.0);
					progressCallback?.Invoke(obj);
				}
			}
			AddLog($"[Extract] ZIP file '{zipPath}' extracted to '{extractPath}' successfully.");
		}
		catch (Exception ex)
		{
			AddLog("[Extract] Failed to extract ZIP file '" + zipPath + "'. Error: " + ex.Message);
			throw;
		}
	}
}
