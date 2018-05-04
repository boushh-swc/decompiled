using System;
using System.Collections.Generic;
using UnityEngine;

public class PlanetaryLightingMonoBehaviour : MonoBehaviour
{
	public string colorDataVersionSting = string.Empty;

	public List<Gradient> buildingsDark = new List<Gradient>();

	public List<Gradient> buildingsLight = new List<Gradient>();

	public List<Gradient> terrainDark = new List<Gradient>();

	public List<Gradient> terrainLight = new List<Gradient>();

	public List<Gradient> shadow = new List<Gradient>();

	public List<Gradient> units = new List<Gradient>();

	public List<Gradient> gridColor = new List<Gradient>();

	public List<Gradient> buildingGridColor = new List<Gradient>();

	public List<Gradient> wallGridColor = new List<Gradient>();
}
