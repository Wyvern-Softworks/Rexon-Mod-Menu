// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.FlingOnGrab
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Fling On Grab [OLD]", "Overpowered", "Flings player on grab.", false, 22, ModType.Toggle, false)]
internal class FlingOnGrab : MonoBehaviour
{
	private float _lastGrabCheckTime;

	internal Coroutine _flingCoroutine;

	internal NetPlayer _grabbedPlayer;


	private void Update()
	{
		if (PhotonNetwork.InRoom && Time.time >= _lastGrabCheckTime + 0.5f)
		{
			_lastGrabCheckTime = Time.time;
			NetPlayer grabbedPlayer = GorillaTagger.Instance.offlineVRRig.rightHandLink.grabbedPlayer;
			if (grabbedPlayer == null)
			{
				grabbedPlayer = GorillaTagger.Instance.offlineVRRig.leftHandLink.grabbedPlayer;
			}
			if (grabbedPlayer != null && _flingCoroutine == null)
			{
				_grabbedPlayer = grabbedPlayer;
				_flingCoroutine = GorillaTagger.Instance.StartCoroutine(BlackscreenCoroutine(grabbedPlayer));
			}
		}
	}

	private IEnumerator BlackscreenCoroutine(NetPlayer targetPlayer)
	{
		yield return new WaitForSeconds(0.2f);

		if (targetPlayer == null)
		{
			ResetFlingState();
			yield break;
		}
		GorillaTagger.Instance.offlineVRRig.rightHandLink.isGroundedHand = true;
		GorillaTagger.Instance.offlineVRRig.leftHandLink.isGroundedHand = true;
		GorillaTagger.Instance.offlineVRRig.rightHandLink.isGroundedButt = true;
		GorillaTagger.Instance.offlineVRRig.leftHandLink.isGroundedButt = true;
		if (GorillaGameManager.instance.FindPlayerVRRig(targetPlayer) == null)
		{
			ResetFlingState();
			yield break;
		}

		foreach (VRRig rig in VRRigCache.ActiveRigs)
		{
			if (rig.isLocal || (rig.leftHandLink.grabbedPlayer != NetworkSystem.Instance.LocalPlayer && rig.rightHandLink.grabbedPlayer != NetworkSystem.Instance.LocalPlayer))
			{
				continue;
			}

			VRRig.LocalRig.enabled = false;
			for (int step = 0; step < 5; step++)
			{
				VRRig.LocalRig.transform.position += Vector3.up * 0.17f;
				GameNetworkUtilities.SendSpoofedRigSerialization(replacePosition: false, Vector3.zero, new int[1] { rig.Creator.ActorNumber }, Vector3.zero, Vector3.zero);
				PhotonNetwork.SendAllOutgoingCommands();
				yield return null;
			}

			for (int step = 0; step < 500; step++)
			{
				VRRig.LocalRig.transform.position += Vector3.up * 0.25f;
				GameNetworkUtilities.SendSpoofedRigSerialization(replacePosition: false, Vector3.zero, new int[1] { rig.Creator.ActorNumber }, Vector3.zero, Vector3.zero);
				PhotonNetwork.SendAllOutgoingCommands();
			}
		}

		yield return new WaitForSeconds(1f);
		_flingCoroutine = null;
		_grabbedPlayer = null;
		yield return new WaitForSeconds(0.2f);

		VRRig.LocalRig.enabled = true;
		PhotonNetwork.SendAllOutgoingCommands();
		yield return new WaitForSeconds(0.2f);

		GorillaTagger.Instance.offlineVRRig.rightHandLink.BreakLink();
		GorillaTagger.Instance.offlineVRRig.leftHandLink.BreakLink();
		PhotonNetwork.SendAllOutgoingCommands();
	}

	internal void ResetFlingState()
	{
		_flingCoroutine = null;
		_grabbedPlayer = null;
	}

	private void OnDisable()
	{
		ResetFlingState();
	}

	private void OnDestroy()
	{
		ResetFlingState();
	}
}
