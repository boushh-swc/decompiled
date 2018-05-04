using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers
{
	public class TrapViewController : IViewFrameTimeObserver
	{
		public const string TRAP_ANIM_STATE = "trapAnimState";

		public const int TRAP_LAYER_HOME = 1;

		public const int TRAP_LAYER_BATTLE = 0;

		private bool observing;

		public TrapViewController()
		{
			Service.TrapViewController = this;
		}

		public void UpdateTrapVisibility(SmartEntity entity)
		{
			if (entity.TrapViewComp == null)
			{
				return;
			}
			TrapViewComponent trapViewComp = entity.TrapViewComp;
			IState currentState = Service.GameStateMachine.CurrentState;
			bool flag = currentState is BattleStartState || currentState is BattlePlayState || currentState is BattlePlaybackState || currentState is BattleEndState;
			bool flag2 = GameUtils.IsVisitingBase();
			bool flag3 = TrapUtils.IsCurrentPlayerInDefensiveBattle(currentState);
			bool flag4 = (flag && !flag3) || flag2;
			trapViewComp.Anim.SetLayerWeight(1, (float)((!flag4) ? 1 : 0));
			trapViewComp.Anim.SetLayerWeight(0, (float)((!flag4) ? 0 : 1));
			bool active = false;
			if (!flag2)
			{
				switch (entity.TrapComp.CurrentState)
				{
				case TrapState.Spent:
					active = (!flag || flag3 || entity.TrapComp.PreviousState == TrapState.Active);
					break;
				case TrapState.Armed:
					active = (!flag || flag3);
					break;
				case TrapState.Active:
					active = true;
					break;
				case TrapState.Destroyed:
					active = flag;
					break;
				}
			}
			trapViewComp.Contents.SetActive(active);
		}

		public void SetTrapViewState(TrapViewComponent view, TrapState state)
		{
			if (view != null)
			{
				if (view.PendingStates.Count == 0)
				{
					view.Anim.SetInteger("trapAnimState", (int)state);
				}
				view.PendingStates.Add(state);
				this.RegisterObservers();
			}
		}

		public void OnViewFrameTime(float dt)
		{
			bool flag = true;
			NodeList<TrapNode> trapNodeList = Service.BuildingLookupController.TrapNodeList;
			for (TrapNode trapNode = trapNodeList.Head; trapNode != null; trapNode = trapNode.Next)
			{
				if (trapNode.TrapViewComp.PendingStates.Count > 0)
				{
					flag = false;
					TrapState value = trapNode.TrapViewComp.PendingStates[0];
					trapNode.TrapViewComp.PendingStates.RemoveAt(0);
					trapNode.TrapViewComp.Anim.SetInteger("trapAnimState", (int)value);
				}
			}
			if (flag)
			{
				this.observing = false;
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		public void RegisterObservers()
		{
			if (!this.observing)
			{
				this.observing = true;
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			}
		}
	}
}
