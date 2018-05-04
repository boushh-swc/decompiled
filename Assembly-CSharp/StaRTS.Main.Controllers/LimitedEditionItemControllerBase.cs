using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public abstract class LimitedEditionItemControllerBase : IEventObserver
	{
		private uint nextUpdateTimer;

		public List<ILimitedEditionItemVO> ValidLEIs
		{
			get;
			private set;
		}

		public LimitedEditionItemControllerBase()
		{
			this.ValidLEIs = new List<ILimitedEditionItemVO>();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.GameStateChanged);
		}

		private Dictionary<string, T>.ValueCollection GetAllItems<T>() where T : ILimitedEditionItemVO
		{
			StaticDataController staticDataController = Service.StaticDataController;
			return staticDataController.GetAll<T>();
		}

		protected abstract void UpdateValidItems();

		public void UpdateValidItems<T>() where T : ILimitedEditionItemVO
		{
			this.ValidLEIs.Clear();
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			int serverTime = (int)Service.ServerAPI.ServerTime;
			Dictionary<string, T>.ValueCollection allItems = this.GetAllItems<T>();
			int num = -1;
			foreach (T current in allItems)
			{
				int num2 = current.EndTime - serverTime;
				if (num2 >= 0)
				{
					if (num == -1 || num2 < num)
					{
						num = num2;
					}
					int num3 = current.StartTime - serverTime;
					if (num3 > 0)
					{
						if (num == -1 || num3 < num)
						{
							num = num3;
						}
					}
					else if (this.IsValidForPlayer(current, currentPlayer))
					{
						this.ValidLEIs.Add(current);
					}
				}
			}
			if (num != -1 && (long)num < 432000L)
			{
				this.nextUpdateTimer = Service.ViewTimerManager.CreateViewTimer((float)num, false, new TimerDelegate(this.UpdateValidItems), null);
			}
		}

		public void UpdateValidItems(uint id, object cookie)
		{
			this.UpdateValidItems();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.GameStateChanged)
			{
				if (id == EventId.ContractCompleted)
				{
					this.UpdateValidItems();
				}
			}
			else
			{
				IGameState gameState = (IGameState)Service.GameStateMachine.CurrentState;
				if (gameState != null && gameState.CanUpdateHomeContracts())
				{
					this.StartUpdating();
				}
				else
				{
					this.StopUpdating();
				}
			}
			return EatResponse.NotEaten;
		}

		private void StartUpdating()
		{
			this.UpdateValidItems();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ContractCompleted);
		}

		private void StopUpdating()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.ContractCompleted);
			if (this.nextUpdateTimer != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.nextUpdateTimer);
				this.nextUpdateTimer = 0u;
			}
		}

		protected virtual bool IsValidForPlayer(ILimitedEditionItemVO vo, CurrentPlayer player)
		{
			return CrateUtils.AllConditionsMet(vo.AudienceConditions);
		}
	}
}
