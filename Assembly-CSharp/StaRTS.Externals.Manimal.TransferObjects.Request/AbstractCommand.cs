using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Externals.Manimal.TransferObjects.Request
{
	public abstract class AbstractCommand<TRequest, TResponse> : AbstractRequest, ICommand, ISerializable where TRequest : AbstractRequest where TResponse : AbstractResponse
	{
		public delegate void OnSuccessCallback(TResponse response, object cookie);

		public delegate void OnFailureCallback(uint status, object cookie);

		private AbstractCommand<TRequest, TResponse>.OnSuccessCallback onSuccessCallback;

		private AbstractCommand<TRequest, TResponse>.OnFailureCallback onFailureCallback;

		public uint Id
		{
			get;
			private set;
		}

		public uint Tries
		{
			get;
			set;
		}

		public string Token
		{
			get;
			private set;
		}

		public uint Time
		{
			get;
			private set;
		}

		public string Action
		{
			get;
			private set;
		}

		public string Description
		{
			get
			{
				return this.Action;
			}
		}

		public TRequest RequestArgs
		{
			get;
			protected set;
		}

		public TResponse ResponseResult
		{
			get;
			protected set;
		}

		public object Context
		{
			get;
			set;
		}

		public AbstractRequest Request
		{
			get
			{
				return this.RequestArgs;
			}
		}

		public AbstractCommand(string action, TRequest request, TResponse response)
		{
			this.Id = RequestId.Get();
			this.Token = RequestToken.Get();
			this.Tries = 1u;
			this.Action = action;
			this.RequestArgs = request;
			this.ResponseResult = response;
			this.onSuccessCallback = null;
			this.onFailureCallback = null;
			this.Time = 0u;
		}

		public abstract void OnSuccess();

		public abstract OnCompleteAction OnFailure(uint status, object data);

		public void AddSuccessCallback(AbstractCommand<TRequest, TResponse>.OnSuccessCallback onSuccessCallback)
		{
			if (this.onSuccessCallback != null)
			{
				Service.Logger.Error("Cannot add multiple success callbacks");
			}
			this.onSuccessCallback = onSuccessCallback;
		}

		public void AddFailureCallback(AbstractCommand<TRequest, TResponse>.OnFailureCallback onFailureCallback)
		{
			if (this.onFailureCallback != null)
			{
				Service.Logger.Error("Cannot add multiple failure callbacks");
			}
			this.onFailureCallback = onFailureCallback;
		}

		public void RemoveAllCallbacks()
		{
			this.onSuccessCallback = null;
			this.onFailureCallback = null;
		}

		public virtual OnCompleteAction OnComplete(Data data, bool success)
		{
			if (data.Result != null)
			{
				TResponse responseResult = this.ResponseResult;
				responseResult.FromObject(data.Result);
			}
			if (success)
			{
				this.OnSuccess();
				if (this.onSuccessCallback != null)
				{
					this.onSuccessCallback(this.ResponseResult, this.Context);
				}
				return OnCompleteAction.Ok;
			}
			if (this.onFailureCallback != null)
			{
				this.onFailureCallback(data.Status, this.Context);
			}
			return this.OnFailure(data.Status, data.Result);
		}

		public ICommand SetTime(uint time)
		{
			this.Time = time;
			return this;
		}

		protected virtual bool IsAddToken()
		{
			return true;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start().AddString("action", this.Action).AddObject<TRequest>("args", this.RequestArgs).Add<uint>("requestId", this.Id).Add<uint>("time", this.Time);
			if (this.IsAddToken())
			{
				serializer.AddString("token", this.Token);
			}
			serializer.End();
			return serializer.ToString();
		}
	}
}
