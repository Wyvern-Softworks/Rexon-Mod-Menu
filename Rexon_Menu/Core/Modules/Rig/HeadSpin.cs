// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.HeadSpin
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Head Spin", "Rig", "Spins your head.", false, 3, ModType.Toggle, false)]
internal class HeadSpin : MonoBehaviour
{
	private float _rotationAngle;


	private void Update()
	{
		_rotationAngle += Time.deltaTime * 500f;
		if (_rotationAngle > 360f)
		{
			_rotationAngle -= 360f;
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig != null && offlineVRRig.head != null)
		{
			offlineVRRig.head.trackingRotationOffset.y = _rotationAngle;
		}
	}

	private void OnDisable()
	{
		RestoreHeadRotation();
	}

	private void RestoreHeadRotation()
	{
		GorillaTagger tagger = GorillaTagger.Instance;
		if (tagger == null || tagger.offlineVRRig == null)
		{
			return;
		}

		if (tagger.offlineVRRig.head != null)
		{
			tagger.offlineVRRig.head.trackingRotationOffset.y = 0f;
		}
	}
}
