// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.Chams
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu_Mat;
using Rexon_Shader;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Chams", "Visuals", "See players through walls.", false, 3, ModType.Toggle, false)]
internal sealed class Chams : MonoBehaviour
{
	private const string BodyMaterialName = "gorilla_body";
	private const string OriginalPalette = "Original";
	private const string DepthTestProperty = "_ZTest";

	private readonly Dictionary<string, Shader> originalShaders = new();
	private readonly Dictionary<string, VRRig> trackedRigs = new();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			RestorePlayers();
			return;
		}

		RemoveDepartedPlayers();
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig || rig.mainSkin == null || player.UserId == null)
			{
				continue;
			}

			if (!trackedRigs.ContainsKey(player.UserId))
			{
				trackedRigs[player.UserId] = rig;
				originalShaders[player.UserId] = rig.mainSkin.material.shader;
			}

			Material material = rig.mainSkin.material;
			material.shader = ShaderBridge.EspShader;
			material.SetInt(DepthTestProperty, 8);
			material.renderQueue = 4000;
			material.color = GetDisplayColor(player, 0.6f);
		}
	}

	private void RemoveDepartedPlayers()
	{
		HashSet<string> activeUserIds = new();
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			if (player.UserId != null)
			{
				activeUserIds.Add(player.UserId);
			}
		}

		List<string> staleUserIds = new();
		foreach (KeyValuePair<string, VRRig> entry in trackedRigs)
		{
			if (activeUserIds.Contains(entry.Key) && entry.Value != null)
			{
				continue;
			}

			RestorePlayer(entry.Key, entry.Value);
			staleUserIds.Add(entry.Key);
		}

		foreach (string userId in staleUserIds)
		{
			trackedRigs.Remove(userId);
			originalShaders.Remove(userId);
		}
	}

	private static Color GetDisplayColor(Player player, float alpha)
	{
		if (Recovered.Obfuscated.VisualThemeSetting.ThemeNames[
				Recovered.Obfuscated.VisualThemeSetting.CurrentIndex] != OriginalPalette)
		{
			Color selected = Recovered.Obfuscated.VisualThemeSetting.GetCurrentColor();
			return new Color(selected.r, selected.g, selected.b, alpha);
		}

		return MatBridge.IsInfected(player)
			? new Color(1f, 0f, 0f, alpha)
			: new Color(0f, 1f, 0f, alpha);
	}

	private void RestorePlayer(string userId, VRRig rig)
	{
		if (rig == null || rig.mainSkin == null || !originalShaders.TryGetValue(userId, out Shader shader))
		{
			return;
		}

		Material material = rig.mainSkin.material;
		material.shader = shader;
		if (material.name.Contains(BodyMaterialName))
		{
			material.color = rig.playerColor;
		}
	}

	private void RestorePlayers()
	{
		foreach (KeyValuePair<string, VRRig> entry in trackedRigs)
		{
			RestorePlayer(entry.Key, entry.Value);
		}

		trackedRigs.Clear();
		originalShaders.Clear();
	}

	private void OnDisable()
	{
		RestorePlayers();
	}
}
