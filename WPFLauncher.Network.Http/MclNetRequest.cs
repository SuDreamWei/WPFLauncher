using System.Collections.Generic;
using Mcl.Core.Network;

namespace WPFLauncher.Network.Http;

public class MclNetRequest : NetRequest
{
	public string Body { get; set; }

	public ProtocolOption NeedEncrypt { get; set; }

	public string KeyIn { get; set; }

	public string KeyOut { get; set; }

	public MclNetRequest(string resource, Method method, ProtocolOption needEncrypt = ProtocolOption.Normal)
		: base(resource, method)
	{
		Body = string.Empty;
		KeyIn = string.Empty;
		KeyOut = string.Empty;
		NeedEncrypt = needEncrypt;
		AddHeader("Content-Type", "application/json");
	}

	public MclNetRequest(X19HttpParameter param)
		: base(param.Resource, param.Method)
	{
		Body = param.Body;
		NeedEncrypt = param.Option;
		KeyIn = string.Empty;
		KeyOut = string.Empty;
		base.Timeout = param.Timeout;
		foreach (KeyValuePair<string, string> item in param.Header)
		{
			AddHeader(item.Key, item.Value);
		}
	}
}
