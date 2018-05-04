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
			case EventId.HeroDeployed:
			case EventId.AddDecalToTroop:
				this.OnHeroDeployed(cookie as Entity);
				return EatResponse.NotEaten;
			case EventId.TroopAbilityActivate:
			case EventId.TroopAbilityDeactivate:
			case EventId.TroopAbilityCoolDownComplete:
			case EventId.ChampionDeployed:
			{
				IL_2E:
				if (id == EventId.TroopViewReady)
				{
					EntityViewParams entityViewParams = cookie as EntityViewParams;
					this.OnHeroOrChampionViewReady(entityViewParams.Entity);
					return EatResponse.NotEaten;
				}
				if (id != EventId.GameStateChanged)
				{
					return EatResponse.NotEaten;
				}
				Type type = (Type)cookie;
				if (type == typeof(BattleEndState) || type == typeof(BattleEndPlaybackState))
				{
					this.CleanupAllDecals();
				}
				return EatResponse.NotEaten;
			}
			case EventId.HeroKilled:
			case EventId.ChampionKilled:
				this.OnHeroOrChampionKilled(cookie as Entity);
				return EatResponse.NotEaten;
			}
			goto IL_2E;
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
