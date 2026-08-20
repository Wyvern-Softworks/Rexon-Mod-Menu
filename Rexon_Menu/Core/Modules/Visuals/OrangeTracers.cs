// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.OrangeTracers
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

[Mod("Orange Tracers", "Visuals", "Orange tracers to all players.", false, 34, ModType.Toggle, false)]
internal class OrangeTracers : MonoBehaviour
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
	private List<TracerData> _tracers = new List<TracerData>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearTracers();
			return;
		}
		Vector3 handPosition = GorillaTagger.Instance.rightHandTransform.position;
		for (int i = _tracers.Count - 1; i >= 0; i--)
		{
			TracerData tracerData = _tracers[i];
			if (tracerData.Line == null || tracerData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(tracerData.Player))
			{
				if (tracerData.Line != null)
				{
					Object.Destroy(tracerData.Line.gameObject);
				}
				_tracers.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			LineRenderer line = _tracers.FirstOrDefault(entry => entry.UserId == player.UserId).Line;
			if (line == null)
			{
				line = new GameObject("OrangeTracer_" + player.UserId).AddComponent<LineRenderer>();
				line.startWidth = 0.006f;
				line.endWidth = 0.006f;
				line.positionCount = 2;
				line.useWorldSpace = true;
				line.material = ShaderPatch.CreateTransparentMaterial(Color.white);
				line.startColor = new Color(1f, 0.5f, 0f);
				line.endColor = new Color(1f, 0.3f, 0f);
				_tracers.Add(new TracerData(player, player.UserId, line));
			}
			line.SetPosition(0, handPosition);
			line.SetPosition(1, rig.transform.position);
		}
	}

	private void ClearTracers()
	{
		for (int i = 0; i < _tracers.Count; i++)
		{
			if (_tracers[i].Line != null)
			{
				Object.Destroy(_tracers[i].Line.gameObject);
			}
		}
		_tracers.Clear();
	}

	private void OnDisable()
	{
		ClearTracers();
	}

	private void OnDestroy()
	{
		ClearTracers();
	}
}
