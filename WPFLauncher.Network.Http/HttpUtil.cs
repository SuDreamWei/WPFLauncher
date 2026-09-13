using System;
using System.Collections.Generic;
using System.Net;
using MCStudio.Modules.App;
using Mcl.Core.Network;
using Mcl.Core.Network.Interface;
using Mcl.Core.Utils;
using Newtonsoft.Json;

namespace WPFLauncher.Network.Http;

public class HttpUtil
{
	private static MclNetClient Create(string baseUrl)
	{
		return new MclNetClient(baseUrl, isWebSrv: false);
	}

	public static MclNetRequest CreateRequest(Method method, string url, object body, List<Cookie> cookies = null, Dictionary<string, string> header = null)
	{
		string text = string.Empty;
		if (body != null)
		{
			text = JsonConvert.SerializeObject(body);
		}
		MclNetRequest mclNetRequest = new MclNetRequest(string.Empty, method)
		{
			Body = text
		};
		if (header != null)
		{
			foreach (KeyValuePair<string, string> item in header)
			{
				mclNetRequest.AddHeader(item.Key, item.Value);
			}
		}
		mclNetRequest.AddParameter(string.Empty, text, ParameterType.RequestBody);
		if (cookies != null)
		{
			foreach (Cookie cookie in cookies)
			{
				mclNetRequest.AddCookie(cookie.Name, cookie.Value);
			}
		}
		return mclNetRequest;
	}

	public static INetResponse Get(string url)
	{
		MclNetClient mclNetClient = Create(url);
		MclNetRequest request = new MclNetRequest(string.Empty, Method.GET)
		{
			Body = string.Empty
		};
		return mclNetClient.Execute(request);
	}

	public static NetRequestAsyncHandle GetAsync(string url, Action<string> callback)
	{
		MclNetClient mclNetClient = Create(url);
		MclNetRequest request = new MclNetRequest(string.Empty, Method.GET)
		{
			Body = string.Empty
		};
		return mclNetClient.ExecuteAsync(request, delegate
		{
			Singleton<ThreadManager>.Instance.CallInMainThread(delegate
			{
				callback?.Invoke(((INetResponse)null)?.Content);
			});
		});
	}

	public static INetResponse MakeHttp(Method method, string url, object body, List<Cookie> cookies = null, Dictionary<string, string> header = null)
	{
		MclNetClient mclNetClient = Create(url);
		MclNetRequest request = CreateRequest(method, url, body, cookies, header);
		return mclNetClient.Execute(request);
	}

	public static INetResponse<T> MakeHttp<T>(Method method, string url, object body, List<Cookie> cookies = null, Dictionary<string, string> header = null) where T : new()
	{
		INetResponse<T> result = null;
		Singleton<ThreadManager>.Instance.CallInNonMainThreadSync(delegate
		{
			MclNetClient mclNetClient = Create(url);
			MclNetRequest request = CreateRequest(method, url, body, cookies, header);
			INetResponse raw = mclNetClient.Execute(request);
			result = X19Http.Deserialize<T>(request, raw);
		});
		return result;
	}

	public static void MakeHttpAsync<T>(Method method, string url, object body, List<Cookie> cookies = null, Dictionary<string, string> header = null, Action<INetResponse<T>, NetRequestAsyncHandle> callback = null) where T : new()
	{
		MclNetClient mclNetClient = Create(url);
		MclNetRequest request = CreateRequest(method, url, body, cookies, header);
		mclNetClient.ExecuteAsync(request, delegate(INetResponse response, NetRequestAsyncHandle handle)
		{
			X19Http.DeserializeResponse(request, callback, response, handle);
		});
	}

	public static void MakeHttpAsync(Method method, string url, object body, List<Cookie> cookies = null, Dictionary<string, string> header = null, Action<INetResponse, NetRequestAsyncHandle> callback = null)
	{
		MclNetClient mclNetClient = Create(url);
		MclNetRequest request = CreateRequest(method, url, body, cookies, header);
		mclNetClient.ExecuteAsync(request, delegate(INetResponse response, NetRequestAsyncHandle handler)
		{
			callback?.Invoke(response, handler);
		});
	}
}
