using System;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public interface IGeometryVO
	{
		Vector3 IconCameraPosition
		{
			get;
			set;
		}

		Vector3 IconLookatPosition
		{
			get;
			set;
		}

		Vector3 IconCloseupCameraPosition
		{
			get;
			set;
		}

		Vector3 IconCloseupLookatPosition
		{
			get;
			set;
		}

		string IconBundleName
		{
			get;
			set;
		}

		string IconAssetName
		{
			get;
			set;
		}

		float IconRotationSpeed
		{
			get;
			set;
		}
	}
}
