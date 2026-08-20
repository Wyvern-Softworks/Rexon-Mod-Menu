// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.FakeLagSelf
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Fake Lag Self", "Rig", "Makes you appear laggy to others.", false, 32, ModType.Toggle, false)]
internal class FakeLagSelf : MonoBehaviour
{
	private float _lastRigToggleTime;

	private float _lastUpdateTime;


	private void Update()
	{
		if (Time.time >= _lastUpdateTime + 0.2f)
		{
			_lastUpdateTime = Time.time;
			VRRig localRig = GorillaTagger.Instance.offlineVRRig;
			float toggleDelay = localRig.enabled ? 0.08f : Random.Range(0.4f, 1.4f);
			if (Time.time > _lastRigToggleTime + toggleDelay)
			{
				_lastRigToggleTime = Time.time;
				localRig.enabled = !localRig.enabled;
			}
		}
	}

	private void OnDisable()
	{
		GorillaTagger.Instance.offlineVRRig.enabled = true;
	}
}
