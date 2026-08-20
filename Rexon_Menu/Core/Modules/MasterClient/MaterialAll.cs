// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.MaterialAll
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Material All", "Masterclient", "Cycles material states on all players.", false, 5, ModType.Toggle, false)]
internal class MaterialAll : MonoBehaviour
{
	private float _lastCycleTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom
			|| !PhotonNetwork.IsMasterClient
			|| Time.time <= _lastCycleTime + 0.1f)
		{
			return;
		}
		_lastCycleTime = Time.time;

		foreach (VRRig rig in VRRigCache.ActiveRigs)
		{
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
		}
			NetworkView networkView =
				(NetworkView)Traverse.Create(rig).Field("netView").GetValue();
			NetPlayer player = networkView.GetView.Owner;
			if (player == null)
			{
				continue;
			}

			GameNetworkUtilities.OriginalRigMaterialIndexes.TryGetValue(rig, out int materialIndex);
			materialIndex = (materialIndex + 1) % 4;
			if (GorillaGameManager.instance != null
				&& (int)GorillaGameManager.instance.GameType() == 0
				&& materialIndex < 2)
			{
				materialIndex = 2;
			}
			GameNetworkUtilities.OriginalRigMaterialIndexes[rig] = materialIndex;

			switch (materialIndex)
			{
				case 0:
					ClearTaggedMaterial(player);
					break;
				case 1:
					GameNetworkUtilities.RequestMonkeBallGameState(
						new Dictionary<int, int> { { player.ActorNumber, 0 } });
					break;
				case 2:
					GameNetworkUtilities.RequestMonkeBallGameState(
						new Dictionary<int, int> { { player.ActorNumber, 1 } });
					break;
				case 3:
					break;
			}
		}
	}

	private static void ClearTaggedMaterial(NetPlayer player)
	{
		if (GorillaGameManager.instance == null)
		{
			return;
		}
		switch ((int)GorillaGameManager.instance.GameType())
		{
			case 2:
				((GorillaPaintbrawlManager)GorillaGameManager.instance)
					.playerLives[player.ActorNumber] = 3;
				break;
			case 1:
			case 6:
			case 7:
			case 11:
				break;
			default:
				GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
				if (tagManager.isCurrentlyTag && tagManager.currentIt == player)
				{
					tagManager.currentIt = null;
				}
				if (tagManager.currentInfected.Contains(player))
				{
					tagManager.currentInfected.Remove(player);
				}
				break;
		}
	}

	private void OnDisable()
	{
		GameNetworkUtilities.OriginalRigMaterialIndexes.Clear();
	}
}
