using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace WPFLauncher.Modules.Login;

public class NeteaseLogin
{
	private readonly string app_key_login = "aecfrxodyqaaaajp-g-x19";

	private readonly string app_key_register = "aecglf6ee4aaaarz-g-a50";

	private readonly string base_url = "https://service.mkey.163.com";

	private readonly string device_file = "device_info.json";

	private readonly HttpClient httpClient;

	private readonly HttpClientHandler httpClientHandler;

	private readonly string login_version = "1.14.6.45947";

	private readonly string register_version = "1.5.0.24";

	private readonly string timestamp;

	private readonly string transaction_id;

	private readonly string udid;

	private readonly string uni_transaction_id;

	private string device_id;

	private string device_key;

	private string final_token;

	private Dictionary<string, string> headers;

	private Dictionary<string, string> json_headers;

	private string ticket;

	private string token;

	private string user_id;

	public NeteaseLogin(bool load_device = true)
	{
		httpClientHandler = new HttpClientHandler
		{
			ServerCertificateCustomValidationCallback = (HttpRequestMessage sender, X509Certificate2? cert, X509Chain? chain, SslPolicyErrors sslPolicyErrors) => true
		};
		httpClient = new HttpClient(httpClientHandler);
		headers = new Dictionary<string, string>
		{
			{ "Content-Type", "application/x-www-form-urlencoded" },
			{ "User-Agent", "Go-http-client/1.1" }
		};
		json_headers = new Dictionary<string, string>
		{
			{ "Content-Type", "application/json" },
			{ "User-Agent", "Go-http-client/1.1" }
		};
		timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		udid = GenerateUdid();
		transaction_id = udid + "_" + timestamp + "_100003704";
		uni_transaction_id = udid + "_" + timestamp + "_100005231";
		if (load_device)
		{
			LoadDeviceInfo();
		}
	}

	private string GenerateUdid()
	{
		return Guid.NewGuid().ToString("N").ToUpper();
	}

