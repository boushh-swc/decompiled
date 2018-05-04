using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views
{
	public class DerivedTransformationManager : IViewFrameTimeObserver
	{
		private Dictionary<GameObject, DerivedTransformationObject> objectMap;

		private bool alreadyLoggedGameObjectNullError;

		public DerivedTransformationManager()
		{
			Service.DerivedTransformationManager = this;
			this.objectMap = new Dictionary<GameObject, DerivedTransformationObject>();
		}

		public void AddDerivedTransformation(GameObject gameObject, DerivedTransformationObject derivedTransformationObject)
		{
			if (gameObject != null && derivedTransformationObject.BaseTransformationObject != null)
			{
				this.objectMap.Add(gameObject, derivedTransformationObject);
				if (this.objectMap.Count == 1)
				{
					Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
				}
			}
		}

		public void RemoveDerivedTransformation(GameObject gameObject)
		{
			if (gameObject != null)
			{
				this.objectMap.Remove(gameObject);
				if (this.objectMap.Count == 0)
				{
					Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
				}
			}
			else
			{
				Service.Logger.Warn("Null object is being passed to RemoveDerivedTransformation");
			}
		}

		public void OnViewFrameTime(float dt)
		{
			foreach (KeyValuePair<GameObject, DerivedTransformationObject> current in this.objectMap)
			{
				GameObject key = current.Key;
				DerivedTransformationObject value = current.Value;
				if (key == null)
				{
					if (!this.alreadyLoggedGameObjectNullError)
					{
						string text = "destroyed";
						if (value.BaseTransformationObject != null)
						{
							text = value.BaseTransformationObject.name;
						}
						Service.Logger.ErrorFormat("GameObject being added to DerivedTransformationManager is being destroyed. Associated base object is {0}.", new object[]
						{
							text
						});
						this.alreadyLoggedGameObjectNullError = true;
					}
				}
				else if (!(value.BaseTransformationObject == null))
				{
					Transform transform = key.transform;
					transform.position = value.BaseTransformationObject.transform.position + value.PositionOffset;
					if (value.IsUpdateRotation)
					{
						if (value.AddRelativeRotation)
						{
							transform.rotation = GameUtils.ApplyRelativeRotation(value.RelativeRotation, value.BaseTransformationObject.transform.rotation);
						}
						else
						{
							transform.rotation = value.BaseTransformationObject.transform.rotation;
						}
					}
				}
			}
		}
	}
}
