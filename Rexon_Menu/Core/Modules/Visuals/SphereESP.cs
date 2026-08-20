// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SphereESP
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Sphere ESP", "Visuals", "Spheres around all players.", false, 13, ModType.Toggle, false)]
internal class SphereESP : MonoBehaviour
{
	internal struct SphereData
	{
		public Player Player;

		public string UserId;

		public GameObject Sphere;

		public SphereData(Player player, string userId, GameObject obj)
		{
			Player = player;
			UserId = userId;
			Sphere = obj;
		}
	}

	private static List<SphereData> Spheres = new List<SphereData>();


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearSpheres();
			return;
		}
		for (int i = Spheres.Count - 1; i >= 0; i--)
		{
			SphereData sphereData = Spheres[i];
			if (sphereData.Sphere == null || sphereData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(sphereData.Player))
			{
				if (sphereData.Sphere != null)
				{
					Object.Destroy(sphereData.Sphere);
				}
				Spheres.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			GameObject sphere = Spheres.FirstOrDefault(entry => entry.UserId == player.UserId).Sphere;
			if (sphere == null)
			{
				sphere = GameObject.CreatePrimitive((PrimitiveType)0);
				sphere.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
				Object.Destroy(sphere.GetComponent<Collider>());
				sphere.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(new Color(0f, 0.5f, 1f, 0.15f));
				Spheres.Add(new SphereData(player, player.UserId, sphere));
			}
			sphere.transform.position = rig.transform.position;
		}
	}

	private void ClearSpheres()
	{
		for (int i = 0; i < Spheres.Count; i++)
		{
			if (Spheres[i].Sphere != null)
			{
				Object.Destroy(Spheres[i].Sphere);
			}
		}
		Spheres.Clear();
	}

	private void OnDisable()
	{
		ClearSpheres();
	}
}

