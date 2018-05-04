using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class StaticDataController
	{
		internal interface IStaticDataWrapper
		{
			void Flush();
		}

		internal class StaticDataWrapper<T> : StaticDataController.IStaticDataWrapper where T : IValueObject
		{
			public static Dictionary<string, T> StaticData;

			public void Flush()
			{
				StaticDataController.StaticDataWrapper<T>.StaticData = null;
			}
		}

		private List<StaticDataController.IStaticDataWrapper> staticDataWrapperList;

		public StaticDataController()
		{
			Service.StaticDataController = this;
			this.staticDataWrapperList = new List<StaticDataController.IStaticDataWrapper>();
		}

		public void Exterminate()
		{
			for (int i = this.staticDataWrapperList.Count - 1; i >= 0; i--)
			{
				this.staticDataWrapperList[i].Flush();
			}
			this.staticDataWrapperList.Clear();
		}

		public void Unload<T>() where T : IValueObject
		{
			int i = 0;
			int count = this.staticDataWrapperList.Count;
			while (i < count)
			{
				StaticDataController.IStaticDataWrapper staticDataWrapper = this.staticDataWrapperList[i];
				if (staticDataWrapper is StaticDataController.StaticDataWrapper<T>)
				{
					staticDataWrapper.Flush();
					this.staticDataWrapperList.RemoveAt(i);
					break;
				}
				i++;
			}
		}

		public void Load<T>(Catalog catalog, string sheetName) where T : IValueObject, new()
		{
			this.staticDataWrapperList.Add(new StaticDataController.StaticDataWrapper<T>());
			StaticDataController.StaticDataWrapper<T>.StaticData = new Dictionary<string, T>();
			Sheet sheet = catalog.GetSheet(sheetName);
			if (sheet == null)
			{
				return;
			}
			sheet.SetupColumnIndexes<T>();
			Dictionary<string, Row> allRows = sheet.GetAllRows();
			if (allRows != null)
			{
				foreach (KeyValuePair<string, Row> current in allRows)
				{
					Row value = current.Value;
					T vo = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
					vo.ReadRow(value);
					value.Invalidate();
					this.Add<T>(current.Key, vo);
				}
			}
			sheet.Invalidate();
		}

		public void Add<T>(string uid, T vo) where T : IValueObject
		{
			StaticDataController.StaticDataWrapper<T>.StaticData.Add(uid, vo);
		}

		public T Get<T>(string uid) where T : IValueObject
		{
			return StaticDataController.StaticDataWrapper<T>.StaticData[uid];
		}

		public T GetOptional<T>(string uid) where T : IValueObject
		{
			return (!StaticDataController.StaticDataWrapper<T>.StaticData.ContainsKey(uid)) ? default(T) : StaticDataController.StaticDataWrapper<T>.StaticData[uid];
		}

		public Dictionary<string, T>.ValueCollection GetAll<T>() where T : IValueObject
		{
			return StaticDataController.StaticDataWrapper<T>.StaticData.Values;
		}
	}
}
