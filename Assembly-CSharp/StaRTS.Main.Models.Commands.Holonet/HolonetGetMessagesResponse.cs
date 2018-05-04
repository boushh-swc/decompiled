using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Holonet
{
	public class HolonetGetMessagesResponse : AbstractResponse
	{
		public List<TransmissionVO> MessageVOs
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.MessageVOs = new List<TransmissionVO>();
			List<object> list = obj as List<object>;
			if (list != null)
			{
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					this.MessageVOs.Add(TransmissionVO.CreateFromServerObject(list[i]));
				}
			}
			return this;
		}
	}
}
