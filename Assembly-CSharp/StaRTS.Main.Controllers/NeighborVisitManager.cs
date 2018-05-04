using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Controllers.World.Transitions;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class NeighborVisitManager
	{
		private bool processing;

		public GamePlayer NeighborPlayer;

		public List<SquadDonatedTroop> NeighborSquadTroops
		{
			get;
			private set;
		}

		public NeighborVisitManager()
		{
			Service.NeighborVisitManager = this;
		}

		public void VisitNeighbor(string neighborId)
		{
			if (this.processing)
			{
				return;
			}
			this.processing = true;
			ProcessingScreen.Show();
			VisitNeighborRequest request = new VisitNeighborRequest(neighborId);
			VisitNeighborCommand visitNeighborCommand = new VisitNeighborCommand(request);
			visitNeighborCommand.AddSuccessCallback(new AbstractCommand<VisitNeighborRequest, VisitNeighborResponse>.OnSuccessCallback(this.OnVisitNeighborSuccess));
			visitNeighborCommand.AddFailureCallback(new AbstractCommand<VisitNeighborRequest, VisitNeighborResponse>.OnFailureCallback(this.OnVisitNeighborFailure));
			Service.ServerAPI.Sync(visitNeighborCommand);
		}

		private void OnVisitNeighborSuccess(VisitNeighborResponse response, object cookie)
		{
			ProcessingScreen.Hide();
			if (response == null)
			{
				this.processing = false;
				return;
			}
			this.NeighborPlayer = new GamePlayer();
			this.NeighborPlayer.PlayerName = response.Name;
			this.NeighborPlayer.Faction = response.Faction;
			this.NeighborPlayer.Map = response.MapData;
			this.NeighborPlayer.Inventory = response.InventoryData;
			this.NeighborPlayer.AttackRating = response.AttackRating;
			this.NeighborPlayer.DefenseRating = response.DefenseRating;
			this.NeighborPlayer.AttacksWon = response.AttacksWon;
			this.NeighborPlayer.DefensesWon = response.DefensesWon;
			this.NeighborPlayer.Squad = response.Squad;
			this.NeighborPlayer.UnlockedLevels = response.UpgradesData;
			this.NeighborSquadTroops = response.SquadTroops;
			NeighborMapDataLoader mapDataLoader = new NeighborMapDataLoader(response);
			IState currentState = Service.GameStateMachine.CurrentState;
			AbstractTransition transition;
			if (currentState is GalaxyState)
			{
				transition = new GalaxyMapToBaseTransition(new NeighborVisitState(), mapDataLoader, new TransitionCompleteDelegate(this.OnTransitionComplete), false, false);
			}
			else if (currentState is WarBoardState)
			{
				transition = new WarboardToWarbaseTransition(new NeighborVisitState(), mapDataLoader, new TransitionCompleteDelegate(this.OnTransitionComplete), false, false);
			}
			else
			{
				transition = new WorldToWorldTransition(new NeighborVisitState(), mapDataLoader, new TransitionCompleteDelegate(this.OnTransitionComplete), false, true);
			}
			Service.WorldTransitioner.StartTransition(transition);
		}

		private void OnVisitNeighborFailure(uint id, object cookie)
		{
			ProcessingScreen.Hide();
		}

		private void OnTransitionComplete()
		{
			this.processing = false;
		}
	}
}
