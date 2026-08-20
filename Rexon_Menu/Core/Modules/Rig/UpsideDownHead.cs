// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.UpsideDownHead
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Upside Down Head", "Rig", "Flips your head upside down.", false, 4, ModType.Toggle, false)]
internal class UpsideDownHead : MonoBehaviour
{
	private bool _headRotationRestored;


	private void Update()
	{
		GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = 180f;
		_headRotationRestored = false;
	}

	private void OnDisable()
	{
		if (!_headRotationRestored)
		{
			_headRotationRestored = true;
			GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = 0f;
		}
	}
}

