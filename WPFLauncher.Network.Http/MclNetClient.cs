using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Mcl.Core.Network;
using Mcl.Core.Network.Interface;
using Mcl.Core.Utils;
using Newtonsoft.Json;

namespace WPFLauncher.Network.Http;

internal class MclNetClient : NetClient
{
	public static string UserID;

	private static string _userToken;

	public static string USerTokenMD5;

	public static string UserTokenKeyName = "user-token";

	public static string UserIdKeyName = "user-id";

	public static string AuthenticationkeyName = "Authentication";

	private static int m_current = 0;

	private static object m_locker = new object();

	private static Queue<string> m_supenmanList = new Queue<string>();

	private static Queue<int> m_nextcountList = new Queue<int>();

	private static Queue<ulong> m_timeStampList = new Queue<ulong>();

	private static uint m_id = 0u;

	private const int INTERVAL = 300;

	private bool mIsG79Http;

	private string mG79UserID;

	private string mG79Token;

	public static string UserToken
	{
		get
		{
			return _userToken;
		}
		set
		{
			_userToken = value;
			USerTokenMD5 = GetMd532(value);
		}
	}

	public bool IsWebSrv { get; set; }

	private static bool BeWanted
	{
		get
		{
			if (m_supenmanList.Count != 0 && m_nextcountList.Count != 0)
			{
				return m_current == m_nextcountList.Peek();
			}
			return false;
		}
	}

	private static void Enqueue(string content)
	{
		if (!string.IsNullOrEmpty(content))
		{
			m_supenmanList.Enqueue(content);
			int item = NumberHelper.RandomNumber(1, 5);
			m_nextcountList.Enqueue(item);
			m_timeStampList.Enqueue((ulong)TimeHelper.GetUNIXTimeStamp());
		}
	}

	private static string Dequeue()
	{
		m_current = 0;
		m_timeStampList.Dequeue();
		m_nextcountList.Dequeue();
		return m_supenmanList.Dequeue();
	}

	private static bool IsWanted()
	{
		if (m_supenmanList.Count > 0 && TimeHelper.GetUNIXTimeStamp() - (long)m_timeStampList.Peek() > 300)
		{
			m_current++;
		}
		return BeWanted;
	}

	public static string GetMd532(string str)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
		bytes = mD5CryptoServiceProvider.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array = bytes;
		foreach (byte b in array)
		{
			stringBuilder.AppendFormat("{0:x2}", b);
		}
		return stringBuilder.ToString();
	}

	public static void MakeSuperMan(string type, object dict, uint sequenceId = 0u)
	{
		lock (m_locker)
		{
			Enqueue(JsonConvert.SerializeObject(new SupperMCEntity
			{
				t = type,
				c = dict,
				id = m_id++,
				sid = sequenceId
			}));
		}
	}

	private static bool Equals(string keyIn, string keyOut)
	{
		if (!string.IsNullOrEmpty(keyIn) && !string.IsNullOrEmpty(keyOut) && keyIn.Length == 16 && keyOut.Length == 16)
		{
			return keyOut.Substring(0, 8) == keyIn.Substring(8, 8);
		}
		return false;
	}

	private void Authentication(MclNetRequest request)
	{
		if (!IsWebSrv)
		{
			return;
		}
		lock (m_locker)
		{
		}
		if (Var.CurrentAccount.HasLogin)
		{
			string value = X19Crypto.ComputeDynamicToken(request.Resource, request.Body);
			if (mIsG79Http)
			{
				request.AddHeader(UserIdKeyName, mG79UserID);
				request.AddHeader(UserTokenKeyName, mG79Token);
			}
			else
			{
				request.AddHeader(UserIdKeyName, UserID);
				request.AddHeader(UserTokenKeyName, value);
			}
		}
		if (request.NeedEncrypt == ProtocolOption.Authentication || request.NeedEncrypt == ProtocolOption.CommonEncrypt)
		{
			string keyStr = string.Empty;
			byte[] value2 = X19Crypto.HttpEncrypt(request.Resource, request.Body, out keyStr);
			request.KeyIn = keyStr;
			request.AddParameter(string.Empty, value2, ParameterType.RequestBody);
		}
		else
		{
			request.AddParameter(string.Empty, request.Body, ParameterType.RequestBody);
		}
	}

	private void Decrypt(INetResponse response, MclNetRequest request)
	{
		if (response == null || response.ErrorException != null)
		{
			Logger.Default.Error($"ErrorException:{response?.ErrorException}" + "url:" + request?.Resource);
			return;
		}
		string keyStr = string.Empty;
		switch (request.NeedEncrypt)
		{
		case ProtocolOption.Normal:
			break;
		case ProtocolOption.CommonEncrypt:
			response.Content = X19Crypto.HttpDecrypt(response.RawBytes, out keyStr);
			break;
		case ProtocolOption.Authentication:
			response.Content = X19Crypto.ParseLoginResponse(response.RawBytes, out keyStr);
			Logger.Default.Info("Authentication Response: " + response.Content + ", key: " + keyStr);
			break;
		}
	}

	public INetResponse Execute(MclNetRequest request)
	{
		Authentication(request);
		INetResponse netResponse = base.Execute(request);
		Decrypt(netResponse, request);
		return netResponse;
	}

	public NetRequestAsyncHandle ExecuteAsync(MclNetRequest request, Action<INetResponse, NetRequestAsyncHandle> callback)
	{
		Authentication(request);
		return base.ExecuteAsync(request, delegate(INetResponse response, NetRequestAsyncHandle handle)
		{
			Decrypt(response, request);
			callback(response, handle);
		});
	}

	public MclNetClient(string baseUrl, bool isWebSrv = true)
		: base(baseUrl)
	{
		IsWebSrv = isWebSrv;
	}

	public void SetG79Token(long userId, string token)
	{
		mG79UserID = userId.ToString();
		mG79Token = token;
		mIsG79Http = true;
	}
}
