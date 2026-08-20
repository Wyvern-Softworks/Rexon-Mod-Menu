// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.CylinderESP
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

[Mod("Cylinder ESP", "Visuals", "Cylinders around all players.", false, 15, ModType.Toggle, false)]
internal class CylinderESP : MonoBehaviour
{
	internal struct CylinderData
	{
		public Player Player;

		public string UserId;

		public GameObject Cylinder;

		public CylinderData(Player player, string userId, GameObject obj)
		{
			Player = player;
			UserId = userId;
			Cylinder = obj;
		}
	}

	private static List<CylinderData> Cylinders = new List<CylinderData>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearCylinders();
			return;
		}
		for (int i = Cylinders.Count - 1; i >= 0; i--)
		{
			CylinderData cylinderData = Cylinders[i];
			if (cylinderData.Cylinder == null || cylinderData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(cylinderData.Player))
			{
				if (cylinderData.Cylinder != null)
				{
					Object.Destroy(cylinderData.Cylinder);
				}
				Cylinders.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			GameObject cylinder = Cylinders.FirstOrDefault(entry => entry.UserId == player.UserId).Cylinder;
			if (cylinder == null)
			{
				cylinder = GameObject.CreatePrimitive((PrimitiveType)2);
				cylinder.transform.localScale = new Vector3(0.6f, 1.5f, 0.6f);
				Object.Destroy(cylinder.GetComponent<Collider>());
				cylinder.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(new Color(0f, 1f, 0.5f, 0.1f));
				Cylinders.Add(new CylinderData(player, player.UserId, cylinder));
			}
			cylinder.transform.position = rig.transform.position;
		}
	}

	private void ClearCylinders()
	{
		for (int i = 0; i < Cylinders.Count; i++)
		{
			if (Cylinders[i].Cylinder != null)
			{
				Object.Destroy(Cylinders[i].Cylinder);
			}
		}
		Cylinders.Clear();
	}

	private void OnDisable()
	{
		ClearCylinders();
	}
}

