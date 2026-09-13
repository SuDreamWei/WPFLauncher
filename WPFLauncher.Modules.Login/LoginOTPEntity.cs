using System;
using WPFLauncher.Model.Common;

namespace WPFLauncher.Modules.Login;

[Serializable]
public class LoginOTPEntity : EntityBase
{
	public const int OPEN = 1;

	public int otp { get; set; }

	public string otp_token { get; set; }

	public string aid { get; set; }

	public int lock_time { get; set; }

	public int open_otp { get; set; }
}
