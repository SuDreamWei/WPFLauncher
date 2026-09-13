using System;

namespace WPFLauncher.Modules.Login;

[Serializable]
public class VersionEntity
{
	public string version { get; set; }

	public string launcher_md5 { get; set; }

	public string updater_md5 { get; set; }
}
