// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.BreakAudioGunV2
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Break Audio Gun V2", "Overpowered", "Corrupts target's audio. Be near them.", false, 26, ModType.Toggle, false)]
internal class BreakAudioGunV2 : MonoBehaviour
{
	private const string GunId = "BreakAudioGunV2";
	private const string PlayHandTapRpc = "RPC_PlayHandTap";

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
			VRRig targetRig = MatBridge.GetVRRigFor(_targetPlayer);
			if (targetRig != null && !targetRig.isOfflineVRRig && !targetRig.isMyPlayer)
			{
				if (!_alternateHand)
				{
					GorillaTagger.Instance.myVRRig.GetView.SendRpc(PlayHandTapRpc, _targetPlayer, 213, true, 6560f);
					_alternateHand = true;
				}
				else
				{
					GorillaTagger.Instance.myVRRig.GetView.SendRpc(PlayHandTapRpc, _targetPlayer, 213, false, 6560f);
					_alternateHand = false;
				}
				GameNetworkUtilities.FlushAndReplayNetworkTraffic();
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

	private void OnDestroy()
	{
		GunController.Release(GunId);
		_targetPlayer = null;
	}
}
