using System;
using MCStudio.Modules.Log;
using Mcl.Core.Network;
using Mcl.Core.Network.Interface;
using Mcl.Core.Utils;
using Newtonsoft.Json;
using WPFLauncher.Model.Common;
using WPFLauncher.Network.Http;

namespace WPFLauncher.Modules.Login;

internal static class LoginProtocolWeb
{
	private const string LOGIN_OTP = "/login-otp";

	private const string AUTHENTICATION_OTP = "/authentication-otp";

	public const string USER_DETAIL = "/user-detail";

	public const string USER_DETAIL_FOR_APICENTER = "/user-detail-for-apicenter";

	public const string NICKNAME_SETTING = "/nickname-setting";

	public const string AUTHENTICATION = "/authentication";

	private static int HeartBeatNum;

	public static long LastHeartBeatTime;

	public static void GetDC(string sauthJson, Action<EntityResponse<LoginOTPEntity>> callback = null)
	{
		LoginOTPRequest value = new LoginOTPRequest
		{
			sauth_json = sauthJson
		};
		X19HttpLogin.PostAsync("/login-otp", JsonConvert.SerializeObject(value), delegate(INetResponse<EntityResponse<LoginOTPEntity>> response, NetRequestAsyncHandle handle)
		{
			callback?.Invoke(response.Data);
		});
	}

	public static void CreateAuthentication(string sauth_json, string version, string launcher_md5, string updater_md5, string aid, string otpToken, string otpPwd, Action<EntityResponse<AuthenticationEntity>> callback)
	{
		X19HttpLogin.PostAsync("/authentication-otp", JsonConvert.SerializeObject(new AuthenticationEntity
		{
			sa_data = JsonConvert.SerializeObject(Singleton<LogManager>.Instance.GetLogPublicData()),
			sauth_json = sauth_json,
			version = new VersionEntity
			{
				version = version,
				launcher_md5 = launcher_md5,
				updater_md5 = updater_md5
			},
			aid = aid,
			otp_token = otpToken,
			otp_pwd = otpPwd
		}), delegate(INetResponse<EntityResponse<AuthenticationEntity>> response, NetRequestAsyncHandle handle)
		{
			callback?.Invoke(response.Data);
		}, ProtocolOption.Authentication);
	}

	public static void GetUserDetail(string userId, Action<EntityResponse<UserDetailEntity>> callback)
	{
		X19Http.PostAsync("/user-detail/", "", delegate(INetResponse<EntityResponse<UserDetailEntity>> response, NetRequestAsyncHandle handle)
		{
			callback?.Invoke(response.Data);
		});
	}

	public static void CreateNickname(string userId, string name, Action<EntityResponse<NicknameSettingEntity>> callback)
	{
		string parameter = JsonConvert.SerializeObject(new NicknameSettingEntity
		{
			entity_id = userId,
			name = name
		});
		X19Http.PostAsync("/nickname-setting", parameter, delegate(INetResponse<EntityResponse<NicknameSettingEntity>> response, NetRequestAsyncHandle handle)
		{
			callback?.Invoke(response?.Data);
		});
	}

	public static void UpdateAuthentication(string userId, Action<EntityResponse<AuthenticationEntity>> callback)
	{
		if (TimeHelper.GetUNIXTimeStamp() - LastHeartBeatTime >= 1200)
		{
			HeartBeatNum++;
			Logger.Default.Error($"{userId} UpdateAuthentication {HeartBeatNum}");
			string resource = "/authentication/" + userId;
			string parameter = JsonConvert.SerializeObject(new AuthenticationEntity
			{
				entity_id = userId
			});
			X19Http.PatchAsync(resource, parameter, delegate(INetResponse<EntityResponse<AuthenticationEntity>> response, NetRequestAsyncHandle handle)
			{
				callback?.Invoke(response?.Data);
			}, ProtocolOption.Authentication);
		}
	}
}
