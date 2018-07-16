using System;
using System.Runtime.CompilerServices;
using Utilities;

public class WebServiceResponse
{
	private const string SuccessMessage = "success";

	private readonly bool _dataIsNotNull;

	public int Code
	{
		get;
		private set;
	}

	public JSONObject Data
	{
		get
		{
			JSONObject item;
			if (!this._dataIsNotNull)
			{
				item = null;
			}
			else
			{
				item = (new JSONObject(this.RawResponse, -2, false, false))["data"];
			}
			return item;
		}
	}

	public string Message
	{
		get;
		private set;
	}

	public string RawResponse
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

	public string Status
	{
		get;
		private set;
	}

	public bool Success
	{
		get;
		private set;
	}

	public WebServiceResponse(ServerService service, ServerRequest request, string rawResponse, string status, string message, string code, bool dataIsNotNull)
	{
		this.Service = service;
		this.Request = request;
		this.RawResponse = rawResponse;
		this.Status = status;
		this.Message = message;
		this.Code = (!string.IsNullOrEmpty(code) ? int.Parse(code) : -1);
		this._dataIsNotNull = dataIsNotNull;
		this.Success = (this.Status == null || !dataIsNotNull || this.Code != 0 ? false : this.Status.Equals("success", StringComparison.InvariantCultureIgnoreCase));
	}

	public override string ToString()
	{
		return string.Format("WebServiceResponse. Status: {0}. Message: {1}.\nRaw:\n{2}", this.Status, this.Message, this.RawResponse ?? "null");
	}
}