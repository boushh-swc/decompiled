using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers
{
	public class StickerController : LimitedEditionItemControllerBase
	{
		public StickerController()
		{
			Service.StickerController = this;
		}

		protected override void UpdateValidItems()
		{
			base.UpdateValidItems<StickerVO>();
		}

		public string GetFactionBasedTextureAsset(StickerVO stickerVO)
		{
			if (Service.CurrentPlayer.Faction == FactionType.Empire)
			{
				return stickerVO.TextureOverrideAssetNameEmpire;
			}
			return stickerVO.TextureOverrideAssetNameRebel;
		}

		public StickerVO GetStoreStickerToDisplay(StickerType type)
		{
			StickerVO stickerVO = null;
			this.UpdateValidItems();
			foreach (ILimitedEditionItemVO current in base.ValidLEIs)
			{
				StickerVO stickerVO2 = (StickerVO)current;
				if (stickerVO2.Type == type && (stickerVO == null || stickerVO2.Priority < stickerVO.Priority))
				{
					stickerVO = stickerVO2;
				}
			}
			return stickerVO;
		}
	}
}
