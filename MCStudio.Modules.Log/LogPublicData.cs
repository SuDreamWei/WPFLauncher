using System;

namespace MCStudio.Modules.Log;

[Serializable]
public class LogPublicData
{
	public string os_name { get; set; }

	public string os_ver { get; set; }

	public string mac_addr { get; set; }

	public string udid { get; set; }

	public string app_ver { get; set; }

	public string sdk_ver { get; set; }

	public string network { get; set; }

	public string disk { get; set; }

	public string launcher_type { get; set; }

	public string pay_channel { get; set; }

	public LogPublicData(string os_name, string mac_addr, string udid, string app_ver, string disk, string os_ver)
	{
		this.os_name = os_name;
		this.mac_addr = mac_addr;
		this.udid = udid;
		this.app_ver = app_ver;
		this.disk = disk;
		this.os_ver = os_ver;
		sdk_ver = string.Empty;
		network = string.Empty;
		launcher_type = "PC_java";
		pay_channel = "netease";
	}
}
