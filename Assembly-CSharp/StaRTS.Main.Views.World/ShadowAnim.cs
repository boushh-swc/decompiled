using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class ShadowAnim : IViewFrameTimeObserver
	{
		private const float FADE_FACTOR = 15f;

		private const float SCALE_FACTOR = 50f;

		private const float SHADOW_POS_Y = 1.1f;

		private const float SHADOW_MIN_ALPHA = 0.1f;

		private const string CENTER_OF_MASS = "center_of_mass";

		private bool setup;

		private bool playing;

		private float shadowYPosition;

		public GameObject CenterOfMass
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

		public ShadowAnim(GameObject gameObject)
		{
			this.setup = false;
			this.playing = false;
			this.shadowYPosition = 1.1f;
			this.EnsureShadowAnimSetup(gameObject);
		}

		public ShadowAnim(GameObject gameObject, float yPosition) : this(gameObject)
		{
			this.shadowYPosition = yPosition;
		}

		public void EnsureShadowAnimSetup(GameObject gameObject)
		{
			if (this.setup || gameObject == null)
			{
				return;
			}
			AssetMeshDataMonoBehaviour component = gameObject.GetComponent<AssetMeshDataMonoBehaviour>();
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
						this.CenterOfMass = component.OtherGameObjects[i];
						break;
					}
					i++;
				}
			}
			if (component.ShadowGameObject != null)
			{
				this.ShadowGameObject = component.ShadowGameObject;
				this.ShadowMaterial = UnityUtils.EnsureMaterialCopy(this.ShadowGameObject.GetComponent<Renderer>());
				this.ShadowMaterial.shader = Service.AssetManager.Shaders.GetShader("TransportShadow");
				this.ShadowMaterial.color = new Color(0f, 0f, 0f, 0f);
			}
			this.setup = true;
			if (this.playing)
			{
				this.PlayShadowAnim(true);
			}
		}

		public void PlayShadowAnim(bool play)
		{
			if (this.setup)
			{
				if (play)
				{
					Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
				}
				else
				{
					Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
				}
			}
			this.playing = play;
		}

		public void OnViewFrameTime(float dt)
		{
			if (this.CenterOfMass != null)
			{
				Transform transform = this.CenterOfMass.transform;
				Vector3 position = new Vector3(transform.position.x, this.shadowYPosition, transform.position.z);
				Quaternion rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
				if (this.ShadowGameObject != null && this.ShadowMaterial != null)
				{
					float a = Mathf.Clamp(1f - transform.position.y / 15f, 0.1f, 1f);
					float num = 1f + (transform.position.y - 1f) / 50f;
					Transform transform2 = this.ShadowGameObject.transform;
					transform2.position = position;
					transform2.rotation = rotation;
					transform2.localScale = new Vector3(num, 1f, num);
					this.ShadowMaterial.color = new Color(0f, 0f, 0f, a);
				}
			}
		}
	}
}
