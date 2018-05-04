using StaRTS.Main.Views.UX.Elements;
using System;
using UnityEngine;

namespace StaRTS.Main.Models
{
	public class EventTickerObject
	{
		public string message
		{
			get;
			set;
		}

		public UXButtonClickedDelegate onClickFunction
		{
			get;
			set;
		}

		public string planet
		{
			get;
			set;
		}

		public Color textColor
		{
			get;
			set;
		}

		public Color bgColor
		{
			get;
			set;
		}
	}
}
