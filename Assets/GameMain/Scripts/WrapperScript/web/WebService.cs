using Ionic.Zlib;
using Newtonsoft.Json;
using SkyTrakWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UniRx;
using UnityEngine;
using Utilities;

public class WebService : MonoBehaviour
{
	public const string DevKey = "E4F68-874E7-C80E5-1E8C3";

	private const string SkyTrakBaseUrl = "/skytrak";

	private const string SkyGolfBetaBaseUrl = "https://b0baa0a7fff0ce025514b85f7387bc22clubsg.skygolf.com/api4";

	private const string SkyGolfBetaQABaseUrl = "https://qa2-8264ee52f589f4c0191aa94f87aa1aebclubsg.skygolf.com/api4";

	private const string SkyGolfProductionBaseUrl = "https://clubsg.skygolf.com/api4";

	private static WebService instance;

	private Queue<WebService.QueuedRequest>[] _requestsQueues;

	private WebService.QueuedRequest[] _currentRequests;

	//[SerializeField]
	private STSWServerUrlMode serverUrlMode = STSWServerUrlMode.STSW_SERVER_URL_QA;

	private const int QueuesCount = 1;

	private const int HttpRequestAttemptsCount = 5;

	private const string AcceptEncodingHeader = "ACCEPT-ENCODING";

	private const string ContentEncodingHeader = "CONTENT-ENCODING";

	private const string EncodingHeaderValue = "gzip";

	public static STSWServerUrlMode ServerUrl
	{
		get
		{
			return STSWServerUrlMode.STSW_SERVER_URL_PRODUCTION;
		}
	}

	private void Awake()
	{
		if (WebService.instance == null)
		{
			WebService.instance = this;
			DontDestroyOnLoad(base.gameObject);
			//base.gameObject.transform.parent = base.gameObject.transform.root.parent;
			this.CreateQueues();
		}
		else if (WebService.instance.gameObject != base.gameObject)
		{
			DestroyImmediate(base.gameObject);
		}
	}

	public static void CallWebService(ServerRequest request, Dictionary<string, string> postFields, Action<WebServiceResponse> callback, bool callbackInParallelThread = false)
	{
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		int shortestQueueIndex = WebService.Instance().GetShortestQueueIndex();
		WebService.Instance()._requestsQueues[shortestQueueIndex].Enqueue(new WebService.QueuedRequest(request, postFields, callback, callbackInParallelThread));
	}

	private void CreateQueues()
	{
		this._requestsQueues = new Queue<WebService.QueuedRequest>[1];
		this._currentRequests = new WebService.QueuedRequest[1];
		for (int i = 0; i < 1; i++)
		{
			this._requestsQueues[i] = new Queue<WebService.QueuedRequest>();
		}
	}

	private static string GetBaseUrl()
	{
		switch (WebService.ServerUrl)
		{
			case STSWServerUrlMode.STSW_SERVER_URL_BETA_TESTING:
				{
					return "https://b0baa0a7fff0ce025514b85f7387bc22clubsg.skygolf.com/api4";
				}
			case STSWServerUrlMode.STSW_SERVER_URL_QA:
				{
					return "https://qa2-8264ee52f589f4c0191aa94f87aa1aebclubsg.skygolf.com/api4";
				}
			case STSWServerUrlMode.STSW_SERVER_URL_PRODUCTION:
				{
					return "https://clubsg.skygolf.com/api4";
				}
		}
		throw new NotSupportedException(string.Format("Server URL mode {0} is not supported", WebService.ServerUrl));
	}

	private static string GetFieldsString(Dictionary<string, string> fields)
	{
		return (
			from kvp in fields
			select string.Format("{{ {0} : {1} }}", kvp.Key, kvp.Value)).Aggregate<string>((string s1, string s2) => string.Concat(s1, ",\n", s2));
	}

	private static string GetServerRequestUrl(ServerService service, ServerRequest request, bool isSkyTrakCall)
	{
		string baseUrl = WebService.GetBaseUrl();
		if (isSkyTrakCall)
		{
			baseUrl = string.Concat(baseUrl, "/skytrak");
		}
		return (service != ServerService.General ? string.Format("{0}/{1}/{2}.php", baseUrl, service.GetUrlPart(), request.GetUrlPart()) : string.Format("{0}/{1}.php", baseUrl, request.GetUrlPart()));
	}

