using System;
using WPFLauncher.Model.Common;

namespace WPFLauncher.Modules.Login;

[Serializable]
public class AuthenticationEntity : EntityBase
{
	public string sa_data { get; set; }

	public string sauth_json { get; set; }

	public VersionEntity version { get; set; }

	public string sdkuid { get; set; }

	public string aid { get; set; }

	public bool hasMessage { get; set; }

	public bool hasGmail { get; set; }

	public string otp_token { get; set; }

	public string otp_pwd { get; set; }

	public string token { get; set; }

	public int lock_time { get; set; }

	public string env { get; set; }

	public string min_engine_version { get; set; }

	public string min_patch_version { get; set; }
}
