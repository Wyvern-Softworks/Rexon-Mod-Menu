// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.CyanChams
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

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Cyan Chams", "Visuals", "Cyan chams on all players.", false, 41, ModType.Toggle, false)]
internal class CyanChams : MonoBehaviour
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
	private static readonly Color ChamsColor = new(0f, 1f, 1f, 0.6f);


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			RestorePlayerMaterials();
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
		foreach (Player player in PhotonNetwork.PlayerListOthers)
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
			((Renderer)rig.mainSkin).material.color = ChamsColor;
		}
	}

	private void RestorePlayerMaterials()
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
		RestorePlayerMaterials();
	}

	private void OnDestroy()
	{
		RestorePlayerMaterials();
	}
}
