using StaRTS.Assets;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class LandingTakeOffEffectAnim : IViewFrameTimeObserver
	{
		private const float EFFECT_POS_Y = 1.1f;

		private const string CENTER_OF_MASS = "center_of_mass";

		public AssetHandle LandingHandle;

		public AssetHandle TakeoffHandle;

		public GameObject ShipCenterOfMass
		{
			get;
			set;
		}

		public GameObject LandingEffect
		{
			get;
			set;
		}

		public GameObject TakeOffEffect
		{
			get;
			set;
		}

		public LandingTakeOffEffectAnim(GameObject ship)
		{
			AssetMeshDataMonoBehaviour component = ship.GetComponent<AssetMeshDataMonoBehaviour>();
			if (component == null)
			{
				return;
			}
			if (component.OtherGameObjects != null)
			{
				int i = 0;
				int count = component.OtherGameObjects.Count;
				while (i < count)
				{
					if (component.OtherGameObjects[i].name.Contains("center_of_mass"))
					{
						this.ShipCenterOfMass = component.OtherGameObjects[i];
						break;
					}
					i++;
				}
			}
		}

		public void OnViewFrameTime(float dt)
		{
			if (this.ShipCenterOfMass != null)
			{
				Transform transform = this.ShipCenterOfMass.transform;
				Vector3 position = new Vector3(transform.position.x, 1.1f, transform.position.z);
				Quaternion rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
				if (this.LandingEffect != null)
				{
					Transform transform2 = this.LandingEffect.transform;
					transform2.position = position;
					transform2.rotation = rotation;
				}
				if (this.TakeOffEffect != null)
				{
					Transform transform3 = this.TakeOffEffect.transform;
					transform3.position = position;
					transform3.rotation = rotation;
				}
			}
		}
	}
}
