// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.SteamLongArms
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Steam Long Arms", "Rig", "Adjustable long arms.", false, 46, ModType.Toggle, false)]
internal sealed class SteamLongArms : MonoBehaviour
{
	internal static float ScaleMultiplier = 1.5f;

	private static Vector3 _originalScale = Vector3.zero;
	private static bool _originalScaleCaptured;

	private static void CaptureOriginalScale()
	{
		if (_originalScaleCaptured)
		{
			return;
		}

		Vector3 scale = GTPlayer.Instance.transform.localScale;
		if (scale.x >= 0.5f && scale.y >= 0.5f && scale.z >= 0.5f)
		{
			_originalScale = scale;
			_originalScaleCaptured = true;
		}
	}

	private void OnEnable()
	{
		CaptureOriginalScale();
	}

	private void Update()
	{
		CaptureOriginalScale();
		if (_originalScaleCaptured)
		{
			GTPlayer.Instance.maxArmLength = 100f;
			GTPlayer.Instance.transform.localScale = _originalScale * ScaleMultiplier;
		}
	}

	private void OnDisable()
	{
		if (_originalScaleCaptured)
		{
			GTPlayer.Instance.transform.localScale = _originalScale;
		}
		ScaleMultiplier = 1f;
	}
}
