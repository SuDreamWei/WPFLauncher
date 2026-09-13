using System;

namespace WPFLauncher.Modules.Login;

[Serializable]
internal class LoginOTPRequest
{
	public string sauth_json { get; set; }
}
