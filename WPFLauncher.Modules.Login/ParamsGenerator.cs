using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace WPFLauncher.Modules.Login;

public static class ParamsGenerator
{
	public static string GetStringMd5(string input_string)
	{
		using MD5 mD = MD5.Create();
		byte[] bytes = Encoding.UTF8.GetBytes(input_string);
		return BitConverter.ToString(mD.ComputeHash(bytes)).Replace("-", "").ToLower();
	}

	public static string AesECBEncrypt(string data, string key)
	{
		using Aes aes = Aes.Create();
		aes.Key = Convert.FromHexString(key);
		aes.Mode = CipherMode.ECB;
		aes.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = aes.CreateEncryptor();
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		return BitConverter.ToString(cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length)).Replace("-", "").ToLower();
	}

	public static string GenerateParams(string username, string password, string device_key)
	{
		try
		{
			if (!File.Exists("device_info.json"))
			{
				Function.AddLog("device_info.json文件不存在，请检查文件路径");
				return null;
			}
			string key = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText("device_info.json"))["device_key"].ToString();
			return AesECBEncrypt(JsonSerializer.Serialize(new Dictionary<string, object>
			{
				{
					"password",
					GetStringMd5(password)
				},
				{ "unique_id", "11060edf9f833c9b8b02e00e0426f647" },
				{ "username", username }
			}), key);
		}
		catch (Exception ex)
		{
			Function.AddLog("生成参数时出错: " + ex.Message);
			return null;
		}
	}
}
