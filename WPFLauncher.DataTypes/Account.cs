using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using JS4399MC;
using Mcl.Core.Network.Interface;
using Mcl.Core.Utils;
using Newtonsoft.Json;
using WPFLauncher.Model.Common;
using WPFLauncher.Modules.Login;
using WPFLauncher.Modules.Utils;
using WPFLauncher.Network.Http;

namespace WPFLauncher.DataTypes;

[JsonObject(MemberSerialization.OptIn)]
public class Account
{
	public bool HasLogin;

	public bool HasLogout;

	public static Account CurrentSelectedAccount { get; set; }

	[JsonIgnore]
	public bool IsCurrentSelected
	{
		get
		{
			return CurrentSelectedAccount == this;
		}
		set
		{
			if (value)
			{
				CurrentSelectedAccount = this;
			}
			else if (CurrentSelectedAccount == this)
			{
				CurrentSelectedAccount = null;
			}
			OnPropertyChanged("IsCurrentSelected");
		}
	}

	[JsonProperty]
	public string NickName { get; set; }

	[JsonProperty]
	public string UserName { get; set; }

	[JsonProperty]
	public string Password { get; set; }

	[JsonProperty]
	public AccountTypes AccountType { get; set; }

	[JsonIgnore]
	public EntityResponse<AuthenticationEntity> AuthenticationResult { get; set; }

	[JsonIgnore]
	public EntityResponse<LoginOTPEntity> LoginOtpResult { get; set; }

	[JsonIgnore]
	public EntityResponse<UserDetailEntity> UserDetail { get; set; }

	[JsonIgnore]
	public string Token { get; set; }

