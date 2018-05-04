using System;

namespace StaRTS.Main.Controllers.ServerMessages
{
	public static class MessageFactory
	{
		private const string MESSAGE_KEEP_ALIVE = "keepAlive";

		private const string MESSAGE_SQUAD = "guild";

		private const string MESSAGE_DELTA = "delta";

		private const string MESSAGE_CONTRACT_FINISHED = "finishedContracts";

		private const string MESSAGE_ADMIN = "admin";

		private const string MESSAGE_LOGIN = "login";

		private const string MESSAGE_CHECKPOINT = "checkpoint";

		private const string MESSAGE_TRANSACTIONS = "transactions";

		private const string MESSAGE_IAP_RECEIPT = "iapReceipt";

		public static IMessage CreateMessage(string messageType, object message, out bool valid)
		{
			valid = true;
			if (messageType == "delta")
			{
				return new DeltaMessage().FromObject(message) as IMessage;
			}
			if (messageType == "keepAlive")
			{
				return new KeepAliveMessage().FromObject(message) as IMessage;
			}
			if (messageType == "guild")
			{
				return new SquadServerMessage().FromObject(message) as IMessage;
			}
			if (messageType == "finishedContracts")
			{
				return new ContractFinishedMessage().FromObject(message) as IMessage;
			}
			if (messageType == "admin")
			{
				return new AdminMessage().FromObject(message) as IMessage;
			}
			if (messageType == "iapReceipt")
			{
				return new IAPReceiptServerMessage().FromObject(message) as IMessage;
			}
			if (messageType == "login" || messageType == "checkpoint" || messageType == "transactions")
			{
				return null;
			}
			valid = false;
			return null;
		}
	}
}