	private int GetShortestQueueIndex()
	{
		int count = this._requestsQueues[0].Count;
		int num = 0;
		for (int i = 0; i < -1; i++)
		{
			if (this._requestsQueues[i + 1].Count < count)
			{
				count = this._requestsQueues[i + 1].Count;
				num = i + 1;
			}
		}
		return num;
	}

	private static WebService Instance()
	{
		return WebService.instance;
	}

	private static void ParseStatus(string json, out string code, out string status, out string message, out bool dataIsNotNull)
	{
		code = null;
		status = null;
		message = null;
		dataIsNotNull = false;
		if (string.IsNullOrEmpty(json))
		{
			return;
		}
		int num = 0;
		string value = null;
		byte[] bytes = Encoding.UTF8.GetBytes(json);
		try
		{
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				using (StreamReader streamReader = new StreamReader(memoryStream))
				{
					using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
					{
						while (jsonTextReader.Read())
						{
							if (jsonTextReader.TokenType == JsonToken.StartObject || jsonTextReader.TokenType == JsonToken.StartArray)
							{
								num++;
							}
							if (jsonTextReader.TokenType == JsonToken.EndObject || jsonTextReader.TokenType == JsonToken.EndArray)
							{
								num--;
							}
							string str = value;
							value = null;
							if (num == 2 && (jsonTextReader.TokenType == JsonToken.StartObject || jsonTextReader.TokenType == JsonToken.StartArray) && str == "data")
							{
								dataIsNotNull = true;
							}
							if (num == 1)
							{
								if (jsonTextReader.TokenType != JsonToken.PropertyName)
								{
									if (jsonTextReader.TokenType != JsonToken.String)
									{
										continue;
									}
									string lowerInvariant = (str ?? string.Empty).ToLowerInvariant();
									if (lowerInvariant == null)
									{
										continue;
									}
									if (lowerInvariant == "code")
									{
										code = jsonTextReader.Value as string;
									}
									else if (lowerInvariant == "status")
									{
										status = jsonTextReader.Value as string;
									}
									else if (lowerInvariant == "message")
									{
										message = jsonTextReader.Value as string;
									}
								}
								else
								{
									value = jsonTextReader.Value as string;
								}
							}
						}
					}
				}
			}
		}
		catch (Exception exception)
		{
			message = exception.Message;
		}
	}

	private void ProcessRequest(int index)
	{
		Action<Exception> action1 = null;
		WebService.QueuedRequest queuedRequest = this._currentRequests[index];
		Guid guid = Guid.NewGuid();
		string serverRequestUrl = WebService.GetServerRequestUrl(queuedRequest.Service, queuedRequest.Request, queuedRequest.IsSkyTrakCall);
		object[] service = new object[] { queuedRequest.Service, queuedRequest.Request, guid, serverRequestUrl, null, null };
		service[4] = (queuedRequest.PostFields.Count != 0 ? string.Format("{0}?{1}", serverRequestUrl, (
			from kvp in queuedRequest.PostFields
			select string.Format("{0}={1}", kvp.Key, kvp.Value)).Aggregate<string>((string s1, string s2) => string.Concat(s1, "&", s2))) : serverRequestUrl);
		service[5] = (queuedRequest.PostFields.Count != 0 ? (
			from kvp in queuedRequest.PostFields
			select string.Format("{{ {0}, {1} }}", kvp.Key, kvp.Value)).Aggregate<string>((string s1, string s2) => string.Concat(s1, ",\n", s2)) : string.Empty);
		AppLog.Log(string.Format("=== CallWebService. Service: {0}. Request: {1}. ID: {2}\nURL {3}\nGET: {4}\nData:\n{5}", service), true);
		WebService.Request(serverRequestUrl, queuedRequest.PostFields, (string response) => {
			string str;
			string str1;
			string str2;
			bool flag;
			AppLog.Log(string.Format("=== CallWebService Response. Service: {0}. Request: {1}. ID: {2}\nURL {3}\nData:\n{4}", new object[] { queuedRequest.Service, queuedRequest.Request, guid, serverRequestUrl, response }), true);
			if (string.IsNullOrEmpty(response))
			{
				NetworkInternetVerification.instance.Reset();
			}
			WebService.ParseStatus(response, out str2, out str, out str1, out flag);
			Action callback = () => queuedRequest.Callback(new WebServiceResponse(queuedRequest.Service, queuedRequest.Request, response, str, str1, str2, flag));
			if (!queuedRequest.CallbackInParallelThread)
			{
				callback();
			}
			else
			{
				Action action = callback;
				if (action1 == null)
				{
					action1 = (Exception ex) => AppLog.LogException(ex, true);
				}
				ThreadingUtilities.Run(action, null, action1);
			}
			this._currentRequests[index] = null;
		});
	}

	private static void Request(string serviceUrl, Dictionary<string, string> postFields, Action<string> callback)
	{
		IObservable<WWW> wWW;
		Dictionary<string, string> strs;
		WWWForm wWWForm = null;
		if (postFields != null)
		{
			wWWForm = new WWWForm();
			foreach (KeyValuePair<string, string> postField in postFields)
			{
				if (postField.Key == null)
				{
					throw new NullReferenceException(string.Format("Post field key is null. URL: {0}\nFields:\n{1}", serviceUrl, WebService.GetFieldsString(postFields)));
				}
				if (postField.Value == null)
				{
					throw new NullReferenceException(string.Format("Post field value for key '{0}' is null. URL: {1}\nFields:\n{2}", postField.Key, serviceUrl, WebService.GetFieldsString(postFields)));
				}
				wWWForm.AddField(postField.Key, postField.Value);
			}
		}
		string str = Uri.EscapeUriString(serviceUrl);
		if (wWWForm == null)
		{
			strs = new Dictionary<string, string>()
			{
				{ "ACCEPT-ENCODING", "gzip" }
			};
			wWW = ObservableWWW.GetWWW(str, strs, null);
		}
		else
		{
			strs = new Dictionary<string, string>()
			{
				{ "ACCEPT-ENCODING", "gzip" }
			};
			wWW = ObservableWWW.PostWWW(str, wWWForm, strs, null);
		}
		wWW.Retry<WWW>(5).CatchIgnore<WWW, WWWErrorException>((WWWErrorException ex) => {
			AppLog.LogError(string.Format("WWW error: {0}", ex.RawErrorMessage), true);
			callback(null);
		}).Select<WWW, string>((WWW www) => {
			string empty = string.Empty;
			www.responseHeaders.TryGetValue("CONTENT-ENCODING", out empty);
			if (empty != "gzip")
			{
				return www.text;
			}
			return WebService.Ungzip(www.bytes);
		}).Subscribe<string>(callback, (Exception e) => {
			AppLog.LogException(e, true);
			callback(null);
		});
	}

	public static void Reset()
	{
		Queue<WebService.QueuedRequest>[] queueArrays = WebService.instance._requestsQueues;
		for (int i = 0; i < (int)queueArrays.Length; i++)
		{
			queueArrays[i].Clear();
		}
	}

	private static string Ungzip(byte[] bytes)
	{
		string end;
		try
		{
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					using (StreamReader streamReader = new StreamReader(gZipStream, Encoding.UTF8))
					{
						end = streamReader.ReadToEnd();
					}
				}
			}
		}
		catch (Exception exception)
		{
			AppLog.LogException(exception, true);
			end = null;
		}
		return end;
	}

	private void Update()
	{
		for (int i = 0; i < 1; i++)
		{
			if (this._currentRequests[i] == null && this._requestsQueues[i].Count != 0)
			{
				this._currentRequests[i] = this._requestsQueues[i].Dequeue();
				this.ProcessRequest(i);
			}
		}
	}

	private class QueuedRequest
	{
		public Action<WebServiceResponse> Callback
		{
			get;
			private set;
		}

		public bool CallbackInParallelThread
		{
			get;
			private set;
		}

		public bool IsSkyTrakCall
		{
			get;
			private set;
		}

		public Dictionary<string, string> PostFields
		{
			get;
			private set;
		}

		public ServerRequest Request
		{
			get;
			private set;
		}

		public ServerService Service
		{
			get;
			private set;
		}

		public QueuedRequest(ServerRequest request, Dictionary<string, string> postFields, Action<WebServiceResponse> callback, bool callbackInParallelThread)
		{
			this.Service = request.GetService();
			this.Request = request;
			this.IsSkyTrakCall = request.IsSkyTrakCall();
			this.PostFields = postFields.ToDictionary<KeyValuePair<string, string>, string, string>((KeyValuePair<string, string> kvp) => kvp.Key, (KeyValuePair<string, string> kvp) => kvp.Value);
			this.Callback = callback;
			this.CallbackInParallelThread = callbackInParallelThread;
		}
	}
}