using System.Collections.Generic;

namespace WPFLauncher.Modules.Login;

public static class LoginHelper
{
	public static Dictionary<string, object> FullLogin(string username, string password, bool force_new_device = false)
	{
		NeteaseLogin neteaseLogin = new NeteaseLogin();
		Dictionary<string, object> dictionary = new Dictionary<string, object> { { "success", false } };
		Dictionary<string, object> dictionary2 = (Dictionary<string, object>)(dictionary["device_info"] = neteaseLogin.RegisterDevice(force_new_device));
		if (!(bool)dictionary2["success"])
		{
			return dictionary;
		}
		Dictionary<string, object> dictionary4 = (Dictionary<string, object>)(dictionary["login_result"] = neteaseLogin.Login(username, password));
		if (!(bool)dictionary4["success"])
		{
			return dictionary;
		}
		Dictionary<string, object> dictionary6 = (Dictionary<string, object>)(dictionary["ticket_result"] = neteaseLogin.CreateTicket());
		if (!(bool)dictionary6["success"])
		{
			return dictionary;
		}
		Dictionary<string, object> dictionary8 = (Dictionary<string, object>)(dictionary["ticket_login_result"] = neteaseLogin.LoginWithTicket());
		if (!(bool)dictionary8["success"])
		{
			return dictionary;
		}
		Dictionary<string, object> authInfo = neteaseLogin.GetAuthInfo();
		dictionary["auth_info"] = authInfo;
		Dictionary<string, object> dictionary10 = (Dictionary<string, object>)(dictionary["mc_login_result"] = neteaseLogin.LoginMinecraftServer());
		dictionary["success"] = dictionary10["success"];
		return dictionary;
	}

	public static Dictionary<string, object> NeteaseLoginToSauth(string username, string password)
	{
		NeteaseLogin neteaseLogin = new NeteaseLogin();
		if (!(bool)neteaseLogin.RegisterDevice()["success"])
		{
			return null;
		}
		if (!(bool)neteaseLogin.Login(username, password)["success"])
		{
			return null;
		}
		if (!(bool)neteaseLogin.CreateTicket()["success"])
		{
			return null;
		}
		if (!(bool)neteaseLogin.LoginWithTicket()["success"])
		{
			return null;
		}
		return neteaseLogin.GetSauthJson();
	}
}
