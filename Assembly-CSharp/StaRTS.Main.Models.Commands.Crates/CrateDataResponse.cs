using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class CrateDataResponse : AbstractResponse
	{
		public CrateData CrateDataTO
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			this.CrateDataTO = new CrateData();
			this.CrateDataTO.FromObject(dictionary);
			return this;
		}
	}
}
