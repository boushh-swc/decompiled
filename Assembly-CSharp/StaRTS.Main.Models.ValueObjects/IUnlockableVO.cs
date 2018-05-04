using System;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public interface IUnlockableVO
	{
		Vector3 IconUnlockScale
		{
			get;
			set;
		}

		Vector3 IconUnlockRotation
		{
			get;
			set;
		}

		Vector3 IconUnlockPosition
		{
			get;
			set;
		}
	}
}
