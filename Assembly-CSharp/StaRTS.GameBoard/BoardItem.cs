using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard.Components;
using System;
using System.Collections.Generic;

namespace StaRTS.GameBoard
{
	public class BoardItem
	{
		private Entity data;

		private Board currentBoard;

		private BoardCell currentCell;

		private SizeComponent size;

		private FilterComponent filter;

		private FlagStamp flagStamp;

		private LinkedListNode<BoardItem> linkedListNode;

		public Entity Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		public Board CurrentBoard
		{
			get
			{
				return this.currentBoard;
			}
		}

		public BoardCell CurrentCell
		{
			get
			{
				return this.currentCell;
			}
		}

		public SizeComponent Size
		{
			get
			{
				return this.size;
			}
		}

		public FilterComponent Filter
		{
			get
			{
				return this.filter;
			}
			set
			{
				this.filter = value;
			}
		}

		public FlagStamp FlagStamp
		{
			get
			{
				return this.flagStamp;
			}
			set
			{
				this.flagStamp = value;
			}
		}

		public int Width
		{
			get
			{
				return this.size.Width;
			}
			set
			{
				this.size.Width = value;
			}
		}

		public int Depth
		{
			get
			{
				return this.size.Depth;
			}
			set
			{
				this.size.Depth = value;
			}
		}

		public int BoardX
		{
			get
			{
				return this.currentCell.X;
			}
		}

		public int BoardZ
		{
			get
			{
				return this.currentCell.Z;
			}
		}

		public LinkedListNode<BoardItem> LinkedListNode
		{
			get
			{
				return this.linkedListNode;
			}
			set
			{
				this.linkedListNode = value;
			}
		}

		public BoardItem(SizeComponent size, Entity data, FilterComponent filter)
		{
			this.size = size;
			this.data = data;
			this.filter = filter;
		}

		internal void Internal_InformAddedToBoard(Board board, int x, int y)
		{
			this.currentBoard = board;
			this.currentCell = board.GetCellAt(x, y);
		}

		internal void Internal_InformRemovedFromBoard()
		{
			this.currentBoard = null;
			this.currentCell = null;
		}

		public override string ToString()
		{
			string str = "< [";
			if (this.currentCell != null)
			{
				str = str + this.BoardX.ToString() + ", " + this.BoardZ.ToString();
			}
			else
			{
				str += "-, -";
			}
			return str + "] " + this.data.ToString() + ">";
		}
	}
}
