using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Holonet
{
	public class HolonetGetCommandCenterEntriesResponse : AbstractResponse
	{
		public List<CommandCenterVO> CCVOs
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.CCVOs = new List<CommandCenterVO>();
			List<object> list = obj as List<object>;
			if (list != null)
			{
				int count = list.Count;
				StaticDataController staticDataController = Service.StaticDataController;
				for (int i = 0; i < count; i++)
				{
					string text = list[i] as string;
					if (!string.IsNullOrEmpty(text))
					{
						this.CCVOs.Add(staticDataController.Get<CommandCenterVO>(text));
					}
				}
			}
			return this;
		}
	}
}
