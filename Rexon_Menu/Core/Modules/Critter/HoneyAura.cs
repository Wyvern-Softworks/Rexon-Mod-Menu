// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.HoneyAura
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Recovered.Obfuscated;

[Mod("Honey Aura [MASTER]", "Critter", "Spawn honey around you.", false, 10, ModType.Toggle, false)]
internal class HoneyAura : MonoBehaviour
{
	private void Update()
	{
		if (ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.6f)
		{
			Vector3 position = GorillaTagger.Instance.offlineVRRig.transform.position;
			Vector3 spawnPosition = new Vector3(
				position.x + Random.Range(-1f, 1f),
				position.y + Random.Range(-1f, 1f),
				position.z + Random.Range(-1f, 1f));
			CritterUtilities.SpawnHoney(spawnPosition, Quaternion.identity, 0.04f);
		}
	}
}
