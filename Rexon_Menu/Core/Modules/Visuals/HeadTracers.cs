// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.HeadTracers
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

[Mod("Head Tracers", "Visuals", "Tracers to player heads.", false, 5, ModType.Toggle, false)]
internal sealed class HeadTracers : MonoBehaviour
{
	private const string OriginalPalette = "Original";

	private sealed class Tracer
	{
		public Player Player { get; }
		public string UserId { get; }
		public LineRenderer Line { get; }

		public Tracer(Player player, LineRenderer line)
		{
			Player = player;
			UserId = player.UserId;
			Line = line;
		}
	}

	private readonly List<Tracer> tracers = new();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearTracers();
			return;
		}

		for (int index = tracers.Count - 1; index >= 0; index--)
		{
			Tracer tracer = tracers[index];
			if (tracer.Line != null && tracer.Player != null && PhotonNetwork.PlayerListOthers.Contains(tracer.Player))
			{
				continue;
			}

			if (tracer.Line != null)
			{
				Object.Destroy(tracer.Line.gameObject);
			}
			tracers.RemoveAt(index);
		}

		Vector3 origin = GorillaTagger.Instance.headCollider.transform.position;
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}

			Tracer tracer = tracers.FirstOrDefault(candidate => candidate.UserId == player.UserId);
			if (tracer == null)
			{
				LineRenderer line = new GameObject("HeadTracer_" + player.UserId).AddComponent<LineRenderer>();
				line.startWidth = 0.005f;
				line.endWidth = 0.005f;
				line.positionCount = 2;
				line.useWorldSpace = true;
				line.material = ShaderPatch.CreateTransparentMaterial(Color.white);
				tracer = new Tracer(player, line);
				tracers.Add(tracer);
			}

			Color color = GetDisplayColor(player, 0.6f);
			tracer.Line.material.color = color;
			tracer.Line.startColor = color;
			tracer.Line.endColor = color;
			tracer.Line.SetPosition(0, origin);
			tracer.Line.SetPosition(1, rig.headConstraint.position);
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
		foreach (Tracer tracer in tracers)
		{
			if (tracer.Line != null)
			{
				Object.Destroy(tracer.Line.gameObject);
			}
		}
		tracers.Clear();
	}

	private void OnDisable()
	{
		ClearTracers();
	}
}
