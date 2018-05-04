using System;
using System.Collections.Generic;

namespace StaRTS.Externals.Manimal
{
	public interface IResponseHandler
	{
		bool MatchProtocolVersion(uint protocolVersion);

		void SendMessages(Dictionary<string, object> messages);

		bool Desync(DesyncType type, uint status);
	}
}
