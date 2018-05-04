using System;
using UnityEngine;

namespace StaRTS.Main.Views.Entities
{
	public class OutlinedAsset : ShaderSwappedAsset
	{
		private const string PARAM_COLOR = "_OutlineColor";

		private const string PARAM_WIDTH = "_Outline";

		public void Init(GameObject gameObj)
		{
			base.Init(gameObj, "Outline_Unlit");
		}

		public void SetOutlineColor(Color color)
		{
			if (base.EnsureMaterials())
			{
				int i = 0;
				int count = this.shaderSwappedMaterials.Count;
				while (i < count)
				{
					this.SetColor(i, color);
					i++;
				}
			}
		}

		public void RemoveOutline()
		{
			base.RemoveAppliedShader();
		}

		private void SetColor(int i, Color color)
		{
			this.shaderSwappedMaterials[i].SetColor("_OutlineColor", color);
		}

		public void SetOutlineWidth(float width)
		{
			if (this.shaderSwappedMaterials == null)
			{
				return;
			}
			int i = 0;
			int count = this.shaderSwappedMaterials.Count;
			while (i < count)
			{
				this.shaderSwappedMaterials[i].SetFloat("_Outline", width);
				i++;
			}
		}
	}
}
