using StaRTS.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Utils
{
	public class CircleMeshUtils
	{
		public static void AddCircleMesh(GameObject go, float radius, float ringWidth, float uvProp)
		{
			MeshFilter meshFilter = go.GetComponent<MeshFilter>();
			if (meshFilter == null)
			{
				meshFilter = go.AddComponent<MeshFilter>();
			}
			if (go.GetComponent<MeshRenderer>() == null)
			{
				go.AddComponent<MeshRenderer>();
			}
			Mesh mesh = meshFilter.sharedMesh;
			if (mesh == null)
			{
				mesh = UnityUtils.CreateMesh();
				mesh.name = "Mesh_" + go.name;
				meshFilter.sharedMesh = mesh;
			}
			mesh.Clear();
			int num = Math.Max(16, 2 * (int)(radius + ringWidth));
			float num2 = 360f / (float)num;
			int num3 = num;
			if (num3 < 2)
			{
				return;
			}
			List<Vector3> list = new List<Vector3>();
			List<int> list2 = new List<int>();
			List<Vector3> list3 = new List<Vector3>();
			List<Vector2> list4 = new List<Vector2>();
			Vector3 point = new Vector3(radius, 0f, 0f);
			Vector3 point2 = new Vector3(radius + ringWidth, 0f, 0f);
			Vector3 up = Vector3.up;
			Vector2 item = new Vector2(0f, 0f);
			Vector2 item2 = new Vector2(1f, 0f);
			Vector2 item3 = new Vector2(0f, uvProp);
			Vector2 item4 = new Vector2(1f, uvProp);
			Quaternion rotation = Quaternion.identity;
			Quaternion rotation2 = Quaternion.identity;
			float num4 = 0f;
			for (int i = 0; i < num3; i++)
			{
				rotation = Quaternion.AngleAxis(num4, -Vector3.up);
				rotation2 = Quaternion.AngleAxis(num4 + num2, -Vector3.up);
				int num5 = i * 4;
				list.Add(rotation * point);
				list.Add(rotation2 * point);
				list.Add(rotation * point2);
				list.Add(rotation2 * point2);
				list3.Add(up);
				list3.Add(up);
				list3.Add(up);
				list3.Add(up);
				list2.Add(0 + num5);
				list2.Add(1 + num5);
				list2.Add(2 + num5);
				list2.Add(1 + num5);
				list2.Add(3 + num5);
				list2.Add(2 + num5);
				list4.Add(item);
				list4.Add(item2);
				list4.Add(item3);
				list4.Add(item4);
				num4 += num2;
			}
			mesh.vertices = list.ToArray();
			mesh.uv = list4.ToArray();
			mesh.triangles = list2.ToArray();
			mesh.normals = list3.ToArray();
			mesh.bounds = new Bounds(Vector3.zero, new Vector3(radius + ringWidth, 0.1f, radius + ringWidth));
		}
	}
}
