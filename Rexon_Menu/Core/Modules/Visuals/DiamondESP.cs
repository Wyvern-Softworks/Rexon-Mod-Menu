// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.DiamondESP
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

[Mod("Diamond ESP", "Visuals", "Diamond shapes around all players.", false, 21, ModType.Toggle, false)]
internal class DiamondESP : MonoBehaviour
{
	internal struct DiamondData
	{
		public Player Player;

		public string UserId;

		public GameObject Diamond;

		public DiamondData(Player player, string userId, GameObject diamond)
		{
			Player = player;
			UserId = userId;
			Diamond = diamond;
		}
	}

	private static List<DiamondData> Diamonds = new List<DiamondData>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearDiamonds();
			return;
		}
		for (int i = Diamonds.Count - 1; i >= 0; i--)
		{
			DiamondData diamondData = Diamonds[i];
			if (diamondData.Diamond == null || diamondData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(diamondData.Player))
			{
				if (diamondData.Diamond != null)
				{
					Object.Destroy(diamondData.Diamond);
				}
				Diamonds.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			GameObject diamond = Diamonds.FirstOrDefault(entry => entry.UserId == player.UserId).Diamond;
			if (diamond == null)
			{
				diamond = GameObject.CreatePrimitive((PrimitiveType)3);
				diamond.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
				diamond.transform.rotation = Quaternion.Euler(45f, 0f, 45f);
				Object.Destroy(diamond.GetComponent<Collider>());
				diamond.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(new Color(0.5f, 1f, 0f, 0.3f));
				Diamonds.Add(new DiamondData(player, player.UserId, diamond));
			}
			diamond.transform.position = rig.transform.position;
		}
	}

	private void ClearDiamonds()
	{
		for (int i = 0; i < Diamonds.Count; i++)
		{
			if (Diamonds[i].Diamond != null)
			{
				Object.Destroy(Diamonds[i].Diamond);
			}
		}
		Diamonds.Clear();
	}

	private void OnDisable()
	{
		ClearDiamonds();
	}
}

