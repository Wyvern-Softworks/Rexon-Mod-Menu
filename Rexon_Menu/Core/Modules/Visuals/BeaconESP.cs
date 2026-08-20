// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.BeaconESP
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

[Mod("Beacon ESP", "Visuals", "Tall beacon pillars on all players.", false, 16, ModType.Toggle, false)]
internal class BeaconESP : MonoBehaviour
{
	internal struct BeaconData
	{
		public Player Player;

		public string UserId;

		public GameObject Beacon;

		public BeaconData(Player player, string userId, GameObject obj)
		{
			Player = player;
			UserId = userId;
			Beacon = obj;
		}
	}

	private static List<BeaconData> Beacons = new List<BeaconData>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearBeacons();
			return;
		}
		for (int i = Beacons.Count - 1; i >= 0; i--)
		{
			BeaconData beaconData = Beacons[i];
			if (beaconData.Beacon == null || beaconData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(beaconData.Player))
			{
				if (beaconData.Beacon != null)
				{
					Object.Destroy(beaconData.Beacon);
				}
				Beacons.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			GameObject beacon = Beacons.FirstOrDefault(entry => entry.UserId == player.UserId).Beacon;
			if (beacon == null)
			{
				beacon = GameObject.CreatePrimitive((PrimitiveType)3);
				beacon.transform.localScale = new Vector3(0.1f, 50f, 0.1f);
				Object.Destroy(beacon.GetComponent<Collider>());
				beacon.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(new Color(1f, 1f, 0f, 0.3f));
				Beacons.Add(new BeaconData(player, player.UserId, beacon));
			}
			beacon.transform.position = rig.transform.position + new Vector3(0f, 25f, 0f);
		}
	}

	private void ClearBeacons()
	{
		for (int i = 0; i < Beacons.Count; i++)
		{
			if (Beacons[i].Beacon != null)
			{
				Object.Destroy(Beacons[i].Beacon);
			}
		}
		Beacons.Clear();
	}

	private void OnDisable()
	{
		ClearBeacons();
	}
}

