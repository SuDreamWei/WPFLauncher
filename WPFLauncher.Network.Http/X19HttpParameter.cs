using System.Collections.Generic;
using Mcl.Core.Network;

namespace WPFLauncher.Network.Http;

public class X19HttpParameter
{
	public static int DETAULT_TIMEOUT = 300000;

	public Method Method { get; set; }

	public string Resource { get; set; }

	public string Body { get; set; }

	public int Timeout { get; set; }

	public ProtocolOption Option { get; set; }

	public Dictionary<string, string> Header { get; set; }

	public string CustomDomain { get; set; }

	public X19HttpParameter()
	{
		Resource = string.Empty;
		Timeout = DETAULT_TIMEOUT;
		Option = ProtocolOption.Normal;
		Header = new Dictionary<string, string>();
		Body = string.Empty;
		Method = Method.GET;
	}

	public X19HttpParameter(string resource)
	{
		Resource = resource;
		Timeout = DETAULT_TIMEOUT;
		Option = ProtocolOption.Normal;
		Header = new Dictionary<string, string>();
		Body = string.Empty;
		Method = Method.GET;
	}
}
