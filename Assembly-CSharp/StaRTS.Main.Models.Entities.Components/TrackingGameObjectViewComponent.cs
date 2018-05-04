using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Models.Entities.Components
{
	public class TrackingGameObjectViewComponent : ComponentBase
	{
		private const float TRACKING_ROTATION = -3.14159274f;

		private const string NOT_APPLICABLE = "n/a";

		private GameObject trackingGameObject;

		public float YawVelocity;

		public float PitchVelocity;

		public float Yaw
		{
			get;
			set;
		}

		public float Pitch
		{
			get;
			set;
		}

		public float Speed
		{
			get;
			set;
		}

		public TrackingGameObjectViewComponent(GameObject gameObject, ITrackerVO trackerVO, TrackingComponent trackingComp)
		{
			if (string.IsNullOrEmpty(trackerVO.TrackerName))
			{
				Service.Logger.Error("TrackerName not set for an entity which has a TrackingGameObjectViewComponent.");
				return;
			}
			Transform transform = gameObject.transform.Find(trackerVO.TrackerName);
			if (transform != null && transform.gameObject != null)
			{
				this.trackingGameObject = transform.gameObject;
			}
			else if (trackerVO.TrackerName != "n/a")
			{
				Service.Logger.ErrorFormat("Turret {0} cannot find a tracking object at {1}", new object[]
				{
					gameObject.name,
					trackerVO.TrackerName
				});
			}
			this.Yaw = trackingComp.TargetYaw;
			this.YawVelocity = 0f;
			if (trackingComp.TrackPitch)
			{
				this.Pitch = trackingComp.TargetPitch;
				this.PitchVelocity = 0f;
			}
			this.Speed = Service.BattlePlaybackController.CurrentPlaybackScale;
		}

		public void YawRotate(float rads)
		{
			if (this.trackingGameObject == null)
			{
				return;
			}
			rads += -3.14159274f;
			float angle = -rads * 57.29578f;
			this.trackingGameObject.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
		}

		public void PitchRotate(float rads)
		{
			if (this.trackingGameObject == null)
			{
				return;
			}
			this.trackingGameObject.transform.Rotate(rads * 57.29578f, 0f, 0f);
		}
	}
}
