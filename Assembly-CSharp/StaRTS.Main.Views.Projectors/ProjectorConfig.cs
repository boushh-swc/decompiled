using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Projectors
{
	public class ProjectorConfig
	{
		public string AssetName;

		public string[] AttachmentAssets;

		public Vector3 CameraPosition;

		public Vector3 CameraInterest;

		public bool closeup;

		public int SnapshotFrameDelay = 2;

		public AnimationPreference AnimPreference;

		public string AnimationName;

		public AnimState AnimState;

		public bool Metered;

		public float MeterValue;

		public bool Outline;

		public float OutlineOuter = 0.01f;

		public float OutlineInner = 0.01f;

		public Color Tint = Color.white;

		public string TrackerName;

		public float IconRotationSpeed;

		public UXSprite FrameSprite;

		public float Sharpness = 1f;

		public Action<RenderTexture, ProjectorConfig> RenderCallback;

		public float RenderWidth;

		public float RenderHeight;

		public bool AssetReady;

		public GameObject MainAsset;

		public string buildingEquipmentShaderName;

		public bool EnableCrateHoloShaderSwap;

		public bool IsEquivalentTo(ProjectorConfig config)
		{
			if (config == null)
			{
				return false;
			}
			if (this.AssetName != config.AssetName)
			{
				return false;
			}
			if (this.AttachmentAssets != null && config.AttachmentAssets != null)
			{
				if (this.AttachmentAssets.Length != config.AttachmentAssets.Length)
				{
					return false;
				}
				for (int i = 0; i < this.AttachmentAssets.Length; i++)
				{
					if (this.AttachmentAssets[i] != config.AttachmentAssets[i])
					{
						return false;
					}
				}
			}
			return !(this.CameraPosition != config.CameraPosition) && !(this.CameraInterest != config.CameraInterest) && this.closeup == config.closeup && this.SnapshotFrameDelay == config.SnapshotFrameDelay && this.AnimPreference == config.AnimPreference && !(this.AnimationName != config.AnimationName) && this.AnimState == config.AnimState && this.Metered == config.Metered && this.MeterValue == config.MeterValue && this.Outline == config.Outline && this.OutlineOuter == config.OutlineOuter && this.OutlineInner == config.OutlineInner && !(this.TrackerName != config.TrackerName) && this.FrameSprite == config.FrameSprite && this.Sharpness == config.Sharpness && this.RenderWidth == config.RenderWidth && this.RenderHeight == config.RenderHeight && !(this.buildingEquipmentShaderName != config.buildingEquipmentShaderName) && !(this.Tint != config.Tint);
		}

		public void MakeEquivalentTo(ProjectorConfig config)
		{
			if (config == null)
			{
				return;
			}
			this.AssetName = config.AssetName;
			this.AttachmentAssets = config.AttachmentAssets;
			this.CameraPosition = config.CameraPosition;
			this.CameraInterest = config.CameraInterest;
			this.closeup = config.closeup;
			this.SnapshotFrameDelay = config.SnapshotFrameDelay;
			this.AnimPreference = config.AnimPreference;
			this.AnimationName = config.AnimationName;
			this.AnimState = config.AnimState;
			this.Metered = config.Metered;
			this.MeterValue = config.MeterValue;
			this.Outline = config.Outline;
			this.OutlineOuter = config.OutlineOuter;
			this.OutlineInner = config.OutlineInner;
			this.TrackerName = config.TrackerName;
			this.FrameSprite = config.FrameSprite;
			this.Sharpness = config.Sharpness;
			this.RenderWidth = config.RenderWidth;
			this.RenderHeight = config.RenderHeight;
			this.buildingEquipmentShaderName = config.buildingEquipmentShaderName;
			this.Tint = config.Tint;
		}

		public void Destroy()
		{
			this.FrameSprite = null;
			this.RenderCallback = null;
			this.MainAsset = null;
		}
	}
}
