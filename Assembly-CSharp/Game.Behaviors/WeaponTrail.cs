using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game.Behaviors
{
	public class WeaponTrail : MonoBehaviour
	{
		public class Point
		{
			public float TimeCreated;

			public Vector3 BasePosition;

			public Vector3 TipPosition;

			public bool LineBreak;
		}

		public delegate Vector3 ToVector3<T>(T v);

		public delegate float InterpolateFunction(float a, float b, float c, float d);

		[SerializeField]
		private bool emit = true;

		[SerializeField]
		private Material material;

		[SerializeField]
		private float lifeTime = 1f;

		[SerializeField]
		private Color[] colors;

		[SerializeField]
		private float[] sizes;

		[SerializeField]
		private float minVertexDistance = 0.2f;

		[SerializeField]
		private float maxVertexDistance = 2f;

		[SerializeField]
		private float maxAngle = 10f;

		[SerializeField]
		private int subdivisions = 4;

		[SerializeField]
		private Transform weaponBase;

		[SerializeField]
		private Transform weaponTip;

		private List<WeaponTrail.Point> points = new List<WeaponTrail.Point>();

		private List<WeaponTrail.Point> smoothedPoints = new List<WeaponTrail.Point>();

		private GameObject trailGO;

		private Mesh trailMesh;

		private Vector3 lastPosition;

		private bool lastFrameEmit = true;

		private bool started;

		private const string TRAIL_OBJECT_NAME = "Trail";

		private const string TRAIL_MESH_NAME_SUFFIX = "TrailMesh";

		[CompilerGenerated]
		private static WeaponTrail.ToVector3<Transform> <>f__mg$cache0;

		[CompilerGenerated]
		private static WeaponTrail.ToVector3<Vector3> <>f__mg$cache1;

		public bool Emit
		{
			set
			{
				this.emit = value;
			}
		}

		private void Start()
		{
			this.started = true;
			this.lastPosition = base.transform.position;
			this.trailGO = new GameObject("Trail");
			this.trailGO.transform.parent = null;
			this.trailGO.transform.position = Vector3.zero;
			this.trailGO.transform.rotation = Quaternion.identity;
			this.trailGO.transform.localScale = Vector3.one;
			this.trailGO.AddComponent(typeof(MeshFilter));
			this.trailGO.AddComponent(typeof(MeshRenderer));
			this.trailGO.GetComponent<Renderer>().sharedMaterial = this.material;
			this.trailMesh = new Mesh();
			this.trailMesh.name = base.name + "TrailMesh";
			this.trailGO.GetComponent<MeshFilter>().sharedMesh = this.trailMesh;
		}

		public void Restart()
		{
			if (this.trailGO == null && this.started)
			{
				this.lastPosition = base.transform.position;
				this.trailGO = new GameObject("Trail");
				this.trailGO.transform.parent = null;
				this.trailGO.transform.position = Vector3.zero;
				this.trailGO.transform.rotation = Quaternion.identity;
				this.trailGO.transform.localScale = Vector3.one;
				this.trailGO.AddComponent(typeof(MeshFilter));
				this.trailGO.AddComponent(typeof(MeshRenderer));
				this.trailGO.GetComponent<Renderer>().sharedMaterial = this.material;
				this.trailMesh.name = base.name + "TrailMesh";
				this.trailGO.GetComponent<MeshFilter>().sharedMesh = this.trailMesh;
			}
		}

		public void OnDisable()
		{
			UnityEngine.Object.Destroy(this.trailGO);
			this.trailGO = null;
		}

		public void ChangeLifeTime(float time)
		{
			this.lifeTime = time;
		}

		public void LateUpdate()
		{
			if (!Camera.main)
			{
				return;
			}
			float magnitude = (this.lastPosition - base.transform.position).magnitude;
			if (this.emit)
			{
				if (magnitude > this.minVertexDistance)
				{
					bool flag = false;
					if (this.points.Count < 3)
					{
						flag = true;
					}
					else
					{
						Vector3 from = this.points[this.points.Count - 2].TipPosition - this.points[this.points.Count - 3].TipPosition;
						Vector3 to = this.points[this.points.Count - 1].TipPosition - this.points[this.points.Count - 2].TipPosition;
						if (Vector3.Angle(from, to) > this.maxAngle || magnitude > this.maxVertexDistance)
						{
							flag = true;
						}
					}
					if (flag)
					{
						WeaponTrail.Point point = new WeaponTrail.Point();
						point.BasePosition = this.weaponBase.position;
						point.TipPosition = this.weaponTip.position;
						point.TimeCreated = Time.time;
						this.points.Add(point);
						this.lastPosition = base.transform.position;
						if (this.points.Count == 1)
						{
							this.smoothedPoints.Add(point);
						}
						else if (this.points.Count > 1)
						{
							for (int i = 0; i < 1 + this.subdivisions; i++)
							{
								this.smoothedPoints.Add(point);
							}
						}
						if (this.points.Count >= 4)
						{
							IEnumerable<Vector3> collection = WeaponTrail.NewCatmullRom(new Vector3[]
							{
								this.points[this.points.Count - 4].TipPosition,
								this.points[this.points.Count - 3].TipPosition,
								this.points[this.points.Count - 2].TipPosition,
								this.points[this.points.Count - 1].TipPosition
							}, this.subdivisions, false);
							IEnumerable<Vector3> collection2 = WeaponTrail.NewCatmullRom(new Vector3[]
							{
								this.points[this.points.Count - 4].BasePosition,
								this.points[this.points.Count - 3].BasePosition,
								this.points[this.points.Count - 2].BasePosition,
								this.points[this.points.Count - 1].BasePosition
							}, this.subdivisions, false);
							List<Vector3> list = new List<Vector3>(collection);
							List<Vector3> list2 = new List<Vector3>(collection2);
							float timeCreated = this.points[this.points.Count - 4].TimeCreated;
							float timeCreated2 = this.points[this.points.Count - 1].TimeCreated;
							for (int j = 0; j < list.Count; j++)
							{
								int num = this.smoothedPoints.Count - (list.Count - j);
								if (num > -1 && num < this.smoothedPoints.Count)
								{
									WeaponTrail.Point point2 = new WeaponTrail.Point();
									point2.BasePosition = list2[j];
									point2.TipPosition = list[j];
									point2.TimeCreated = Mathf.Lerp(timeCreated, timeCreated2, (float)j / (float)list.Count);
									this.smoothedPoints[num] = point2;
								}
							}
						}
					}
					else
					{
						this.points[this.points.Count - 1].BasePosition = this.weaponBase.position;
						this.points[this.points.Count - 1].TipPosition = this.weaponTip.position;
						this.smoothedPoints[this.smoothedPoints.Count - 1].BasePosition = this.weaponBase.position;
						this.smoothedPoints[this.smoothedPoints.Count - 1].TipPosition = this.weaponTip.position;
					}
				}
				else
				{
					if (this.points.Count > 0)
					{
						this.points[this.points.Count - 1].BasePosition = this.weaponBase.position;
						this.points[this.points.Count - 1].TipPosition = this.weaponTip.position;
					}
					if (this.smoothedPoints.Count > 0)
					{
						this.smoothedPoints[this.smoothedPoints.Count - 1].BasePosition = this.weaponBase.position;
						this.smoothedPoints[this.smoothedPoints.Count - 1].TipPosition = this.weaponTip.position;
					}
				}
			}
			if (!this.emit && this.lastFrameEmit && this.points.Count > 0)
			{
				this.points[this.points.Count - 1].LineBreak = true;
			}
			this.lastFrameEmit = this.emit;
			List<WeaponTrail.Point> list3 = new List<WeaponTrail.Point>();
			foreach (WeaponTrail.Point current in this.points)
			{
				if (Time.time - current.TimeCreated > this.lifeTime)
				{
					list3.Add(current);
				}
			}
			foreach (WeaponTrail.Point current2 in list3)
			{
				this.points.Remove(current2);
			}
			list3 = new List<WeaponTrail.Point>();
			foreach (WeaponTrail.Point current3 in this.smoothedPoints)
			{
				if (Time.time - current3.TimeCreated > this.lifeTime)
				{
					list3.Add(current3);
				}
			}
			foreach (WeaponTrail.Point current4 in list3)
			{
				this.smoothedPoints.Remove(current4);
			}
			List<WeaponTrail.Point> list4 = this.smoothedPoints;
			if (list4.Count > 1)
			{
				Vector3[] array = new Vector3[list4.Count * 2];
				Vector2[] array2 = new Vector2[list4.Count * 2];
				int[] array3 = new int[(list4.Count - 1) * 6];
				Color[] array4 = new Color[list4.Count * 2];
				for (int k = 0; k < list4.Count; k++)
				{
					WeaponTrail.Point point3 = list4[k];
					float num2 = (Time.time - point3.TimeCreated) / this.lifeTime;
					Color color = Color.Lerp(Color.white, Color.clear, num2);
					if (this.colors != null && this.colors.Length > 0)
					{
						float num3 = num2 * (float)(this.colors.Length - 1);
						float num4 = Mathf.Floor(num3);
						float num5 = Mathf.Clamp(Mathf.Ceil(num3), 1f, (float)(this.colors.Length - 1));
						float t = Mathf.InverseLerp(num4, num5, num3);
						if (num4 >= (float)this.colors.Length)
						{
							num4 = (float)(this.colors.Length - 1);
						}
						if (num4 < 0f)
						{
							num4 = 0f;
						}
						if (num5 >= (float)this.colors.Length)
						{
							num5 = (float)(this.colors.Length - 1);
						}
						if (num5 < 0f)
						{
							num5 = 0f;
						}
						color = Color.Lerp(this.colors[(int)num4], this.colors[(int)num5], t);
					}
					float num6 = 0f;
					if (this.sizes != null && this.sizes.Length > 0)
					{
						float num7 = num2 * (float)(this.sizes.Length - 1);
						float num8 = Mathf.Floor(num7);
						float num9 = Mathf.Clamp(Mathf.Ceil(num7), 1f, (float)(this.sizes.Length - 1));
						float t2 = Mathf.InverseLerp(num8, num9, num7);
						if (num8 >= (float)this.sizes.Length)
						{
							num8 = (float)(this.sizes.Length - 1);
						}
						if (num8 < 0f)
						{
							num8 = 0f;
						}
						if (num9 >= (float)this.sizes.Length)
						{
							num9 = (float)(this.sizes.Length - 1);
						}
						if (num9 < 0f)
						{
							num9 = 0f;
						}
						num6 = Mathf.Lerp(this.sizes[(int)num8], this.sizes[(int)num9], t2);
					}
					Vector3 a = point3.TipPosition - point3.BasePosition;
					array[k * 2] = point3.BasePosition - a * (num6 * 0.5f);
					array[k * 2 + 1] = point3.TipPosition + a * (num6 * 0.5f);
					array4[k * 2] = (array4[k * 2 + 1] = color);
					float x = (float)k / (float)list4.Count;
					array2[k * 2] = new Vector2(x, 0f);
					array2[k * 2 + 1] = new Vector2(x, 1f);
					if (k > 0)
					{
						array3[(k - 1) * 6] = k * 2 - 2;
						array3[(k - 1) * 6 + 1] = k * 2 - 1;
						array3[(k - 1) * 6 + 2] = k * 2;
						array3[(k - 1) * 6 + 3] = k * 2 + 1;
						array3[(k - 1) * 6 + 4] = k * 2;
						array3[(k - 1) * 6 + 5] = k * 2 - 1;
					}
				}
				this.trailMesh.Clear();
				this.trailMesh.vertices = array;
				this.trailMesh.colors = array4;
				this.trailMesh.uv = array2;
				this.trailMesh.triangles = array3;
			}
			else
			{
				this.trailMesh.Clear();
			}
		}

		private static Vector3 CastVector3Func(Vector3 v)
		{
			return v;
		}

		private static Vector3 TransformDotPosition(Transform t)
		{
			return t.position;
		}

		public static IEnumerable<Vector3> NewCatmullRom(Transform[] nodes, int slices, bool loop)
		{
			if (WeaponTrail.<>f__mg$cache0 == null)
			{
				WeaponTrail.<>f__mg$cache0 = new WeaponTrail.ToVector3<Transform>(WeaponTrail.TransformDotPosition);
			}
			return WeaponTrail.NewCatmullRom<Transform>(nodes, WeaponTrail.<>f__mg$cache0, slices, loop);
		}

		public static IEnumerable<Vector3> NewCatmullRom(Vector3[] points, int slices, bool loop)
		{
			if (WeaponTrail.<>f__mg$cache1 == null)
			{
				WeaponTrail.<>f__mg$cache1 = new WeaponTrail.ToVector3<Vector3>(WeaponTrail.CastVector3Func);
			}
			return WeaponTrail.NewCatmullRom<Vector3>(points, WeaponTrail.<>f__mg$cache1, slices, loop);
		}

		[DebuggerHidden]
		private static IEnumerable<Vector3> NewCatmullRom<T>(IList nodes, WeaponTrail.ToVector3<T> toVector3, int slices, bool loop)
		{
			if (nodes.Count >= 2)
			{
				yield return toVector3((T)((object)nodes[0]));
				int num = nodes.Count - 1;
				int num2 = 0;
				while (loop || num2 < num)
				{
					if (loop && num2 > num)
					{
						num2 = 0;
					}
					int index = (num2 != 0) ? (num2 - 1) : ((!loop) ? num2 : num);
					int index2 = num2;
					int num3 = (num2 != num) ? (num2 + 1) : ((!loop) ? num2 : 0);
					int index3 = (num3 != num) ? (num3 + 1) : ((!loop) ? num3 : 0);
					int num4 = slices + 1;
					for (int i = 1; i <= num4; i++)
					{
						yield return WeaponTrail.CatmullRom(toVector3((T)((object)nodes[index])), toVector3((T)((object)nodes[index2])), toVector3((T)((object)nodes[num3])), toVector3((T)((object)nodes[index3])), (float)i, (float)num4);
					}
					num2++;
				}
			}
			yield break;
		}

		private static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, float elapsedTime, float duration)
		{
			float num = elapsedTime / duration;
			float num2 = num * num;
			float num3 = num2 * num;
			return previous * (-0.5f * num3 + num2 - 0.5f * num) + start * (1.5f * num3 + -2.5f * num2 + 1f) + end * (-1.5f * num3 + 2f * num2 + 0.5f * num) + next * (0.5f * num3 - 0.5f * num2);
		}
	}
}
