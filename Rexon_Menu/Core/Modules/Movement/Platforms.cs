// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Movement.Platforms
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Movement;

[Mod("Platforms", "Movement", "Grip to spawn platforms", false, 1, ModType.Toggle, false)]
internal class Platforms : MonoBehaviour
{
	private static readonly Vector3 PlatformScale = new Vector3(0.0125f, 0.28f, 0.19f);

	private GameObject _leftPlatform;
	private GameObject _rightPlatform;
	private bool _leftCreated;
	private bool _rightCreated;
	private bool _leftRemoved = true;
	private bool _rightRemoved = true;

	private void Update()
	{
		if (GorillaTagger.Instance == null || GorillaTagger.Instance.offlineVRRig == null)
		{
			return;
		}

		UpdateHandPlatform(
			ControllerInputPoller.instance.leftGrab,
			GorillaTagger.Instance.leftHandTransform,
			ref _leftPlatform,
			ref _leftCreated,
			ref _leftRemoved,
			createEvent: 69,
			removeEvent: 71);

		UpdateHandPlatform(
			ControllerInputPoller.instance.rightGrab,
			GorillaTagger.Instance.rightHandTransform,
			ref _rightPlatform,
			ref _rightCreated,
			ref _rightRemoved,
			createEvent: 70,
			removeEvent: 72);

		RefreshPlatformColors();
	}

	private static void UpdateHandPlatform(
		bool gripPressed,
		Transform hand,
		ref GameObject platform,
		ref bool created,
		ref bool removed,
		byte createEvent,
		byte removeEvent)
	{
		if (gripPressed)
		{
			if (!created && platform == null)
			{
				platform = CreatePlatform(hand.position, hand.rotation);
				created = true;
				removed = false;
				RaisePlatformEvent(createEvent, hand.position, hand.rotation);
			}

			return;
		}

		if (!removed && platform != null)
		{
			Object.Destroy(platform);
			platform = null;
			created = false;
			removed = true;
			RaisePlatformEvent(removeEvent, Vector3.zero, Quaternion.identity);
		}
	}

	private static GameObject CreatePlatform(Vector3 position, Quaternion rotation)
	{
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
		platform.transform.position = position;
		platform.transform.rotation = rotation;
		platform.transform.localScale = PlatformScale;
		platform.GetComponent<Renderer>().material.color = PlatformColorSetting.CurrentColor;
		return platform;
	}

	private static void RaisePlatformEvent(byte eventCode, Vector3 position, Quaternion rotation)
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}

		PhotonNetwork.RaiseEvent(
			eventCode,
			new object[] { position, rotation },
			new RaiseEventOptions { Receivers = ReceiverGroup.Others },
			SendOptions.SendReliable);
	}

	private void RefreshPlatformColors()
	{
		if (_leftPlatform != null)
		{
			_leftPlatform.GetComponent<Renderer>().material.color = PlatformColorSetting.CurrentColor;
		}

		if (_rightPlatform != null)
		{
			_rightPlatform.GetComponent<Renderer>().material.color = PlatformColorSetting.CurrentColor;
		}
	}

	private void OnDisable()
	{
		if (_leftPlatform != null)
		{
			Object.Destroy(_leftPlatform);
			_leftPlatform = null;
		}

		if (_rightPlatform != null)
		{
			Object.Destroy(_rightPlatform);
			_rightPlatform = null;
		}

		_leftCreated = false;
		_rightCreated = false;
		_leftRemoved = true;
		_rightRemoved = true;
	}
}
