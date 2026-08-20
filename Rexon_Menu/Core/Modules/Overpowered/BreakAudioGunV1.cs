// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.BreakAudioGunV1
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Break Audio Gun V1", "Overpowered", "Corrupts target audio. [W?]", false, 23, ModType.Toggle, false)]
internal class BreakAudioGunV1 : MonoBehaviour
{
	private const string GunId = "BreakAudioGun";
	private const string PlayHandTapRpc = "RPC_PlayHandTap";

	private float _lastRpcTime;

	private Player _targetPlayer;

	private bool _alternateHand;


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			GunController.Release(GunId);
			_targetPlayer = null;
			return;
		}
		GunController.GunResult gunResult = GunController.GetGunResult(GunId, targetPlayers: true, 0.1f);
		if (!gunResult.IsActive)
		{
			_targetPlayer = null;
			return;
		}
		if (gunResult.IsShooting && gunResult.Target != null)
		{
			_targetPlayer = gunResult.Target;
		}
		if (_targetPlayer != null && gunResult.IsShooting)
		{
			GunController.MarkFired(GunId);
			if (Time.time > _lastRpcTime + 0.02f)
			{
				_lastRpcTime = Time.time;
				VRRig targetRig = MatBridge.GetVRRigFor(_targetPlayer);
				if (!targetRig.isOfflineVRRig && !targetRig.isMyPlayer)
				{
					GorillaTagger.Instance.myVRRig.GetView.SendRpc(
						PlayHandTapRpc, _targetPlayer, 111, _alternateHand, 6560f);
					_alternateHand = !_alternateHand;
					GameNetworkUtilities.FlushAndReplayNetworkTraffic();
				}
			}
		}
		else
		{
			_targetPlayer = null;
		}
	}

	private void OnDisable()
	{
		GunController.Release(GunId);
		_targetPlayer = null;
	}
}
