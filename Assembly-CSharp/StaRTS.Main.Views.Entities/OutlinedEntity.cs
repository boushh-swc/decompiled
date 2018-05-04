using Net.RichardLord.Ash.Core;
using StaRTS.Main.Utils;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Entities
{
	public class OutlinedEntity : ShaderSwappedEntity
	{
		public const string PARAM_COLOR = "_OutlineColor";

		public const string PARAM_WIDTH = "_Outline";

		public OutlinedEntity(Entity entity) : base(entity, "Outline_Unlit")
		{
		}

		protected override void SetMaterialCustomProperties(Material material)
		{
			material.SetColor("_OutlineColor", FXUtils.SELECTION_OUTLINE_COLOR);
			material.SetFloat("_Outline", 0.00125f);
		}
	}
}
