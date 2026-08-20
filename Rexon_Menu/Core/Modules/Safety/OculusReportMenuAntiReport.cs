// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Safety.OculusReportMenuAntiReport
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using ExitGames.Client.Photon;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Safety;

[Mod("Oculus Report Menu Anti Report", "Safety", "Detects when someone opens the Oculus report menu on you.", false, 13, ModType.Toggle, false)]
internal class OculusReportMenuAntiReport : MonoBehaviour
{
	private const string HandTapRpc = "RPC_PlayHandTap";

	private bool _isSubscribed;

	private void OnEnable()
	{
		if (!_isSubscribed)
		{
			_isSubscribed = true;
			PhotonNetwork.NetworkingClient.EventReceived += OnNetworkEvent;
		}
	}

	private void OnDisable()
	{
		if (_isSubscribed)
		{
			_isSubscribed = false;
			PhotonNetwork.NetworkingClient.EventReceived -= OnNetworkEvent;
		}
	}

	private void OnNetworkEvent(EventData eventData)
	{
		if (!enabled)
		{
			return;
		}

		if (eventData.Code == 50
			&& (string)((object[])eventData.CustomData)[0] == PhotonNetwork.LocalPlayer.UserId)
		{
			PhotonNetwork.Disconnect();
		}
		if (eventData.Code != 200)
		{
			return;
		}

		Hashtable eventParameters = (Hashtable)eventData.CustomData;
		int rpcIndex = int.Parse(eventParameters[(byte)5].ToString());
		object[] rpcArguments = (object[])eventParameters[(byte)4];
		if (PhotonNetwork.PhotonServerSettings.RpcList[rpcIndex] != HandTapRpc
			|| (int)rpcArguments[0] != 67)
		{
			return;
		}

		Player reporter = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(eventData.Sender, false);
		VRRig reporterRig = RigUtilities.GetRig(reporter);
		if (reporterRig == null
			|| Vector3.Distance(
				reporterRig.leftHandTransform.position,
				reporterRig.rightHandTransform.position) >= 0.1f)
		{
			return;
		}

		string responseMode = AntiReport.ResponseModeNames[AntiReport.ResponseModeIndex];
		switch (responseMode)
		{
			case "Disconnect":
				PhotonNetwork.Disconnect();
				break;
			case "Fling [GUARDIAN]":
				FlingReporter(reporterRig);
				break;
			case "Stutter":
				StutterReporter(reporterRig);
				break;
		}
	}

	private static void FlingReporter(VRRig reporterRig)
	{
		GameObject guardianObject =
			GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager");
		GorillaGuardianManager guardianManager =
			guardianObject != null
				? guardianObject.GetComponent<GorillaGuardianManager>()
				: null;
		if (guardianManager == null
			|| !guardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			PhotonNetwork.Disconnect();
			return;
		}

		GorillaTagger.Instance.myVRRig.GetView.SendRpc(
			"GrabbedByPlayer",
			reporterRig.Creator,
			true,
			false,
			false);
		Vector3 direction =
			(reporterRig.transform.position - GorillaTagger.Instance.rightHandTransform.position).normalized * 200f;
		Vector3 flingVelocity = new Vector3(
			direction.x + Random.Range(-2f, 2f),
			Random.Range(-2f, 2f),
			direction.z + Random.Range(-2f, 2f));
		GorillaTagger.Instance.myVRRig.GetView.SendRpc(
			"DroppedByPlayer",
			reporterRig.Creator,
			flingVelocity);
		PhotonNetwork.SendAllOutgoingCommands();
	}

	private static void StutterReporter(VRRig reporterRig)
	{
		Player player = reporterRig.Creator.GetPlayerRef();
		FriendshipGroupDetection friendshipGroups = FriendshipGroupDetection.Instance;
		if (player == null || friendshipGroups == null)
		{
			PhotonNetwork.Disconnect();
			return;
		}

		for (int index = 0; index < Rexon_Menu.Core.Modules.Settings.LagPower.Power; index++)
		{
			friendshipGroups.photonView.SendRpc(
				"AddPartyMembers",
				player,
				"Infection",
				(short)12,
				null);
		}
		PhotonNetwork.SendAllOutgoingCommands();
	}
}
