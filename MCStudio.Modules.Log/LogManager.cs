using System;
using MCStudio.Modules.App;
using Mcl.Core.Utils;
using Newtonsoft.Json;
using WPFLauncher;
using WPFLauncher.Network.Http;

namespace MCStudio.Modules.Log;

public class LogManager : ManagerBase<LogManager>
{
	private LogPublicData m_publicData;

	public override void Initial()
	{
		if (m_publicData == null)
		{
			string oSystem = WindowsSystemHelper.GetOSystem();
			string mACAddress = WindowsSystemHelper.GetMACAddress();
			string diskNum = WindowsSystemHelper.GetDiskNum();
			string udid = WindowsSystemHelper.GetUdid(diskNum);
			string wPFLauncherVersion = Var.WPFLauncherVersion;
			string oSFriendlyName = WindowsSystemHelper.GetOSFriendlyName();
			m_publicData = new LogPublicData(oSystem, mACAddress, udid, wPFLauncherVersion, diskNum, oSFriendlyName);
		}
	}

	public LogPublicData GetLogPublicData()
	{
		if (m_publicData == null)
		{
			Initial();
		}
		return m_publicData;
	}

	public void LogCrashReport(string msg)
	{
		try
		{
			Initial();
			string type = (Var.CurrentAccount.HasLogin ? CrashReportEntity.CRASH : CrashReportEntity.CRASH_BEFORE_LOGIN);
			CrashReportEntity value = new CrashReportEntity(msg, TimeHelper.GetUNIXTimeStamp(), m_publicData);
			string text = JsonConvert.SerializeObject(new OperationLogEntity<string>
			{
				type = type,
				data = JsonConvert.SerializeObject(value)
			});
			Logger.Default.Fatal("[LogCrashReport]:" + text);
			X19Http.PostAsyncTask("/salog", text);
		}
		catch (Exception err)
		{
			Logger.Default.Fatal(err, "LogCrashReport");
		}
	}
}
