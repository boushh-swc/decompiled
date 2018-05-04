using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Externals.Manimal.TransferObjects.Request
{
	public interface ICommand : ISerializable
	{
		uint Time
		{
			get;
		}

		uint Id
		{
			get;
		}

		uint Tries
		{
			get;
			set;
		}

		string Description
		{
			get;
		}

		string Token
		{
			get;
		}

		AbstractRequest Request
		{
			get;
		}

		OnCompleteAction OnComplete(Data data, bool success);

		ICommand SetTime(uint time);
	}
}