	public Dictionary<string, object> RegisterDevice(bool force_new = false)
	{
		if (!string.IsNullOrEmpty(device_id) && !string.IsNullOrEmpty(device_key) && !force_new)
		{
			return new Dictionary<string, object>
			{
				{ "success", true },
				{ "device_id", device_id },
				{ "device_key", device_key },
				{ "message", "使用已保存的设备信息" }
			};
		}
		string requestUri = base_url + "/mpay/games/" + app_key_register + "/devices";
		Random random = new Random();
		string value = string.Join("-", from _ in Enumerable.Range(0, 6)
			select random.Next(0, 256).ToString("X2"));
		string value2 = new Random().Next(100000).ToString();
		FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
		{
			{ "app_channel", "netease.allysdk3rd" },
			{ "app_mode", "2" },
			{ "app_type", "games" },
			{ "arch", "win_x64" },
			{ "brand", "Microsoft" },
			{ "cv", "c4.2.0" },
			{ "device_model", "pc_mode" },
			{
				"device_name",
				"DESKTOP-" + Guid.NewGuid().ToString("N").Substring(0, 8)
					.ToUpper()
			},
			{ "device_type", "Computer" },
			{ "game_id", app_key_register },
			{ "gv", register_version },
			{ "init_urs_device", "0" },
			{ "mac", value },
			{ "mcount_app_key", "EEkEEXLymcNjM42yLY3Bn6AO15aGy4yq" },
			{ "mcount_transaction_id", "1" },
			{ "process_id", value2 },
			{ "resolution", "1920*1080" },
			{ "sv", "10.0.22631" },
			{ "system_name", "windows" },
			{ "system_version", "10.0.22631" },
			{
				"trans_id",
				udid + "_" + timestamp + "_100003704"
			},
			{ "udid", udid },
			{ "uni_transaction_id", uni_transaction_id },
			{ "unique_id", "11060edf9f833c9b8b02e00e0426f647" },
			{ "updater_cv", "c1.0.0" }
		});
		Dictionary<string, object> dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(httpClient.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result);
		if (dictionary.TryGetValue("device", out var value3) && value3 is JsonElement jsonElement)
		{
			Dictionary<string, object> dictionary2 = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonElement.GetRawText());
			if (dictionary2.TryGetValue("id", out var value4) && dictionary2.TryGetValue("key", out var value5))
			{
				device_id = value4.ToString();
				device_key = value5.ToString();
				SaveDeviceInfo();
				return new Dictionary<string, object>
				{
					{ "success", true },
					{ "device_id", device_id },
					{ "device_key", device_key }
				};
			}
		}
		return new Dictionary<string, object>
		{
			{ "success", false },
			{ "message", "设备注册失败" },
			{ "result", dictionary }
		};
	}

	private void SaveDeviceInfo()
	{
		if (string.IsNullOrEmpty(device_id) || string.IsNullOrEmpty(device_key))
		{
			return;
		}
		Dictionary<string, object> value = new Dictionary<string, object>
		{
			{ "device_id", device_id },
			{ "device_key", device_key },
			{ "udid", udid },
			{ "timestamp", timestamp },
			{
				"created_at",
				DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
			}
		};
		try
		{
			File.WriteAllText(device_file, JsonSerializer.Serialize(value, new JsonSerializerOptions
			{
				WriteIndented = true
			}));
		}
		catch (Exception ex)
		{
			Function.AddLog("保存设备信息失败: " + ex.Message);
		}
	}

	private void LoadDeviceInfo()
	{
		if (!File.Exists(device_file))
		{
			return;
		}
		try
		{
			Dictionary<string, object> dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(device_file));
			if (dictionary.TryGetValue("device_id", out var value) && dictionary.TryGetValue("device_key", out var value2))
			{
				device_id = value.ToString();
				device_key = value2.ToString();
			}
		}
		catch (Exception ex)
		{
			Function.AddLog("加载设备信息失败: " + ex.Message);
		}
	}

	public Dictionary<string, object> Login(string username, string password)
	{
		if (string.IsNullOrEmpty(device_id))
		{
			return new Dictionary<string, object>
			{
				{ "success", false },
				{ "message", "请先注册设备" }
			};
		}
		string requestUri = $"{base_url}/mpay/games/{app_key_register}/devices/{device_id}/users";
		string value = Convert.ToBase64String(Encoding.UTF8.GetBytes(username));
		string value2 = ParamsGenerator.GenerateParams(username, password, device_key);
		Dictionary<string, string> nameValueCollection = new Dictionary<string, string>
		{
			{ "app_channel", "netease.allysdk3rd" },
			{ "app_mode", "2" },
			{ "app_type", "games" },
			{ "arch", "win_x64" },
			{ "cv", "c4.2.0" },
			{ "game_id", app_key_register },
			{ "gv", register_version },
			{ "mcount_app_key", "EEkEEXLymcNjM42yLY3Bn6AO15aGy4yq" },
			{ "mcount_transaction_id", "3" },
			{ "opt_fields", "nickname,avatar,realname_status,mobile_bind_status,mask_related_mobile,related_login_status" },
			{
				"process_id",
				new Random().Next(100000).ToString()
			},
			{ "sv", "10.0.22631" },
			{
				"transid",
				udid + "_" + timestamp + "_100003704"
			},
			{ "un", value },
			{ "uni_transaction_id", uni_transaction_id },
			{ "updater_cv", "c1.0.0" },
			{ "params", value2 }
		};
		try
		{
			FormUrlEncodedContent content = new FormUrlEncodedContent(nameValueCollection);
			Dictionary<string, object> dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(httpClient.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result);
			if (dictionary.TryGetValue("user", out var value3) && value3 is JsonElement jsonElement)
			{
				Dictionary<string, object> dictionary2 = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonElement.GetRawText());
				if (dictionary2.TryGetValue("token", out var value4) && dictionary2.TryGetValue("id", out var value5))
				{
					token = value4.ToString();
					user_id = value5.ToString();
					return new Dictionary<string, object>
					{
						{ "success", true },
						{ "user", dictionary2 }
					};
				}
			}
			Function.AddLog($"错误: {(dictionary.TryGetValue("reason", out var value6) ? value6 : "未知错误")}");
			return new Dictionary<string, object>
			{
				{ "success", false },
				{ "message", "登录失败" },
				{ "result", dictionary }
			};
		}
		catch (Exception ex)
		{
			Function.AddLog("加密出错: " + ex.Message);
			return new Dictionary<string, object>
			{
				{ "success", false },
				{
					"message",
					"加密出错: " + ex.Message
				}
			};
		}
	}

	public Dictionary<string, object> CreateTicket()
	{
		if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(user_id))
		{
			return new Dictionary<string, object>
			{
				{ "success", false },
				{ "message", "请先登录" }
			};
		}
		string requestUri = base_url + "/mpay/api/users/create_ticket";
		FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
		{
			{ "app_channel", "netease.allysdk3rd" },
			{ "app_mode", "2" },
			{ "app_type", "games" },
			{ "arch", "win_x64" },
			{ "cv", "c4.2.0" },
			{ "device_id", device_id },
			{ "game_id", app_key_register },
			{ "gv", register_version },
			{ "mcount_app_key", "EEkEEXLymcNjM42yLY3Bn6AO15aGy4yq" },
			{ "mcount_transaction_id", "3" },
			{
				"process_id",
				new Random().Next(100000).ToString()
			},
			{ "sv", "10.0.22631" },
			{ "token", token },
			{ "transid", transaction_id },
			{ "uni_transaction_id", uni_transaction_id },
			{ "updater_cv", "c1.0.0" },
			{ "user_id", user_id }
		});
		Dictionary<string, object> dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(httpClient.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result);
		if (dictionary.TryGetValue("ticket", out var value))
		{
			ticket = value.ToString();
			return new Dictionary<string, object>
			{
				{ "success", true },
				{ "ticket", ticket }
			};
		}
		return new Dictionary<string, object>
		{
			{ "success", false },
			{ "message", "创建票据失败" },
			{ "result", dictionary }
		};
	}

	public Dictionary<string, object> LoginWithTicket()
	{
		if (string.IsNullOrEmpty(ticket))
		{
			return new Dictionary<string, object>
			{
				{ "success", false },
				{ "message", "请先创建票据" }
			};
		}
		string requestUri = base_url + "/mpay/api/users/login/ticket";
		FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
		{
			{ "app_channel", "a50_sdk_cn" },
			{ "app_mode", "2" },
			{ "app_type", "games" },
			{ "arch", "win_x64" },
			{ "cv", "c4.2.0" },
			{ "device_id", device_id },
			{ "game_id", app_key_login },
			{ "gv", login_version },
			{ "mcount_app_key", "EEkEEXLymcNjM42yLY3Bn6AO15aGy4yq" },
			{
				"mcount_transaction_id",
				Guid.NewGuid().ToString()
			},
			{ "opt_fields", "nickname,avatar,realname_status,mobile_bind_status,mask_related_mobile,related_login_status" },
			{
				"process_id",
				new Random().Next(100000).ToString()
			},
			{ "sv", "10.0.22631" },
			{ "ticket", ticket },
			{ "transid", transaction_id },
			{ "uni_transaction_id", uni_transaction_id },
			{ "updater_cv", "c1.0.0" }
		});
		Dictionary<string, object> dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(httpClient.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result);
		if (dictionary.TryGetValue("user", out var value) && value is JsonElement jsonElement)
		{
			Dictionary<string, object> dictionary2 = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonElement.GetRawText());
			if (dictionary2.TryGetValue("token", out var value2))
			{
				final_token = value2.ToString();
				return new Dictionary<string, object>
				{
					{ "success", true },
					{ "user", dictionary2 }
				};
			}
		}
		return new Dictionary<string, object>
		{
			{ "success", false },
			{ "message", "使用票据登录失败" },
			{ "result", dictionary }
		};
	}

	public Dictionary<string, object> GetSauthJson()
	{
		if (string.IsNullOrEmpty(final_token) || string.IsNullOrEmpty(user_id))
		{
			return new Dictionary<string, object>
			{
				{ "success", false },
				{ "message", "请先完成登录流程" }
			};
		}
		Dictionary<string, object> value = new Dictionary<string, object>
		{
			{ "app_channel", "a50_sdk_cn" },
			{ "sdkuid", user_id },
			{ "sessionid", final_token },
			{ "aim_info", "{\"aim\":\"111.111.111.111\",\"country\":\"CN\",\"tz\":\"+0800\",\"tzid\":\"\"}" },
			{ "source_platform", "pc" },
			{ "platform", "pc" },
			{ "sdk_version", "4.2.0" },
			{ "gas_token", "" },
			{ "client_login_sn", udid },
			{ "gameid", "x19" },
			{ "login_channel", "netease" },
			{ "udid", udid },
			{ "deviceid", device_id },
			{ "ip", "1.1.1.1" },
			{ "get_access_token", "1" }
		};
		return new Dictionary<string, object> { 
		{
			"sauth_json",
			JsonSerializer.Serialize(value)
		} };
	}

	public Dictionary<string, object> GetAuthInfo()
	{
		if (string.IsNullOrEmpty(final_token) || string.IsNullOrEmpty(user_id))
		{
			return new Dictionary<string, object>
			{
				{ "success", false },
				{ "message", "请先完成登录流程" }
			};
		}
		string requestUri = "https://mgbsdk.matrix.netease.com/x19/sdk/uni_sauth";
		StringContent content = new StringContent(JsonSerializer.Serialize(new Dictionary<string, object>
		{
			{ "app_channel", "a50_sdk_cn" },
			{ "sdkuid", user_id },
			{ "sessionid", final_token },
			{ "aim_info", "{\"aim\":\"111.111.111.111\",\"country\":\"CN\",\"tz\":\"+0800\",\"tzid\":\"\"}" },
			{ "source_platform", "pc" },
			{ "platform", "pc" },
			{ "sdk_version", "4.2.0" },
			{ "gas_token", "" },
			{ "client_login_sn", udid },
			{ "gameid", "x19" },
			{ "login_channel", "netease" },
			{ "udid", udid },
			{ "deviceid", device_id },
			{ "ip", "1.1.1.1" },
			{ "get_access_token", "1" }
		}), Encoding.UTF8, "application/json");
		return JsonSerializer.Deserialize<Dictionary<string, object>>(httpClient.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result);
	}

	public Dictionary<string, object> LoginMinecraftServer()
	{
		if (string.IsNullOrEmpty(user_id))
		{
			return new Dictionary<string, object>
			{
				{ "success", false },
				{ "message", "请先完成登录流程" }
			};
		}
		string requestUri = "https://x19.update.netease.com/serverlist/release.json";
		if (!JsonSerializer.Deserialize<Dictionary<string, object>>(httpClient.GetAsync(requestUri).Result.Content.ReadAsStringAsync().Result).TryGetValue("CoreServerUrl", out var value))
		{
			return new Dictionary<string, object>
			{
				{ "success", false },
				{ "message", "无法获取服务器信息" }
			};
		}
		string requestUri2 = value?.ToString() + "/login-otp";
		Dictionary<string, object> value2 = new Dictionary<string, object>
		{
			{ "gameid", "x19" },
			{ "login_channel", "netease" },
			{ "app_channel", "a50_sdk_cn" },
			{ "platform", "pc" },
			{ "sdkuid", user_id },
			{ "sessionid", final_token },
			{ "sdk_version", "4.2.0" },
			{ "udid", udid },
			{ "deviceid", device_id },
			{ "aim_info", "{\"aim\":\"111.111.111.111\",\"country\":\"CN\",\"tz\":\"+0800\",\"tzid\":\"\"}" },
			{ "client_login_sn", udid },
			{ "gas_token", "" },
			{ "source_platform", "pc" },
			{ "ip", "1.1.1.1" },
			{ "get_access_token", "1" }
		};
		StringContent content = new StringContent(JsonSerializer.Serialize(new Dictionary<string, object> { 
		{
			"sauth_json",
			JsonSerializer.Serialize(value2)
		} }), Encoding.UTF8, "application/json");
		Dictionary<string, object> dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(httpClient.PostAsync(requestUri2, content).Result.Content.ReadAsStringAsync().Result);
		if (dictionary.TryGetValue("code", out var value3) && value3.ToString() == "0" && dictionary.TryGetValue("entity", out var value4))
		{
			return new Dictionary<string, object>
			{
				{ "success", true },
				{ "message", "登录成功" },
				{ "otp_info", value4 }
			};
		}
		return new Dictionary<string, object>
		{
			{ "success", false },
			{ "message", "登录失败" },
			{ "result", dictionary }
		};
	}
}
