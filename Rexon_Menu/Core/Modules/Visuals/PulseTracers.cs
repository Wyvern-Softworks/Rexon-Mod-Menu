// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.PulseTracers
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

[Mod("Pulse Tracers", "Visuals", "Width-pulsing tracers to players.", false, 10, ModType.Toggle, false)]
internal class PulseTracers : MonoBehaviour
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
		float width = Mathf.Lerp(0.003f, 0.018f, (Mathf.Sin(Time.time * 5f) + 1f) * 0.5f);
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
				line = new GameObject("PulseTracer_" + player.UserId).AddComponent<LineRenderer>();
				line.positionCount = 2;
				line.useWorldSpace = true;
				line.material = ShaderPatch.CreateTransparentMaterial(Color.white);
				Tracers.Add(new TracerData(player, player.UserId, line));
			}
			line.startWidth = width;
			line.endWidth = width;
			line.startColor = Color.magenta;
			line.endColor = Color.cyan;
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


