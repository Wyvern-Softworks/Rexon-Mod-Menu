// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ThickTracers
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

[Mod("Thick Tracers", "Visuals", "Thick white tracers to players.", false, 12, ModType.Toggle, false)]
internal class ThickTracers : MonoBehaviour
{
	internal struct TracerData
	{
		public Player Player;

		public string UserId;

		public LineRenderer Line;

		public TracerData(Player player, string userId, LineRenderer line)
		{
			Player = player;
			UserId = userId;
			Line = line;
		}
	}

	private static List<TracerData> Tracers = new();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearTracers();
			return;
		}
		Vector3 handPosition = GorillaTagger.Instance.rightHandTransform.position;
		for (int i = Tracers.Count - 1; i >= 0; i--)
		{
			TracerData tracerData = Tracers[i];
			if (tracerData.Line == null || tracerData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(tracerData.Player))
			{
				if (tracerData.Line != null)
				{
					Object.Destroy(tracerData.Line.gameObject);
				}
				Tracers.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			LineRenderer line = Tracers.FirstOrDefault(entry => entry.UserId == player.UserId).Line;
			if (line == null)
			{
				line = new GameObject("ThickTracer_" + player.UserId).AddComponent<LineRenderer>();
				line.startWidth = 0.03f;
				line.endWidth = 0.03f;
				line.positionCount = 2;
				line.useWorldSpace = true;
				line.material = ShaderPatch.CreateTransparentMaterial(Color.white);
				Color color = new Color(1f, 1f, 1f, 0.5f);
				line.startColor = color;
				line.endColor = color;
				Tracers.Add(new TracerData(player, player.UserId, line));
			}
			line.SetPosition(0, handPosition);
			line.SetPosition(1, rig.transform.position);
		}
	}

	private void ClearTracers()
	{
		for (int i = 0; i < Tracers.Count; i++)
		{
			if (Tracers[i].Line != null)
			{
				Object.Destroy(Tracers[i].Line.gameObject);
			}
		}
		Tracers.Clear();
	}

	private void OnDisable()
	{
		ClearTracers();
	}
}


