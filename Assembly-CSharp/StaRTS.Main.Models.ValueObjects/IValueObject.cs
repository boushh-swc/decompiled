using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public interface IValueObject
	{
		string Uid
		{
			get;
			set;
		}

		void ReadRow(Row row);
	}
}
