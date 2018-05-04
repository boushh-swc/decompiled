using StaRTS.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class FootprintMesh
	{
		private GameObject tiles;

		private Mesh mesh;

		private List<Vector4> quads;

		private string name;

		public FootprintMesh(string name)
		{
			this.name = name;
			this.quads = new List<Vector4>();
		}

		public void DestroyFootprintMesh()
		{
			this.quads = null;
			if (this.mesh != null)
			{
				UnityUtils.DestroyMesh(this.mesh);
				this.mesh = null;
			}
			if (this.tiles != null)
			{
				UnityEngine.Object.Destroy(this.tiles);
				this.tiles = null;
			}
		}

		public void AddQuad(float x, float z, float width, float depth)
		{
			Vector4 item = new Vector4(x, x + width, z, z + depth);
			this.quads.Add(item);
		}

		public void GenerateMeshFromQuads()
		{
			int count = this.quads.Count;
			Vector3[] vertices;
			Vector2[] uv;
			int[] triangles;
			UnityUtils.SetupVerticesForQuads(count, out vertices, out uv, out triangles);
			for (int i = 0; i < count; i++)
			{
				Vector4 vector = this.quads[i];
				float x = vector.x;
				float y = vector.y;
				float z = vector.z;
				float w = vector.w;
				UnityUtils.SetQuadVertices(x, z, y, w, i, vertices);
			}
			this.quads = null;
			this.mesh = UnityUtils.CreateMeshWithVertices(vertices, uv, triangles);
			this.tiles = new GameObject(this.name);
			UnityUtils.SetupMeshMaterial(this.tiles, this.mesh, null);
		}

		public bool ModifyTiles(Vector3 newPosition, Material newMaterial)
		{
			bool result = false;
			Transform transform = this.tiles.transform;
			Vector3 position = transform.position;
			if (newPosition != position)
			{
				transform.position = newPosition;
				result = true;
			}
			Renderer component = this.tiles.GetComponent<Renderer>();
			Material sharedMaterial = component.sharedMaterial;
			if (newMaterial != sharedMaterial)
			{
				component.sharedMaterial = newMaterial;
				result = true;
			}
			return result;
		}
	}
}