	[JsonIgnore]
	public string UserID { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public event Action<Account> LoginSuccess;

	public event Action<Account, string> LoginFailed;

	public event Action<string> LoginProgressUpdated;

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	private void UpdateLoginProgress(string step)
	{
		LoginProgressUpdated?.Invoke(step);
	}

	public async Task Login(Func<string, Task<string>> captchaCallback = null)
	{
		Var.CurrentAccount = this;
		string text = string.Empty;
		UpdateLoginProgress("1/4 获取Sauth...");
		switch (AccountType)
		{
		case AccountTypes.Netease:
		{
			if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
			{
				LoginFailed?.Invoke(this, "用户名和密码都不得为空");
				return;
			}
			Dictionary<string, object> dictionary = LoginHelper.NeteaseLoginToSauth(UserName, Password);
			if (dictionary == null)
			{
				LoginFailed?.Invoke(this, "网易登录出现错误,详情请看日志板块");
				return;
			}
			text = JsonConvert.SerializeObject(dictionary);
			break;
		}
		case AccountTypes._4399:
		{
			if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
			{
				LoginFailed?.Invoke(this, "用户名和密码都不得为空");
				return;
			}
			JS4399Result jS4399Result = await new JS4399(new JS4399HttpConfig()).JS4399LoginAsync(new Dictionary<string, object>
			{
				{ "username", UserName },
				{ "password", Password }
			}, captchaCallback);
			if (!jS4399Result.Success)
			{
				Function.AddLog("登录出错: " + jS4399Result.Message);
				LoginFailed?.Invoke(this, "登录出错: " + jS4399Result.Message);
				return;
			}
			text = jS4399Result.SauthJson;
			break;
		}
		case AccountTypes.Cookie:
			if (string.IsNullOrWhiteSpace(UserName))
			{
				LoginFailed?.Invoke(this, "Sauth不得为空");
				return;
			}
			text = UserName;
			break;
		}
		Function.AddLog("Sauth: " + text);
		if (string.IsNullOrWhiteSpace(text))
		{
			LoginFailed?.Invoke(this, "无法成功获取到Sauth");
			return;
		}
		if (!Tools.IsVaildJson(text))
		{
			LoginFailed?.Invoke(this, "Sauth格式错误");
			return;
		}
		string message = "";
		string sauthJson = JsonConvert.DeserializeObject<LoginOTPRequest>(text).sauth_json;
		UpdateLoginProgress("2/4 获取一次性token...");
		LoginProtocolWeb.GetDC(sauthJson, delegate(EntityResponse<LoginOTPEntity> response)
		{
			if (response != null && response.code == 0 && response.entity != null)
			{
				Logger.Default.Info("loginOTP登录结果: " + response.message + ", 一次性token: " + response.entity.otp_token);
				string wPFLauncherVersion = Var.WPFLauncherVersion;
				string updater_md = "";
				string launcher_md = "";
				LoginProtocolWeb.CreateAuthentication(sauthJson, wPFLauncherVersion, launcher_md, updater_md, response.entity.aid, response.entity.otp_token, null, delegate(EntityResponse<AuthenticationEntity> authenticationResponse)
				{
					if (authenticationResponse != null && authenticationResponse.code == 0 && authenticationResponse?.entity != null)
					{
						Logger.Default.Info("AuthenticationOtp登录成功, entity_id: " + authenticationResponse.entity.entity_id);
						MclNetClient.UserID = authenticationResponse.entity.entity_id;
						MclNetClient.UserToken = authenticationResponse.entity.token;
						Token = authenticationResponse.entity.token;
						UserID = authenticationResponse.entity.entity_id;
						HasLogin = true;
						UpdateLoginProgress("3/4 获取AuthenticationOtp...");
						LoginProtocolWeb.GetUserDetail(authenticationResponse.entity.entity_id, delegate(EntityResponse<UserDetailEntity> userDetail)
						{
							UpdateLoginProgress("4/4 获取用户信息完成");
							if (userDetail != null && userDetail.code == 0 && userDetail?.entity != null)
							{
								Logger.Default.Info("成功获取到账户信息, NickName: " + userDetail.entity.name);
								Logger.Default.Info("登录成功!");
								UserDetail = userDetail;
								NickName = userDetail.entity.name;
								Application.Current.Dispatcher.Invoke(delegate
								{
									OnPropertyChanged("NickName");
									OnPropertyChanged("UserID");
									OnPropertyChanged("HasLogin");
									LoginSuccess?.Invoke(this);
								});
								Task.Run(delegate
								{
									UpdateAccount();
								});
							}
						});
					}
					else
					{
						message = (string.IsNullOrEmpty(authenticationResponse?.message) ? "登录返回异常" : authenticationResponse?.message);
						Function.ShowDialog(message, "登录认证异常");
					}
				});
			}
			else
			{
				message = (string.IsNullOrEmpty(response?.message) ? "登录异常" : response?.message);
				Logger.Default.Error("登录OTP失败: " + message);
				if (Application.Current?.Dispatcher == null)
				{
					try
					{
						LoginFailed?.Invoke(this, message);
						return;
					}
					catch (Exception err)
					{
						Logger.Default.Error(err, "登录失败回调执行异常");
						return;
					}
				}
				Application.Current.Dispatcher.BeginInvoke((Action)delegate
				{
					try
					{
						LoginFailed?.Invoke(this, message);
					}
					catch (Exception err2)
					{
						Logger.Default.Error(err2, "登录失败回调执行异常");
					}
				});
			}
		});
	}

	public void Logout()
	{
		X19HttpLogin.Post("/authentication/delete", JsonConvert.SerializeObject(new
		{
			user_id = UserID,
			logout_type = 0
		}));
		HasLogout = true;
	}

	public void UpdateAccount()
	{
		while (!HasLogout)
		{
			INetResponse netResponse = X19HttpLogin.Post("/authentication/update", JsonConvert.SerializeObject(new
			{
				entity_id = UserID
			}), ProtocolOption.Authentication);
			try
			{
				AuthenticationResult = JsonConvert.DeserializeObject<EntityResponse<AuthenticationEntity>>(netResponse.Content);
				Token = AuthenticationResult.entity.token;
				UserID = AuthenticationResult.entity.entity_id;
			}
			catch (Exception ex)
			{
				Function.AddLog($"账号更新失败: {netResponse.Content}, Message: {ex.Message}\nStackTrace:{ex.StackTrace}");
			}
			Thread.Sleep(600000);
		}
	}
}
