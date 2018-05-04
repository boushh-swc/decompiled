using System;

namespace StaRTS.Utils.MetaData
{
	public class Column
	{
		public string ColName
		{
			get;
			private set;
		}

		public ColumnType ColType
		{
			get;
			private set;
		}

		public Column(string colName, ColumnType colType)
		{
			this.ColName = colName;
			this.ColType = colType;
		}
	}
}
