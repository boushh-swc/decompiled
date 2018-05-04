using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.World.Targeting
{
	public class HeroIdentifier : IEventObserver
	{
		private List<HeroDecal> decals;

		public HeroIdentifier()
		{
			this.decals = new List<HeroDecal>();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.HeroDeployed, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.HeroKilled, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.AddDecalToTroop, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ChampionKilled, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.TroopViewReady, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.HeroKilled:
			case EventId.ChampionKilled:
				this.OnHeroOrChampionKilled(cookie as Entity);
				return EatResponse.NotEaten;
			case EventId.ChampionDeployed:
				IL_1C:
				if (id == EventId.TroopViewReady)
				{
					EntityViewParams entityViewParams = cookie as EntityViewParams;
					this.OnHeroOrChampionViewReady(entityViewParams.Entity);
					return EatResponse.NotEaten;
				}
				if (id == EventId.GameStateChanged)
				{
					Type type = (Type)cookie;
					if (type == typeof(BattleEndState) || type == typeof(BattleEndPlaybackState))
					{
						this.CleanupAllDecals();
					}
					return EatResponse.NotEaten;
				}
				if (id != EventId.HeroDeployed)
				{
					return EatResponse.NotEaten;
				}
				goto IL_3F;
			case EventId.AddDecalToTroop:
				goto IL_3F;
			}
			goto IL_1C;
			IL_3F:
			this.OnHeroDeployed(cookie as Entity);
			return EatResponse.NotEaten;
		}

		private HeroDecal FindDecal(Entity entity)
		{
			int i = 0;
			int count = this.decals.Count;
			while (i < count)
			{
				HeroDecal heroDecal = this.decals[i];
				if (heroDecal.Entity == entity)
				{
					return heroDecal;
				}
				i++;
			}
			return null;
		}

		private void OnHeroDeployed(Entity entity)
		{
			this.decals.Add(new HeroDecal(entity));
		}

		private void OnHeroOrChampionViewReady(Entity entity)
		{
			HeroDecal heroDecal = this.FindDecal(entity);
			if (heroDecal != null)
			{
				heroDecal.TrySetupView();
			}
		}

		private void OnHeroOrChampionKilled(Entity entity)
		{
			HeroDecal heroDecal = this.FindDecal(entity);
			if (heroDecal != null)
			{
				heroDecal.FadeToGray();
			}
		}

		private void CleanupAllDecals()
		{
			int i = 0;
			int count = this.decals.Count;
			while (i < count)
			{
				this.decals[i].Cleanup();
				i++;
			}
			this.decals.Clear();
		}
	}
}
