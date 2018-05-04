using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Story.Actions
{
	public class MoveCameraStoryAction : AbstractStoryAction
	{
		private const int BOARDX_ARG = 0;

		private const int BOARDZ_ARG = 1;

		private int boardX;

		private int boardZ;

		public MoveCameraStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(2);
			this.boardX = Convert.ToInt32(this.prepareArgs[0]);
			this.boardZ = Convert.ToInt32(this.prepareArgs[1]);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			Vector3 zero = Vector3.zero;
			zero.x = Units.BoardToWorldX(this.boardX);
			zero.z = Units.BoardToWorldZ(this.boardZ);
			Service.WorldInitializer.View.PanToLocation(zero);
			this.parent.ChildComplete(this);
		}
	}
}
