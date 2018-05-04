using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.Entities
{
	public class LinearSpline
	{
		private float speedBase;

		private List<Vector3> positions;

		private List<Quaternion> rotations;

		private List<float> speeds;

		private List<float> delays;

		private List<float> distances;

		private List<WaypointReached> callbacks;

		private List<object> cookies;

		private int curInterval;

		private float curTime;

		private float curIntervalStartTime;

		public LinearSpline(float speed)
		{
			this.positions = new List<Vector3>();
			this.rotations = new List<Quaternion>();
			this.speeds = new List<float>();
			this.delays = new List<float>();
			this.distances = new List<float>();
			this.callbacks = new List<WaypointReached>();
			this.cookies = new List<object>();
			this.speedBase = 3f * speed;
		}

		public void AddWayPoint(Vector3 position, Quaternion rotation, float speedRatio, float delay, WaypointReached callback, object cookie)
		{
			this.positions.Add(position);
			this.rotations.Add(rotation);
			this.speeds.Add(this.speedBase * speedRatio);
			this.delays.Add(delay);
			this.callbacks.Add(callback);
			this.cookies.Add(cookie);
			int count = this.positions.Count;
			if (count > 1)
			{
				this.distances.Add(Vector3.Distance(this.positions[count - 1], this.positions[count - 2]));
			}
		}

		public void AddWayPoint(Vector3 position, Quaternion rotation, float speedRatio, float delay)
		{
			this.AddWayPoint(position, rotation, speedRatio, delay, null, null);
		}

		public void AddWayPoint(Vector3 position, Quaternion rotation)
		{
			this.AddWayPoint(position, rotation, 1f, 0f);
		}

		public void Start()
		{
			this.curTime = 0f;
			this.curInterval = 0;
			this.curIntervalStartTime = 0f;
			this.positions.Clear();
			this.rotations.Clear();
			this.delays.Clear();
			this.distances.Clear();
			this.callbacks.Clear();
			this.cookies.Clear();
		}

		private bool advanceTime(float dt, out float curDistance)
		{
			this.curTime += dt;
			curDistance = (this.curTime - this.curIntervalStartTime) * this.speeds[this.curInterval];
			int index = this.curInterval + 1;
			while (this.curInterval < this.positions.Count - 1 && curDistance > this.distances[this.curInterval] + this.delays[index] * this.speeds[this.curInterval])
			{
				this.curIntervalStartTime += this.distances[this.curInterval] / this.speeds[this.curInterval] + this.delays[index];
				this.curInterval = index++;
				curDistance = (this.curTime - this.curIntervalStartTime) * this.speeds[this.curInterval];
				if (this.callbacks[this.curInterval] != null)
				{
					this.callbacks[this.curInterval](this.cookies[this.curInterval]);
					this.callbacks[this.curInterval] = null;
					this.cookies[this.curInterval] = null;
				}
			}
			if (this.curInterval < this.positions.Count - 1)
			{
				if (curDistance >= this.distances[this.curInterval])
				{
					curDistance = this.distances[this.curInterval];
				}
				return true;
			}
			return false;
		}

		public bool Update(float dt, out Vector3 position, out Quaternion rotation)
		{
			float num;
			if (this.advanceTime(dt, out num))
			{
				float t = num / this.distances[this.curInterval];
				position = Vector3.Lerp(this.positions[this.curInterval], this.positions[this.curInterval + 1], t);
				rotation = Quaternion.Slerp(this.rotations[this.curInterval], this.rotations[this.curInterval + 1], t);
				return false;
			}
			position = this.positions[this.positions.Count - 1];
			rotation = this.rotations[this.rotations.Count - 1];
			return true;
		}
	}
}
