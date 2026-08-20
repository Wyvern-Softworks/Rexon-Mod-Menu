// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ArrowPointerESP
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

[Mod("Arrow Pointer ESP", "Visuals", "Arrows pointing toward all players.", false, 19, ModType.Toggle, false)]
internal class ArrowPointerESP : MonoBehaviour
{
	internal struct ArrowData
	{
		public Player Player;

		public string UserId;

		public GameObject Arrow;

		public ArrowData(Player player, string userId, GameObject arrow)
		{
			Player = player;
			UserId = userId;
			Arrow = arrow;
		}
	}

	private static List<ArrowData> Arrows = new List<ArrowData>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearArrows();
			return;
		}
		Vector3 localPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
		for (int i = Arrows.Count - 1; i >= 0; i--)
		{
			ArrowData arrowData = Arrows[i];
			if (arrowData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(arrowData.Player))
			{
				if (arrowData.Arrow != null)
				{
					Object.Destroy(arrowData.Arrow);
				}
				Arrows.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			GameObject arrow = Arrows.FirstOrDefault(entry => entry.UserId == player.UserId).Arrow;
			if (arrow == null)
			{
				arrow = GameObject.CreatePrimitive((PrimitiveType)3);
				arrow.transform.localScale = new Vector3(0.05f, 0.05f, 0.3f);
				Object.Destroy(arrow.GetComponent<Collider>());
				arrow.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(new Color(1f, 0.5f, 0f, 0.8f));
				Arrows.Add(new ArrowData(player, player.UserId, arrow));
			}
			Vector3 direction = (rig.transform.position - localPosition).normalized;
			arrow.transform.position = localPosition + direction * 1.5f;
			arrow.transform.LookAt(rig.transform.position);
		}
	}

	private void ClearArrows()
	{
		for (int i = 0; i < Arrows.Count; i++)
		{
			if (Arrows[i].Arrow != null)
			{
				Object.Destroy(Arrows[i].Arrow);
			}
		}
		Arrows.Clear();
	}

	private void OnDisable()
	{
		ClearArrows();
	}
}

