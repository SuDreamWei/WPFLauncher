using System;
using WPFLauncher.Model.Common;

namespace WPFLauncher.Modules.Login;

[Serializable]
public class UserDetailEntity : EntityBase
{
	public string account { get; set; }

	public string gender { get; set; }

	public string name { get; set; }

	public string signature { get; set; }

	public string avatar_image_url { get; set; }

	public string frame_id { get; set; }

	public uint register_time { get; set; }

	public uint login_time { get; set; }

	public uint logout_time { get; set; }

	public uint realname_status { get; set; }

	public bool isAntiAddiction { get; set; }

	public bool need_realname_auth { get; set; }

	public int nickname_free { get; set; }

	public int nickname_init { get; set; }

	public uint level { get; set; }

	public uint score { get; set; }

	public int freezed { get; set; }

	public int skin_number { get; set; }

	public int cape_number { get; set; }

	public string instruct_info { get; set; }

	public int rest_currency_time { get; set; }
}
