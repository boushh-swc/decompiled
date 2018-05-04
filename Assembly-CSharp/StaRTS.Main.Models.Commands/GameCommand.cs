using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands
{
	public abstract class GameCommand<TRequest, TResponse> : AbstractCommand<TRequest, TResponse> where TRequest : AbstractRequest where TResponse : AbstractResponse
	{
		public GameCommand(string action, TRequest request, TResponse response) : base(action, request, response)
		{
		}

		public override void OnSuccess()
		{
		}

		public override OnCompleteAction OnFailure(uint status, object data)
		{
			return this.HandleFailure(status, data);
		}

		private OnCompleteAction HandleFailure(uint status, object data)
		{
			bool flag = true;
			string text = null;
			if (status != 917u)
			{
				if (status != 1900u)
				{
					if (status == 1999u)
					{
						string playerId = Service.CurrentPlayer.PlayerId;
						text = Service.Lang.Get("DESYNC_BANNED", new object[]
						{
							playerId
						});
					}
				}
				else
				{
					Service.PlayerIdentityController.HandleInactiveIdentityError(data as string);
					flag = false;
				}
			}
			else
			{
				text = Service.Lang.Get("DESYNC_DUPLICATE_SESSION", new object[0]);
			}
			if (!flag)
			{
				return OnCompleteAction.Ok;
			}
			if (text != null)
			{
				string biMessage = text + " Status : " + status;
				AlertScreen.ShowModalWithBI(true, null, text, biMessage);
			}
			return OnCompleteAction.Desync;
		}

		protected OnCompleteAction EatFailure(uint status, object data)
		{
			this.HandleFailure(status, data);
			return OnCompleteAction.Ok;
		}
	}
}
