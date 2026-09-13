using System;
using System.Net;

namespace WPFLauncher.Model.Common;

[Serializable]
public class ResponseBase
{
	public HttpStatusCode statusCode { get; set; } = HttpStatusCode.OK;

	public int code { get; set; } = -1;

	public string message { get; set; }
}
