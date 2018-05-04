using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class DroidNode : Node<DroidNode>
	{
		public SizeComponent Size
		{
			get;
			set;
		}

		public StateComponent State
		{
			get;
			set;
		}

		public DroidComponent Droid
		{
			get;
			set;
		}

		public TransformComponent Transform
		{
			get;
			set;
		}

		public override bool IsValid()
		{
			return base.IsValid() && this.Size != null && this.State != null && this.Droid != null && this.Droid.Target != null && this.Transform != null;
		}
	}
}
