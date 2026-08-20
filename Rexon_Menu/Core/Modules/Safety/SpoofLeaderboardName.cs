// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Safety.SpoofLeaderboardName
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using GorillaNetworking;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Rexon_Menu.Core.Modules.Safety;

[Mod("Spoof Leaderboard Name", "Safety", "Changes your displayed name on the scoreboard.", false, 10, ModType.Toggle, false)]
internal class SpoofLeaderboardName : MonoBehaviour
{
	private const string InitializeMaterialRpc = "RPC_InitializeNoobMaterial";

	private string _originalNickname;
	private string _spoofedNickname;
	private bool _active;

	private void OnEnable()
	{
		try
		{
			_originalNickname = PhotonNetwork.LocalPlayer.NickName;
			_spoofedNickname = GenerateNickname();
			ApplyLocalNickname(_spoofedNickname);

			if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				float red = PlayerPrefs.GetFloat("redValue", 0.5f);
				float green = PlayerPrefs.GetFloat("greenValue", 0.5f);
				float blue = PlayerPrefs.GetFloat("blueValue", 0.5f);
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(
					InitializeMaterialRpc,
					RpcTarget.All,
					red,
					green,
					blue);
				GameNetworkUtilities.FlushAndReplayNetworkTraffic();
			}

			_active = true;
		}
		catch (Exception)
		{
		}
	}

	private void Update()
	{
		if (_active && PhotonNetwork.LocalPlayer.NickName != _spoofedNickname)
		{
			PhotonNetwork.LocalPlayer.NickName = _spoofedNickname;
		}
	}

	private void OnDisable()
	{
		if (!_active)
		{
			return;
		}

		_active = false;
		try
		{
			if (!string.IsNullOrEmpty(_originalNickname))
			{
				ApplyLocalNickname(_originalNickname);
			}
		}
		catch (Exception)
		{
		}
	}

	private static void ApplyLocalNickname(string nickname)
	{
		PhotonNetwork.LocalPlayer.NickName = nickname;
		GorillaComputer computer = (GorillaComputer)GorillaComputer.instance;
		computer.currentName = nickname;
		computer.SetLocalNameTagText(nickname);
	}

	private static string GenerateNickname()
	{
		string[] names = { "GORILLA", "MONKE", "PLAYER", "BANANA", "KONG", "CHIMP", "APE", "PRIMATE" };
		string[] suffixes = { "123", "XD", "99", "PRO", "YT", "TV", "GG", "420" };
		string nickname = names[Random.Range(0, names.Length)] + suffixes[Random.Range(0, suffixes.Length)];
		return nickname.Length <= 12 ? nickname : nickname.Substring(0, 12);
	}
}
