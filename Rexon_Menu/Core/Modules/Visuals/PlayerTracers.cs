// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.PlayerTracers
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Tracers", "Visuals", "Lines to all players.", false, 1, ModType.Toggle, false)]
internal sealed class PlayerTracers : MonoBehaviour
{
	private const string OriginalPalette = "Original";
	private readonly Dictionary<string, LineRenderer> tracers = new();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearTracers();
			return;
		}

		RemoveDepartedPlayers();
		Vector3 origin = GorillaTagger.Instance.rightHandTransform.position;
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig || player.UserId == null)
			{
				continue;
			}

			if (!tracers.TryGetValue(player.UserId, out LineRenderer line) || line == null)
			{
				line = new GameObject("Tracer_" + player.UserId).AddComponent<LineRenderer>();
				line.startWidth = 0.01f;
				line.endWidth = 0.01f;
				line.positionCount = 2;
				line.useWorldSpace = true;
				line.material = ShaderPatch.CreateTransparentMaterial(Color.white);
				tracers[player.UserId] = line;
			}

			Color color = GetDisplayColor(player, 0.6f);
			line.material.color = color;
			line.startColor = color;
			line.endColor = color;
			line.SetPosition(0, origin);
			line.SetPosition(1, rig.transform.position);
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
		foreach (KeyValuePair<string, LineRenderer> entry in tracers)
		{
			if (activeUserIds.Contains(entry.Key) && entry.Value != null)
			{
				continue;
			}

			if (entry.Value != null)
			{
				Object.Destroy(entry.Value.gameObject);
			}
			staleUserIds.Add(entry.Key);
		}

		foreach (string userId in staleUserIds)
		{
			tracers.Remove(userId);
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

	private void ClearTracers()
	{
		foreach (LineRenderer line in tracers.Values)
		{
			if (line != null)
			{
				Object.Destroy(line.gameObject);
			}
		}
		tracers.Clear();
	}

	private void OnDisable()
	{
		ClearTracers();
	}
}
