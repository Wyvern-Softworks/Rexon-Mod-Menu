// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.RgbMonke
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaNetworking;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("RGB Monke", "Rig", "Rainbow monke color in stump.", false, 8, ModType.Toggle, false)]
internal sealed class RgbMonke : MonoBehaviour
{
	private const string ColorProperty = "_Color";
	private const string ColorRpc = "RPC_InitializeNoobMaterial";

	private float _hue;
	private float _lastNetworkUpdateAt;

	private void Update()
	{
		if (!PhotonNetwork.InRoom
			|| PhotonNetwork.CurrentRoom == null
			|| !GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
		{
			return;
		}

		_hue += 0.5f * Time.deltaTime;
		if (_hue > 1f)
		{
			_hue -= 1f;
		}

		Color color = Color.HSVToRGB(_hue, 1f, 1f);
		GorillaComputer.instance.pressedMaterial.SetColor(ColorProperty, color);
		GorillaTagger.Instance.offlineVRRig.materialsToChangeTo[0].SetColor(ColorProperty, color);

		if (Time.time <= _lastNetworkUpdateAt + 0.1f
			|| GorillaTagger.Instance.myVRRig == null
			|| PhotonNetwork.LocalPlayer == null)
		{
			return;
		}

		_lastNetworkUpdateAt = Time.time;
		PlayerPrefs.SetFloat("redValue", color.r);
		PlayerPrefs.SetFloat("greenValue", color.g);
		PlayerPrefs.SetFloat("blueValue", color.b);
		PlayerPrefs.Save();
		GorillaTagger.Instance.myVRRig.GetView.SendRpc(
			ColorRpc,
			RpcTarget.All,
			color.r,
			color.g,
			color.b);
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}
}
