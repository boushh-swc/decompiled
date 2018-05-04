using Net.RichardLord.Ash.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.Entities
{
	public class EntityShaderSwapper
	{
		private List<ShaderSwappedEntity> shaderSwappedEntities;

		public EntityShaderSwapper()
		{
			this.shaderSwappedEntities = new List<ShaderSwappedEntity>();
		}

		public void HighlightForPerk(Entity entity)
		{
			this.ResetToOriginal(entity);
			string shaderName = "PL_2Color_Mask_SA";
			PerkHighlightedEntity item = new PerkHighlightedEntity(entity, shaderName);
			this.shaderSwappedEntities.Add(item);
		}

		public void Outline(Entity entity)
		{
			this.ResetToOriginal(entity);
			OutlinedEntity item = new OutlinedEntity(entity);
			this.shaderSwappedEntities.Add(item);
		}

		public bool ResetToOriginal(Entity entity)
		{
			int num = this.IndexOfEntity(entity);
			if (num >= 0)
			{
				this.shaderSwappedEntities[num].RemoveAppliedShader();
				this.shaderSwappedEntities.RemoveAt(num);
				return true;
			}
			return false;
		}

		private int IndexOfEntity(Entity entity)
		{
			int i = 0;
			int count = this.shaderSwappedEntities.Count;
			while (i < count)
			{
				ShaderSwappedEntity shaderSwappedEntity = this.shaderSwappedEntities[i];
				if (shaderSwappedEntity.Entity == entity)
				{
					return i;
				}
				i++;
			}
			return -1;
		}
	}
}
