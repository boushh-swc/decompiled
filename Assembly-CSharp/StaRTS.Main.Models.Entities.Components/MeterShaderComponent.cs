using Net.RichardLord.Ash.Core;
using StaRTS.Utils;
using System;
using UnityEngine;

namespace StaRTS.Main.Models.Entities.Components
{
	public class MeterShaderComponent : ComponentBase
	{
		private const string METER_SHADER_PROGRESS = "_Progress";

		private GameObject meterObject;

		private Material meterMaterial;

		public float Percentage
		{
			get;
			private set;
		}

		public int FillSize
		{
			get;
			set;
		}

		public MeterShaderComponent(GameObject gameObject)
		{
			this.meterObject = gameObject;
			this.meterMaterial = UnityUtils.EnsureMaterialCopy(this.meterObject.GetComponent<Renderer>());
			this.UpdatePercentage(0f);
			this.FillSize = 0;
		}

		public bool GameObjectEquals(GameObject gameObject)
		{
			return this.meterObject == gameObject;
		}

		public override void OnRemove()
		{
			if (this.meterMaterial != null)
			{
				UnityUtils.DestroyMaterial(this.meterMaterial);
				this.meterMaterial = null;
			}
		}

		public void UpdatePercentage(float percentage)
		{
			percentage = Mathf.Max(0f, percentage);
			percentage = Mathf.Min(1f, percentage);
			this.Percentage = percentage;
			this.meterMaterial.SetFloat("_Progress", this.Percentage);
		}
	}
}
