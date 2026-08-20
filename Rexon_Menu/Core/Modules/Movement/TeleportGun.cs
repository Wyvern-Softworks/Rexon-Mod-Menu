// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.TeleportGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Teleport Gun", "Movement", "Grip + trigger to teleport", false, 3, ModType.Toggle, false)]
internal class TeleportGun : MonoBehaviour
{
	private const string GunId = "TeleportGun";
	private const float GunCooldown = 0.5f;
	private const float DirectTeleportDistance = 4f;
	private const float TravelDuration = 0.1f;
	private const float LandingOffset = 0.5f;

	private void Update()
	{
		GunController.GunResult gun = GunController.GetGunResult(GunId, targetPlayers: false, GunCooldown);
		Vector3 hitPoint = gun.Hit.point;
		if (!gun.IsActive || !gun.IsShooting || !gun.CanFire || hitPoint == Vector3.zero)
		{
			return;
		}

		GunController.MarkFired(GunId);
		Vector3 destination = hitPoint + Vector3.up * LandingOffset;
		Vector3 currentPosition = GTPlayer.Instance.bodyCollider.transform.position;
		float distance = Vector3.Distance(destination, currentPosition);
		Rigidbody playerBody = GTPlayer.Instance.GetComponent<Rigidbody>();

		if (distance > DirectTeleportDistance)
		{
			GTPlayer.Instance.transform.position = destination;
			playerBody.velocity = Vector3.zero;
			return;
		}

		Vector3 direction = (destination - currentPosition).normalized;
		playerBody.velocity = direction * (distance / TravelDuration);
	}

	private void OnDisable()
	{
		GunController.Release(GunId);
	}
}
