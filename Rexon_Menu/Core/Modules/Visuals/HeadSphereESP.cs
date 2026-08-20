// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.HeadSphereESP
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

[Mod("Head Sphere ESP", "Visuals", "Red spheres on player heads.", false, 17, ModType.Toggle, false)]
internal class HeadSphereESP : MonoBehaviour
{
	internal struct HeadSphereData
	{
		public Player Player;

		public string UserId;

		public GameObject Marker;

		public HeadSphereData(Player player, string userId, GameObject sphere)
		{
			Player = player;
			UserId = userId;
			Marker = sphere;
		}
	}

	private static List<HeadSphereData> HeadSpheres = new List<HeadSphereData>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearHeadSpheres();
			return;
		}
		for (int i = HeadSpheres.Count - 1; i >= 0; i--)
		{
			HeadSphereData headSphereData = HeadSpheres[i];
			if (headSphereData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(headSphereData.Player))
			{
				if (headSphereData.Marker != null)
				{
					Object.Destroy(headSphereData.Marker);
				}
				HeadSpheres.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			GameObject sphere = HeadSpheres.FirstOrDefault(entry => entry.UserId == player.UserId).Marker;
			if (sphere == null)
			{
				sphere = GameObject.CreatePrimitive((PrimitiveType)0);
				sphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
				Object.Destroy(sphere.GetComponent<Collider>());
				sphere.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(new Color(1f, 0f, 0f, 0.5f));
				HeadSpheres.Add(new HeadSphereData(player, player.UserId, sphere));
			}
			sphere.transform.position = rig.headMesh.transform.position;
		}
	}

	private void ClearHeadSpheres()
	{
		for (int i = 0; i < HeadSpheres.Count; i++)
		{
			if (HeadSpheres[i].Marker != null)
			{
				Object.Destroy(HeadSpheres[i].Marker);
			}
		}
		HeadSpheres.Clear();
	}

	private void OnDisable()
	{
		ClearHeadSpheres();
	}
}


