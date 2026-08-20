// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ProjectileColorSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Projectile Color: Black", "Settings", "Change projectile color.", true, 6, ModType.Toggle, false)]
internal class ProjectileColorSetting : MonoBehaviour
{
	private void OnEnable()
	{
		GameNetworkUtilities.ProjectileColorIndex = (GameNetworkUtilities.ProjectileColorIndex + 1) % GameNetworkUtilities.ProjectileColorNames.Length;
		BundleManager.SetProjectileColorStatusText("Projectile Color: " + GameNetworkUtilities.ProjectileColorNames[GameNetworkUtilities.ProjectileColorIndex]);
		ConfigurationManager.SaveIfAutoLoadEnabled();
		this.StartCoroutine(DelayedDestroy());
	}

	private IEnumerator DelayedDestroy()
	{
		yield return (object)new WaitForSeconds(0.1f);
		Object.Destroy(this);
	}
}
