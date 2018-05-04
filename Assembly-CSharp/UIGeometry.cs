using System;
using System.Collections.Generic;
using UnityEngine;

public class UIGeometry
{
	public delegate void OnCustomWrite(List<Vector3> v, List<Vector2> u, List<Color> c, List<Vector3> n, List<Vector4> t, List<Vector4> u2);

	public List<Vector3> verts = new List<Vector3>();

	public List<Vector2> uvs = new List<Vector2>();

	public List<Color> cols = new List<Color>();

	public UIGeometry.OnCustomWrite onCustomWrite;

	private List<Vector3> mRtpVerts = new List<Vector3>();

	private Vector3 mRtpNormal;

	private Vector4 mRtpTan;

	public bool hasVertices
	{
		get
		{
			return this.verts.Count > 0;
		}
	}

	public bool hasTransformed
	{
		get
		{
			return this.mRtpVerts != null && this.mRtpVerts.Count > 0 && this.mRtpVerts.Count == this.verts.Count;
		}
	}

	public void Clear()
	{
		this.verts.Clear();
		this.uvs.Clear();
		this.cols.Clear();
		this.mRtpVerts.Clear();
	}

	public void ApplyTransform(Matrix4x4 widgetToPanel, bool generateNormals = true)
	{
		if (this.verts.Count > 0)
		{
			this.mRtpVerts.Clear();
			int i = 0;
			int count = this.verts.Count;
			while (i < count)
			{
				this.mRtpVerts.Add(widgetToPanel.MultiplyPoint3x4(this.verts[i]));
				i++;
			}
			if (generateNormals)
			{
				this.mRtpNormal = widgetToPanel.MultiplyVector(Vector3.back).normalized;
				Vector3 normalized = widgetToPanel.MultiplyVector(Vector3.right).normalized;
				this.mRtpTan = new Vector4(normalized.x, normalized.y, normalized.z, -1f);
			}
		}
		else
		{
			this.mRtpVerts.Clear();
		}
	}

	public void WriteToBuffers(List<Vector3> v, List<Vector2> u, List<Color> c, List<Vector3> n, List<Vector4> t, List<Vector4> u2)
	{
		if (this.mRtpVerts != null && this.mRtpVerts.Count > 0)
		{
			if (n == null)
			{
				int i = 0;
				int count = this.mRtpVerts.Count;
				while (i < count)
				{
					v.Add(this.mRtpVerts[i]);
					u.Add(this.uvs[i]);
					c.Add(this.cols[i]);
					i++;
				}
			}
			else
			{
				int j = 0;
				int count2 = this.mRtpVerts.Count;
				while (j < count2)
				{
					v.Add(this.mRtpVerts[j]);
					u.Add(this.uvs[j]);
					c.Add(this.cols[j]);
					n.Add(this.mRtpNormal);
					t.Add(this.mRtpTan);
					j++;
				}
			}
			if (u2 != null)
			{
				Vector4 zero = Vector4.zero;
				int k = 0;
				int count3 = this.verts.Count;
				while (k < count3)
				{
					zero.x = this.verts[k].x;
					zero.y = this.verts[k].y;
					u2.Add(zero);
					k++;
				}
			}
			if (this.onCustomWrite != null)
			{
				this.onCustomWrite(v, u, c, n, t, u2);
			}
		}
	}
}
