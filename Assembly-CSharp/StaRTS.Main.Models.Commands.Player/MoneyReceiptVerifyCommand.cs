using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class MoneyReceiptVerifyCommand : GameCommand<MoneyReceiptVerifyRequest, MoneyReceiptVerifyResponse>
	{
		public const string ACTION = "player.money.receipt.verify";

		private int[] consumePurchaseErrors = new int[]
		{
			918,
			919,
			920,
			921,
			922,
			923,
			924,
			925,
			926,
			927,
			928,
			929,
			930,
			931,
			932,
			933,
			934,
			948
		};

		public string ProductId
		{
			get;
			set;
		}

		public string TransactionId
		{
			get;
			private set;
		}

		public MoneyReceiptVerifyCommand(MoneyReceiptVerifyRequest request) : base("player.money.receipt.verify", request, new MoneyReceiptVerifyResponse())
		{
		}

		private bool IsPurchaseErrorConsumable(uint status)
		{
			for (int i = 0; i < this.consumePurchaseErrors.Length; i++)
			{
				if ((long)this.consumePurchaseErrors[i] == (long)((ulong)status))
				{
					return true;
				}
			}
			return false;
		}

		public void SetTransactionId(string transactionId)
		{
			this.TransactionId = transactionId;
			MoneyReceiptVerifyResponse responseResult = base.ResponseResult;
			if (responseResult != null)
			{
				responseResult.TransactionId = transactionId;
			}
		}

		public override OnCompleteAction OnFailure(uint status, object data)
		{
			if (this.IsPurchaseErrorConsumable(status))
			{
				Service.InAppPurchaseController.Consume(this.ProductId);
			}
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.Enabled = true;
			}
			return base.OnFailure(status, data);
		}

		public override OnCompleteAction OnComplete(Data data, bool success)
		{
			MoneyReceiptVerifyResponse responseResult = base.ResponseResult;
			responseResult.Status = data.Status;
			return base.OnComplete(data, success);
		}

		public override void OnSuccess()
		{
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.Enabled = true;
			}
		}
	}
}
