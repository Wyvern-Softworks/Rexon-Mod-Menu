// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.TornadoSpin
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Tornado Spin", "Rig", "Both secondary buttons to spin.", false, 6, ModType.Toggle, false)]
internal sealed class TornadoSpin : MonoBehaviour
{
	private float _rotationDegrees;

	private void Update()
	{
		if (!ControllerInputPoller.instance.leftControllerSecondaryButton
			|| !ControllerInputPoller.instance.rightControllerSecondaryButton)
		{
			return;
		}

		_rotationDegrees += 720f * Time.deltaTime;
		if (_rotationDegrees > 360f)
		{
			_rotationDegrees -= 360f;
		}

		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		if (localRig != null)
		{
			localRig.transform.rotation = Quaternion.Euler(0f, _rotationDegrees, 0f);
		}
	}

	private void OnDisable()
	{
		_rotationDegrees = 0f;
	}
}
