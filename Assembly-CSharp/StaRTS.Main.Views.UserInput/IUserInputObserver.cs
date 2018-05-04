using StaRTS.Utils;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UserInput
{
	public interface IUserInputObserver
	{
		EatResponse OnPress(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition);

		EatResponse OnDrag(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition);

		EatResponse OnRelease(int id);

		EatResponse OnScroll(float delta, Vector2 screenPosition);
	}
}
