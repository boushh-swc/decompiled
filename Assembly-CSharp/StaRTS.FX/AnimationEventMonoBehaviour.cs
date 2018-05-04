using StaRTS.Main.Models.Entities;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.FX
{
	public class AnimationEventMonoBehaviour : MonoBehaviour
	{
		private EntityRef entityRefMB;

		public void Start()
		{
			this.entityRefMB = base.gameObject.GetComponent<EntityRef>();
		}

		public void AnimationEvent(string message)
		{
			if (Service.AnimationEventManager == null)
			{
				return;
			}
			Service.AnimationEventManager.ProcessAnimationEvent(this.entityRefMB, message);
		}
	}
}
