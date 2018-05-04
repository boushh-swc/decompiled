using StaRTS.Main.Models.Entities;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Entities
{
	public class PlanetAttachmentTO
	{
		public SmartEntity Entity
		{
			get;
			set;
		}

		public GameObject Locator
		{
			get;
			set;
		}
	}
}
