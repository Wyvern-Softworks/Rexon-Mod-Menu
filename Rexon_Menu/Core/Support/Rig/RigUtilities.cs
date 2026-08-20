// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.RigUtilities
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Recovered.Obfuscated;

public static class RigUtilities
{
	public static VRRig GetRig(NetPlayer player)
	{
		return player == null ? null : GorillaGameManager.instance.FindPlayerVRRig(player);
	}

	public static VRRig GetRig(Player player)
	{
		if (player == null || player.ActorNumber == -1 || GorillaGameManager.instance == null)
		{
			return null;
		}

		VRRig rig = GorillaGameManager.instance.FindPlayerVRRig((NetPlayer)player);
		return rig != null && rig.isActiveAndEnabled ? rig : null;
	}

	public static PhotonView GetPhotonView(VRRig rig)
	{
		NetworkView networkView = Traverse.Create(rig).Field("netView").GetValue<NetworkView>();
		return networkView.GetView;
	}
}
