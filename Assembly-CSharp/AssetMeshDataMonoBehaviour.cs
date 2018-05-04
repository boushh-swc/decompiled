using System;
using System.Collections.Generic;
using UnityEngine;

public class AssetMeshDataMonoBehaviour : MonoBehaviour
{
	public List<GameObject> SelectableGameObjects = new List<GameObject>();

	public GameObject ShadowGameObject;

	public List<GameObject> GunLocatorGameObjects = new List<GameObject>();

	public GameObject TickerGameObject;

	public GameObject MeterGameObject;

	public List<GameObject> OtherGameObjects = new List<GameObject>();

	public List<AnimationClip> ListOfAnimationTracks = new List<AnimationClip>();

	public List<ParticleSystem> ListOfParticleSystems = new List<ParticleSystem>();

	public float WalkSpeed;

	public float RunSpeed;
}
