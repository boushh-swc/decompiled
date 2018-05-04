using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Planets
{
	public class PlanetStatsResponse : Response
	{
		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				string planetUID = string.Empty;
				foreach (KeyValuePair<string, object> current in dictionary)
				{
					planetUID = current.Key;
					int population = Convert.ToInt32(current.Value as string);
					Service.GalaxyPlanetController.UpdatePlanetPopulation(planetUID, population);
				}
			}
			return this;
		}
	}
}
