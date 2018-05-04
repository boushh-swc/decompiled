using StaRTS.GameBoard.Pathfinding.InternalClasses;
using System;

namespace StaRTS.DataStructures.PriorityQueue
{
	public sealed class HeapPriorityQueue
	{
		private int _numNodes;

		private readonly PathfindingCellInfo[] _nodes;

		private int _numNodesEverEnqueued;

		public int Count
		{
			get
			{
				return this._numNodes;
			}
		}

		public int MaxSize
		{
			get
			{
				return this._nodes.Length - 1;
			}
		}

		public PathfindingCellInfo First
		{
			get
			{
				return this._nodes[1];
			}
		}

		public HeapPriorityQueue(int maxNodes)
		{
			this._numNodes = 0;
			this._nodes = new PathfindingCellInfo[maxNodes + 1];
			this._numNodesEverEnqueued = 0;
		}

		public void Clear()
		{
			for (int i = 1; i < this._nodes.Length; i++)
			{
				this._nodes[i] = null;
			}
			this._numNodes = 0;
		}

		public bool Contains(PathfindingCellInfo node)
		{
			return this._nodes[node.QueueIndex] == node;
		}

		public void Enqueue(PathfindingCellInfo node, int priority)
		{
			node.Priority = priority;
			this._numNodes++;
			this._nodes[this._numNodes] = node;
			node.QueueIndex = this._numNodes;
			node.InsertionIndex = this._numNodesEverEnqueued++;
			this.CascadeUp(this._nodes[this._numNodes]);
		}

		private void Swap(PathfindingCellInfo node1, PathfindingCellInfo node2)
		{
			this._nodes[node1.QueueIndex] = node2;
			this._nodes[node2.QueueIndex] = node1;
			int queueIndex = node1.QueueIndex;
			node1.QueueIndex = node2.QueueIndex;
			node2.QueueIndex = queueIndex;
		}

		private void CascadeUp(PathfindingCellInfo node)
		{
			for (int i = node.QueueIndex / 2; i >= 1; i = node.QueueIndex / 2)
			{
				PathfindingCellInfo pathfindingCellInfo = this._nodes[i];
				if (pathfindingCellInfo.Priority < node.Priority || (pathfindingCellInfo.Priority == node.Priority && pathfindingCellInfo.InsertionIndex < node.InsertionIndex))
				{
					break;
				}
				this.Swap(node, pathfindingCellInfo);
			}
		}

		private void CascadeDown(PathfindingCellInfo node)
		{
			int num = node.QueueIndex;
			while (true)
			{
				PathfindingCellInfo pathfindingCellInfo = node;
				int num2 = 2 * num;
				if (num2 > this._numNodes)
				{
					break;
				}
				PathfindingCellInfo pathfindingCellInfo2 = this._nodes[num2];
				if (pathfindingCellInfo2.Priority < pathfindingCellInfo.Priority || (pathfindingCellInfo2.Priority == pathfindingCellInfo.Priority && pathfindingCellInfo2.InsertionIndex < pathfindingCellInfo.InsertionIndex))
				{
					pathfindingCellInfo = pathfindingCellInfo2;
				}
				int num3 = num2 + 1;
				if (num3 <= this._numNodes)
				{
					PathfindingCellInfo pathfindingCellInfo3 = this._nodes[num3];
					if (pathfindingCellInfo3.Priority < pathfindingCellInfo.Priority || (pathfindingCellInfo3.Priority == pathfindingCellInfo.Priority && pathfindingCellInfo3.InsertionIndex < pathfindingCellInfo.InsertionIndex))
					{
						pathfindingCellInfo = pathfindingCellInfo3;
					}
				}
				if (pathfindingCellInfo == node)
				{
					goto IL_E9;
				}
				this._nodes[num] = pathfindingCellInfo;
				int queueIndex = pathfindingCellInfo.QueueIndex;
				pathfindingCellInfo.QueueIndex = num;
				num = queueIndex;
			}
			node.QueueIndex = num;
			this._nodes[num] = node;
			return;
			IL_E9:
			node.QueueIndex = num;
			this._nodes[num] = node;
		}

		private bool HasHigherPriority(PathfindingCellInfo higher, PathfindingCellInfo lower)
		{
			return higher.Priority < lower.Priority || (higher.Priority == lower.Priority && higher.InsertionIndex < lower.InsertionIndex);
		}

		public PathfindingCellInfo Dequeue()
		{
			PathfindingCellInfo pathfindingCellInfo = this._nodes[1];
			this.Remove(pathfindingCellInfo);
			return pathfindingCellInfo;
		}

		public void UpdatePriority(PathfindingCellInfo node, int priority)
		{
			node.Priority = priority;
			this.OnNodeUpdated(node);
		}

		private void OnNodeUpdated(PathfindingCellInfo node)
		{
			int num = node.QueueIndex / 2;
			PathfindingCellInfo lower = this._nodes[num];
			if (num > 0 && this.HasHigherPriority(node, lower))
			{
				this.CascadeUp(node);
			}
			else
			{
				this.CascadeDown(node);
			}
		}

		public void Remove(PathfindingCellInfo node)
		{
			if (this._numNodes <= 1)
			{
				this._nodes[1] = null;
				this._numNodes = 0;
				return;
			}
			bool flag = false;
			PathfindingCellInfo pathfindingCellInfo = this._nodes[this._numNodes];
			if (node.QueueIndex != this._numNodes)
			{
				this.Swap(node, pathfindingCellInfo);
				flag = true;
			}
			this._numNodes--;
			this._nodes[node.QueueIndex] = null;
			if (flag)
			{
				this.OnNodeUpdated(pathfindingCellInfo);
			}
		}

		public PathfindingCellInfo Get(int i)
		{
			return this._nodes[i];
		}

		public bool IsValidQueue()
		{
			for (int i = 1; i < this._nodes.Length; i++)
			{
				if (this._nodes[i] != null)
				{
					int num = 2 * i;
					if (num < this._nodes.Length && this._nodes[num] != null && this.HasHigherPriority(this._nodes[num], this._nodes[i]))
					{
						return false;
					}
					int num2 = num + 1;
					if (num2 < this._nodes.Length && this._nodes[num2] != null && this.HasHigherPriority(this._nodes[num2], this._nodes[i]))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
