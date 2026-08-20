// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.CapsuleESP
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

[Mod("Capsule ESP", "Visuals", "Capsules around all players.", false, 14, ModType.Toggle, false)]
internal class CapsuleESP : MonoBehaviour
{
	private struct CapsuleData
	{
		public Player Player;
		public string UserId;
		public GameObject Capsule;

		public CapsuleData(Player player, string userId, GameObject capsule)
		{
			Player = player;
			UserId = userId;
			Capsule = capsule;
		}
	}

	private static readonly List<CapsuleData> Capsules = new List<CapsuleData>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearCapsules();
			return;
		}

		RemoveDepartedPlayers();
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}

			GameObject capsule = FindCapsule(player.UserId);
			if (capsule == null)
			{
				capsule = CreateCapsule();
				Capsules.Add(new CapsuleData(player, player.UserId, capsule));
			}
			capsule.transform.position = rig.transform.position;
		}
	}

	private static GameObject FindCapsule(string userId)
	{
		for (int index = 0; index < Capsules.Count; index++)
		{
			if (Capsules[index].UserId == userId)
			{
				return Capsules[index].Capsule;
			}
		}
		return null;
	}

	private static GameObject CreateCapsule()
	{
		GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		capsule.transform.localScale = new Vector3(0.4f, 0.6f, 0.4f);
		Object.Destroy(capsule.GetComponent<Collider>());
		capsule.GetComponent<Renderer>().material =
			ShaderPatch.CreateTransparentMaterial(new Color(1f, 0f, 1f, 0.2f));
		return capsule;
	}

	private static void RemoveDepartedPlayers()
	{
		for (int index = Capsules.Count - 1; index >= 0; index--)
		{
			CapsuleData data = Capsules[index];
			if (data.Capsule != null &&
				data.Player != null && PhotonNetwork.PlayerListOthers.Contains(data.Player))
			{
				continue;
			}
			if (data.Capsule != null)
			{
				Object.Destroy(data.Capsule);
			}
			Capsules.RemoveAt(index);
		}
	}

	private static void ClearCapsules()
	{
		foreach (CapsuleData data in Capsules)
		{
			if (data.Capsule != null)
			{
				Object.Destroy(data.Capsule);
			}
		}
		Capsules.Clear();
	}

	private void OnDisable() => ClearCapsules();
}
