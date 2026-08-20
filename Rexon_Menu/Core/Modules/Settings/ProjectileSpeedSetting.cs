// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ProjectileSpeedSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Projectile Speed: Default", "Settings", "Change projectile speed.", true, 7, ModType.Toggle, false)]
internal class ProjectileSpeedSetting : MonoBehaviour
{

	private void OnEnable()
	{
		GameNetworkUtilities.ProjectileSpeedIndex = (GameNetworkUtilities.ProjectileSpeedIndex + 1) % GameNetworkUtilities.ProjectileSpeedNames.Length;
		BundleManager.SetProjectileSpeedStatusText("Projectile Speed: " + GameNetworkUtilities.ProjectileSpeedNames[GameNetworkUtilities.ProjectileSpeedIndex]);
		ConfigurationManager.SaveIfAutoLoadEnabled();
		this.StartCoroutine(DelayedDestroy());
	}

	private IEnumerator DelayedDestroy()
	{
		yield return (object)new WaitForSeconds(0.1f);
		Object.Destroy(this);
	}
}
