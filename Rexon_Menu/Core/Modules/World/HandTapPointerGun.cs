// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.HandTapPointerGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Hand Tap Pointer Gun", "World", "Shoot hand taps at aim.", false, 24, ModType.Toggle, false)]
internal class HandTapPointerGun : MonoBehaviour
{
	private const string GunId = "HandTapPointerGun";
	private const string PlayHandTapRpc = "RPC_PlayHandTap";

	private float _lastTapTime;

	private bool _leftHand;


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			GunController.Release(GunId);
			return;
		}
		GunController.GunResult gunResult = GunController.GetGunResult(GunId, targetPlayers: false);
		if (gunResult.IsActive && gunResult.IsShooting && Time.time > _lastTapTime + 0.08f)
		{
			_lastTapTime = Time.time;
			GorillaTagger.Instance.myVRRig.GetView.SendRpc(PlayHandTapRpc, RpcTarget.All, 66, _leftHand, 0.5f);
			_leftHand = !_leftHand;
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}
	}

	private void OnDisable()
	{
		ClearPointer();
	}

	private void ClearPointer()
	{
		GunController.Release(GunId);
		_lastTapTime = 0f;
	}
}
