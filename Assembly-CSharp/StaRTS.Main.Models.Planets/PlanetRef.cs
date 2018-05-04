using System;
using UnityEngine;

namespace StaRTS.Main.Models.Planets
{
	public class PlanetRef : MonoBehaviour
	{
		public Planet Planet
		{
			get;
			set;
		}

		public PlanetRef(Planet planet)
		{
			this.Planet = planet;
		}
	}
}
