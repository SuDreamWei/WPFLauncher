using System;
using MCStudio.Modules.App;
using Mcl.Core.Extensions;
using Mcl.Core.Network;
using Mcl.Core.Network.Interface;
using Mcl.Core.Utils;
using Newtonsoft.Json;
using WPFLauncher.Model.Common;

namespace WPFLauncher.Network.Http;

public class X19HttpLogin
{
	public static string WebServerUrl = "https://x19mclobt.nie.netease.com";

	public static string CoreServerUrl = "https://x19exprcore.nie.netease.com:8443";

	private static JsonSerializerSettings DefaultJsonSettings = new JsonSerializerSettings
	{
		MissingMemberHandling = MissingMemberHandling.Ignore,
		NullValueHandling = NullValueHandling.Ignore
	};

	public static NetRequestAsyncHandle GetAsync(string resource, Action<INetResponse> callback = null, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.GET,
			Option = needEncrypt,
			Resource = resource
		}, delegate(INetResponse response)
		{
			Singleton<ThreadManager>.Instance.CallInMainThread(delegate
			{
				callback?.Invoke(response);
			});
		});
	}

	public static NetRequestAsyncHandle GetAsyncTask(string resource, Action<INetResponse> callback = null, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.GET,
			Option = needEncrypt,
			Resource = resource
		}, callback);
	}

	public static NetRequestAsyncHandle GetAsync<T>(string resource, Action<INetResponse<T>, NetRequestAsyncHandle> callback, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.GET,
			Option = needEncrypt,
			Resource = resource
		}, delegate(INetResponse<T> response, NetRequestAsyncHandle handle)
		{
			Singleton<ThreadManager>.Instance.CallInMainThread(delegate
			{
				callback?.Invoke(response, handle);
			});
		});
	}

	public static NetRequestAsyncHandle GetAsyncTask<T>(string resource, Action<INetResponse<T>, NetRequestAsyncHandle> callback, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.GET,
			Option = needEncrypt,
			Resource = resource
		}, callback);
	}

	public static NetRequestAsyncHandle PostAsync(string resource, string parameter, Action<INetResponse> callback = null, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.POST,
			Resource = resource,
			Option = needEncrypt
		}, delegate(INetResponse response)
		{
			Singleton<ThreadManager>.Instance.CallInMainThread(delegate
			{
				callback?.Invoke(response);
			});
		});
	}

	public static NetRequestAsyncHandle PostAsyncTask(string resource, string parameter, Action<INetResponse> callback = null, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.POST,
			Resource = resource,
			Option = needEncrypt
		}, callback);
	}

	public static NetRequestAsyncHandle PostAsync<T>(string resource, string parameter, Action<INetResponse<T>, NetRequestAsyncHandle> callback = null, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.POST,
			Resource = resource,
			Option = needEncrypt
		}, delegate(INetResponse<T> response, NetRequestAsyncHandle handle)
		{
			Singleton<ThreadManager>.Instance.CallInMainThread(delegate
			{
				callback?.Invoke(response, handle);
			});
		});
	}

	public static NetRequestAsyncHandle PostAsyncTask<T>(string resource, string parameter, Action<INetResponse<T>, NetRequestAsyncHandle> callback = null, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.POST,
			Resource = resource,
			Option = needEncrypt
		}, callback);
	}

	public static NetRequestAsyncHandle PatchAsync(string resource, string parameter, Action<INetResponse> callback = null, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.PATCH,
			Resource = resource,
			Option = needEncrypt
		}, delegate(INetResponse response)
		{
			Singleton<ThreadManager>.Instance.CallInMainThread(delegate
			{
				callback?.Invoke(response);
			});
		});
	}

	public static NetRequestAsyncHandle PatchAsyncTask(string resource, string parameter, Action<INetResponse> callback = null, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.PATCH,
			Resource = resource,
			Option = needEncrypt
		}, callback);
	}

	public static NetRequestAsyncHandle PatchAsync<T>(string resource, string parameter, Action<INetResponse<T>, NetRequestAsyncHandle> callback, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.PATCH,
			Resource = resource,
			Option = needEncrypt
		}, delegate(INetResponse<T> response, NetRequestAsyncHandle handle)
		{
			Singleton<ThreadManager>.Instance.CallInMainThread(delegate
			{
				callback?.Invoke(response, handle);
			});
		});
	}

	public static NetRequestAsyncHandle PatchAsyncTask<T>(string resource, string parameter, Action<INetResponse<T>, NetRequestAsyncHandle> callback, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.PATCH,
			Resource = resource,
			Option = needEncrypt
		}, callback);
	}

	public static NetRequestAsyncHandle DeleteAsync(string resource, Action<INetResponse> callback = null, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.DELETE,
			Resource = resource,
			Option = needEncrypt
		}, delegate(INetResponse response)
		{
			Singleton<ThreadManager>.Instance.CallInMainThread(delegate
			{
				callback?.Invoke(response);
			});
		});
	}

	public static NetRequestAsyncHandle DeleteAsyncTask(string resource, Action<INetResponse> callback = null, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.DELETE,
			Resource = resource,
			Option = needEncrypt
		}, callback);
	}

	public static NetRequestAsyncHandle DeleteAsync<T>(string resource, Action<INetResponse<T>, NetRequestAsyncHandle> callback, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.DELETE,
			Resource = resource,
			Option = needEncrypt
		}, delegate(INetResponse<T> response, NetRequestAsyncHandle handle)
		{
			Singleton<ThreadManager>.Instance.CallInMainThread(delegate
			{
				callback?.Invoke(response, handle);
			});
		});
	}

	public static NetRequestAsyncHandle DeleteAsyncTask<T>(string resource, Action<INetResponse<T>, NetRequestAsyncHandle> callback, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.DELETE,
			Resource = resource,
			Option = needEncrypt
		}, callback);
	}

	private static MclNetClient Create()
	{
		return new MclNetClient(WebServerUrl);
	}

	public static void DeserializeResponse<T>(INetRequest request, Action<INetResponse<T>, NetRequestAsyncHandle> callback, INetResponse response, NetRequestAsyncHandle asyncHandle) where T : new()
	{
		INetResponse<T> arg;
		try
		{
			arg = X19Http.Deserialize<T>(request, response);
		}
		catch (Exception ex)
		{
			arg = new NetResponse<T>
			{
				Request = request,
				ResponseStatus = ResponseStatus.Error,
				ErrorMessage = ex.Message,
				ErrorException = ex
			};
			Logger.Default.Error(ex, "DeserializeResponse");
		}
		callback?.Invoke(arg, asyncHandle);
	}

	public static INetResponse<T> Deserialize<T>(INetRequest request, INetResponse raw) where T : new()
	{
		INetResponse<T> netResponse = new NetResponse<T>();
		try
		{
			netResponse = raw.ToAsyncResponse<T>();
			netResponse.Request = request;
			if (netResponse.ErrorException == null)
			{
				netResponse.Data = JsonConvert.DeserializeObject<T>(raw.Content, DefaultJsonSettings);
			}
		}
		catch (Exception ex)
		{
			netResponse.ResponseStatus = ResponseStatus.Error;
			netResponse.ErrorMessage = ex.Message;
			netResponse.ErrorException = ex;
			Logger.Default.Error(ex, $"ResponseUri: {raw?.ResponseUri}" + Environment.NewLine + "Deserialize: " + raw?.Content);
		}
		if (netResponse.Data == null)
		{
			netResponse.Data = new T();
		}
		return netResponse;
	}

	private static NetRequestAsyncHandle GetResponseAsync<T>(X19HttpParameter param, Action<INetResponse<T>, NetRequestAsyncHandle> callback) where T : new()
	{
		MclNetClient mclNetClient = Create();
		MclNetRequest request = new MclNetRequest(param.Resource, param.Method, param.Option)
		{
			Body = param.Body,
			Timeout = param.Timeout
		};
		return mclNetClient.ExecuteAsync(request, delegate(INetResponse response, NetRequestAsyncHandle handle)
		{
			X19Http.DeserializeResponse(request, callback, response, handle);
		});
	}

	public static NetRequestAsyncHandle MakeHttp<T>(X19HttpParameter param, Action<INetResponse<T>, NetRequestAsyncHandle> callback) where T : ResponseBase, new()
	{
		return GetResponseAsync(param, delegate(INetResponse<T> response, NetRequestAsyncHandle handle)
		{
			callback?.Invoke(response, handle);
		});
	}

	public static NetRequestAsyncHandle MakeHttp(X19HttpParameter param, Action<INetResponse> callback)
	{
		MclNetClient mclNetClient = Create();
		MclNetRequest request = new MclNetRequest(param);
		return mclNetClient.ExecuteAsync(request, delegate(INetResponse response, NetRequestAsyncHandle handle)
		{
			callback?.Invoke(response);
		});
	}

	public static INetResponse MakeHttp(X19HttpParameter param)
	{
		INetResponse response = new NetResponse();
		Singleton<ThreadManager>.Instance.CallInNonMainThreadSync(delegate
		{
			MclNetClient mclNetClient = Create();
			MclNetRequest request = new MclNetRequest(param);
			response = mclNetClient.Execute(request);
		});
		return response;
	}

	public static INetResponse<T> MakeHttp<T>(X19HttpParameter param) where T : ResponseBase, new()
	{
		INetResponse<T> netResponse = new NetResponse<T>();
		Singleton<ThreadManager>.Instance.CallInNonMainThreadSync(delegate
		{
			MclNetClient mclNetClient = Create();
			MclNetRequest request = new MclNetRequest(param);
			INetResponse raw = mclNetClient.Execute(request);
			netResponse = X19Http.Deserialize<T>(request, raw);
		});
		return netResponse;
	}

	public static void MakeHttpAsync<T>(MclNetRequest request, Action<T> callback = null) where T : ResponseBase, new()
	{
		Create().ExecuteAsync(request, delegate(INetResponse response, NetRequestAsyncHandle handle)
		{
			INetResponse<T> netResponse = X19Http.Deserialize<T>(request, response);
			callback?.Invoke(netResponse.Data);
		});
	}

	public static INetResponse Get(string resource, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.GET,
			Option = needEncrypt,
			Resource = resource
		});
	}

	public static INetResponse<T> Get<T>(string resource, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp<T>(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.GET,
			Option = needEncrypt,
			Resource = resource
		});
	}

	public static INetResponse Post(string resource, string parameter, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.POST,
			Option = needEncrypt,
			Resource = resource
		});
	}

	public static INetResponse<T> Post<T>(string resource, string parameter, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp<T>(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.POST,
			Option = needEncrypt,
			Resource = resource
		});
	}

	public static INetResponse Put(string resource, string parameter, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.PUT,
			Option = needEncrypt,
			Resource = resource
		});
	}

	public static INetResponse<T> Put<T>(string resource, string parameter, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp<T>(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.PUT,
			Option = needEncrypt,
			Resource = resource
		});
	}

	public static INetResponse Delete(string resource, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.DELETE,
			Option = needEncrypt,
			Resource = resource
		});
	}

	public static INetResponse<T> Delete<T>(string resource, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp<T>(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.DELETE,
			Option = needEncrypt,
			Resource = resource
		});
	}

	public static INetResponse Head(string resource, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.HEAD,
			Option = needEncrypt,
			Resource = resource
		});
	}

	public static INetResponse<T> Head<T>(string resource, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp<T>(new X19HttpParameter
		{
			Body = string.Empty,
			Method = Method.HEAD,
			Option = needEncrypt,
			Resource = resource
		});
	}

	public static INetResponse Patch(string resource, string parameter, ProtocolOption needEncrypt = ProtocolOption.Normal)
	{
		return MakeHttp(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.PATCH,
			Option = needEncrypt,
			Resource = resource
		});
	}

	public static INetResponse<T> Patch<T>(string resource, string parameter, ProtocolOption needEncrypt = ProtocolOption.Normal) where T : ResponseBase, new()
	{
		return MakeHttp<T>(new X19HttpParameter
		{
			Body = parameter,
			Method = Method.PATCH,
			Option = needEncrypt,
			Resource = resource
		});
	}
}
