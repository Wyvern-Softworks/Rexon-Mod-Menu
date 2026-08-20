// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.PlayerEsp
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("ESP", "Visuals", "Boxes around all players.", false, 2, ModType.Toggle, false)]
internal sealed class PlayerEsp : MonoBehaviour
{
	private const string OriginalPalette = "Original";

	private sealed class EspBox
	{
		public Player Player { get; }
		public string UserId { get; }
		public GameObject Box { get; }

		public EspBox(Player player, GameObject box)
		{
			Player = player;
			UserId = player.UserId;
			Box = box;
		}
	}

	private readonly List<EspBox> boxes = new();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearBoxes();
			return;
		}

		for (int index = boxes.Count - 1; index >= 0; index--)
		{
			EspBox entry = boxes[index];
			if (entry.Box != null && entry.Player != null && PhotonNetwork.PlayerListOthers.Contains(entry.Player))
			{
				continue;
			}

			if (entry.Box != null)
			{
				Object.Destroy(entry.Box);
			}
			boxes.RemoveAt(index);
		}

		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}

			EspBox entry = boxes.FirstOrDefault(candidate => candidate.UserId == player.UserId);
			if (entry == null)
			{
				GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
				box.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
				Object.Destroy(box.GetComponent<Collider>());
				box.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(Color.white);
				entry = new EspBox(player, box);
				boxes.Add(entry);
			}

			entry.Box.GetComponent<Renderer>().material.color = GetDisplayColor(player, 0.3f);
			entry.Box.transform.position = rig.transform.position;
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

	private void ClearBoxes()
	{
		foreach (EspBox entry in boxes)
		{
			if (entry.Box != null)
			{
				Object.Destroy(entry.Box);
			}
		}
		boxes.Clear();
	}

	private void OnDisable()
	{
		ClearBoxes();
	}
}
