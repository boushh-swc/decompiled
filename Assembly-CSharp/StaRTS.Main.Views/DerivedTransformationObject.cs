using System;
using UnityEngine;

namespace StaRTS.Main.Views
{
	public class DerivedTransformationObject
	{
		public GameObject BaseTransformationObject
		{
			get;
			private set;
		}

		public Vector3 PositionOffset
		{
			get;
			private set;
		}

		public Quaternion RelativeRotation
		{
			get;
			private set;
		}

		public bool IsUpdateRotation
		{
			get;
			private set;
		}

		public bool AddRelativeRotation
		{
			get;
			private set;
		}

		private DerivedTransformationObject(GameObject baseTransformationObject, Vector3 positionOffset, bool isUpdateRotation, bool addRelativeRotation, Quaternion relativeRoation)
		{
			this.BaseTransformationObject = baseTransformationObject;
			this.PositionOffset = positionOffset;
			this.RelativeRotation = relativeRoation;
			this.IsUpdateRotation = isUpdateRotation;
			this.AddRelativeRotation = addRelativeRotation;
		}

		public DerivedTransformationObject(GameObject baseTransformationObject, Vector3 positionOffset, Quaternion relativeRoation) : this(baseTransformationObject, positionOffset, true, true, relativeRoation)
		{
		}

		public DerivedTransformationObject(GameObject baseTransformationObject, Vector3 positionOffset, bool isUpdateRotation) : this(baseTransformationObject, positionOffset, isUpdateRotation, false, Quaternion.identity)
		{
		}
	}
}
