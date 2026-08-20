// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.PulseChams
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using Rexon_Menu_Mat;
using Rexon_Shader;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Pulse Chams", "Visuals", "Pink pulsing chams on players.", false, 24, ModType.Toggle, false)]
internal class PulseChams : MonoBehaviour
{
	private const string OriginalBodyMaterialMarker = "gorilla_body";

	internal struct ChamsData
	{
		public Player Player;

		public string UserId;

		public VRRig Rig;

		public Shader OriginalShader;

		public ChamsData(Player player, string userId, VRRig rig, Shader originalShader)
		{
			Player = player;
			UserId = userId;
			Rig = rig;
			OriginalShader = originalShader;
		}
	}

	private readonly List<ChamsData> _trackedPlayers = new();


	private void Update()
	{
			if (!PhotonNetwork.InRoom)
		{
			RestoreMaterials();
			return;
		}
		for (int index = _trackedPlayers.Count - 1; index >= 0; index--)
		{
			ChamsData chamsData = _trackedPlayers[index];
			if (chamsData.Rig == null || !PhotonNetwork.PlayerListOthers.Contains(chamsData.Player))
			{
				if (chamsData.Rig != null && chamsData.Rig.mainSkin != null)
				{
					((Renderer)chamsData.Rig.mainSkin).material.shader = chamsData.OriginalShader;
					if (((Renderer)chamsData.Rig.mainSkin).material.name.Contains(OriginalBodyMaterialMarker))
					{
						((Renderer)chamsData.Rig.mainSkin).material.color = chamsData.Rig.playerColor;
					}
				}
				_trackedPlayers.RemoveAt(index);
			}
		}
		float alpha = Mathf.Lerp(0.3f, 0.8f, (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f);
		Color color = new Color(1f, 0f, 0.5f, alpha);
		Player[] playerListOthers = PhotonNetwork.PlayerListOthers;
		foreach (Player player in playerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig || rig.mainSkin == null)
			{
				continue;
			}
			bool isTracked = _trackedPlayers.Any(entry => entry.UserId == player.UserId);
			if (!isTracked)
			{
				_trackedPlayers.Add(new ChamsData(player, player.UserId, rig, ((Renderer)rig.mainSkin).material.shader));
			}
			ShaderPatch.EnsureCached();
			((Renderer)rig.mainSkin).material.shader = ShaderBridge.Cached;
			((Renderer)rig.mainSkin).material.color = color;
		}
	}

	private void RestoreMaterials()
	{
			for (int i = 0; i < _trackedPlayers.Count; i++)
		{
			ChamsData chamsData = _trackedPlayers[i];
			if (chamsData.Rig != null && chamsData.Rig.mainSkin != null)
			{
				((Renderer)chamsData.Rig.mainSkin).material.shader = chamsData.OriginalShader;
				if (((Renderer)chamsData.Rig.mainSkin).material.name.Contains(OriginalBodyMaterialMarker))
				{
					((Renderer)chamsData.Rig.mainSkin).material.color = chamsData.Rig.playerColor;
				}
			}
		}
		_trackedPlayers.Clear();
	}

	private void OnDisable()
	{
		RestoreMaterials();
	}
}


