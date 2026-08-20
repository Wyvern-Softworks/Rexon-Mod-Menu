// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.GripIndicator
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Grip Indicator", "Visuals", "Shows green/red sphere above players for grip state.", false, 44, ModType.Toggle, false)]
internal class GripIndicator : MonoBehaviour
{
	private const string IndicatorShaderName = "GUI/Text Shader";
	private const float IndicatorScale = 0.12f;
	private const float IndicatorHeight = 0.45f;

	private static readonly Color ReadyColor = new(0f, 1f, 0f, 0.8f);
	private static readonly Color GrippingColor = new(1f, 0.6f, 0f, 0.8f);
	private static readonly Color IdleColor = new(1f, 0f, 0f, 0.8f);

	private readonly struct IndicatorData
	{
		internal readonly Player Player;
		internal readonly string UserId;
		internal readonly VRRig Rig;
		internal readonly GameObject Indicator;

		internal IndicatorData(Player player, VRRig rig, GameObject indicator)
		{
			Player = player;
			UserId = player.UserId;
			Rig = rig;
			Indicator = indicator;
		}
	}

	private readonly List<IndicatorData> _indicators = new();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearIndicators();
			return;
		}

		RemoveStaleIndicators();
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}

			int indicatorIndex = FindIndicatorIndex(player.UserId);
			if (indicatorIndex < 0)
			{
				_indicators.Add(new IndicatorData(player, rig, CreateIndicator()));
				indicatorIndex = _indicators.Count - 1;
			}

			UpdateIndicator(_indicators[indicatorIndex], rig);
		}
	}

	private void RemoveStaleIndicators()
	{
		for (int index = _indicators.Count - 1; index >= 0; index--)
		{
			IndicatorData data = _indicators[index];
			if (data.Rig != null && PhotonNetwork.PlayerListOthers.Contains(data.Player))
			{
				continue;
			}

			DestroyIndicator(data.Indicator);
			_indicators.RemoveAt(index);
		}
	}

	private int FindIndicatorIndex(string userId)
	{
		for (int index = 0; index < _indicators.Count; index++)
		{
			if (_indicators[index].UserId == userId)
			{
				return index;
			}
		}

		return -1;
	}

	private static GameObject CreateIndicator()
	{
		GameObject indicator = GameObject.CreatePrimitive((PrimitiveType)0);
		indicator.transform.localScale = Vector3.one * IndicatorScale;
		Object.Destroy(indicator.GetComponent<SphereCollider>());
		indicator.GetComponent<Renderer>().material = new Material(Shader.Find(IndicatorShaderName));
		return indicator;
	}

	private static void UpdateIndicator(IndicatorData data, VRRig rig)
	{
		if (data.Indicator == null)
		{
			return;
		}

		int leftGripDigit = rig.handSync / 10000 % 10;
		int rightGripDigit = rig.handSync / 10 % 10;
		bool gripPressed = leftGripDigit >= 5 || rightGripDigit >= 5;
		bool handReady = rig.leftHandLink.isReadyForGrabbing || rig.rightHandLink.isReadyForGrabbing;

		Color color = handReady ? ReadyColor : gripPressed ? GrippingColor : IdleColor;
		data.Indicator.transform.position = rig.transform.position + Vector3.up * IndicatorHeight;
		data.Indicator.GetComponent<Renderer>().material.color = color;
	}

	private static void DestroyIndicator(GameObject indicator)
	{
		if (indicator != null)
		{
			Object.Destroy(indicator);
		}
	}

	private void ClearIndicators()
	{
		foreach (IndicatorData data in _indicators)
		{
			DestroyIndicator(data.Indicator);
		}
		_indicators.Clear();
	}

	private void OnDisable()
	{
		ClearIndicators();
	}

	private void OnDestroy()
	{
		ClearIndicators();
	}
}
