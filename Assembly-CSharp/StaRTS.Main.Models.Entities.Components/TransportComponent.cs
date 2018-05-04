using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using System;
using UnityEngine;

namespace StaRTS.Main.Models.Entities.Components
{
	public class TransportComponent : AssetComponent
	{
		public TransportTypeVO TransportType
		{
			get;
			private set;
		}

		public LinearSpline Spline
		{
			get;
			set;
		}

		public GameObject GameObj
		{
			get;
			set;
		}

		public GameObject ShadowGameObject
		{
			get;
			set;
		}

		public Material ShadowMaterial
		{
			get;
			set;
		}

		public TransportComponent(TransportTypeVO transportType) : base(transportType.AssetName)
		{
			this.TransportType = transportType;
			this.Spline = new LinearSpline((float)transportType.MaxSpeed);
			this.GameObj = null;
			this.ShadowGameObject = null;
			this.ShadowMaterial = null;
		}

		public override void OnRemove()
		{
			if (this.ShadowMaterial != null)
			{
				UnityUtils.DestroyMaterial(this.ShadowMaterial);
				this.ShadowMaterial = null;
			}
			if (this.GameObj != null)
			{
				UnityEngine.Object.Destroy(this.GameObj);
				this.GameObj = null;
				this.ShadowGameObject = null;
			}
		}
	}
}
