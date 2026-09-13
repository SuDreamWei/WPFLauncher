using WPFLauncher.Model.Common;

namespace MCStudio.Modules.Log;

internal class CrashReportEntity : EntityBase
{
	public static readonly string CRASH_BEFORE_LOGIN = "crash_report_before_login";

	public static readonly string CRASH = "crash_report";

	public string reason { get; set; }

	public long timestamp { get; set; }

	public string os_name { get; set; }

	public string os_ver { get; set; }

	public string mac_addr { get; set; }

	public string udid { get; set; }

	public string app_ver { get; set; }

	public string sdk_ver { get; set; }

	public string network { get; set; }

	public string disk { get; set; }

	public string launcher_type { get; set; }

	public CrashReportEntity()
	{
	}

	public CrashReportEntity(string msg, long timestamp, LogPublicData data)
	{
		reason = msg.Replace("\r\n", "");
		this.timestamp = timestamp;
		os_name = data.os_name;
		os_ver = data.os_ver;
		mac_addr = data.mac_addr;
		udid = data.udid;
		app_ver = data.app_ver;
		sdk_ver = data.sdk_ver;
		network = data.network;
		disk = data.disk;
		launcher_type = data.launcher_type;
	}
}
