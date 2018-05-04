using Net.RichardLord.Ash.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.Entities.Components
{
	public class TrapViewComponent : ComponentBase
	{
		public Animator Anim
		{
			get;
			private set;
		}

		public Animator TurretAnim
		{
			get;
			set;
		}

		public List<TrapState> PendingStates
		{
			get;
			private set;
		}

		public GameObject Contents
		{
			get
			{
				return this.Anim.transform.GetChild(0).gameObject;
			}
		}

		public TrapViewComponent(Animator anim)
		{
			this.Anim = anim;
			this.PendingStates = new List<TrapState>();
		}
	}
}
