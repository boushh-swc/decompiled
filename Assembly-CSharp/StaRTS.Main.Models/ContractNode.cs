using StaRTS.DataStructures.PriorityQueue;
using StaRTS.Main.Models.Player.World;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models
{
	internal class ContractNode : PriorityQueueNode
	{
		public Contract Contract
		{
			get;
			private set;
		}

		public uint StartServerTime
		{
			get;
			private set;
		}

		public Building BuildingTO
		{
			get;
			private set;
		}

		public LinkedListNode<ContractNode> NextNode
		{
			get;
			set;
		}

		public ContractNode(Contract contract, uint startServerTime, Building buildingTO)
		{
			this.Contract = contract;
			this.StartServerTime = startServerTime;
			this.BuildingTO = buildingTO;
		}
	}
}
