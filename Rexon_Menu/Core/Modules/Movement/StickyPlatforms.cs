// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Movement.StickyPlatforms
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Movement;

[Mod("Sticky Platforms", "Movement", "Stacked grip platforms", false, 2, ModType.Toggle, false)]
internal class StickyPlatforms : MonoBehaviour
{
	private const int StackSize = 20;
	private static readonly Vector3 PlatformScale = new Vector3(0.00725f, 0.28f, 0.14f);

	private readonly List<GameObject> _leftPlatforms = new List<GameObject>();
	private readonly List<GameObject> _rightPlatforms = new List<GameObject>();
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

		UpdateHandStack(
			ControllerInputPoller.instance.leftGrab,
			isLeftHand: true,
			ref _leftCreated,
			ref _leftRemoved,
			_leftPlatforms);

		UpdateHandStack(
			ControllerInputPoller.instance.rightGrab,
			isLeftHand: false,
			ref _rightCreated,
			ref _rightRemoved,
			_rightPlatforms);

		RefreshPlatformColors();
	}

	private static void UpdateHandStack(
		bool gripPressed,
		bool isLeftHand,
		ref bool created,
		ref bool removed,
		List<GameObject> platforms)
	{
		if (gripPressed)
		{
			if (created || platforms.Count != 0)
			{
				return;
			}

			(Vector3 posePosition, Quaternion _) = GetHandPose(isLeftHand);
			Transform hand = isLeftHand
				? GorillaTagger.Instance.leftHandTransform
				: GorillaTagger.Instance.rightHandTransform;

			for (int index = 0; index < StackSize; index++)
			{
				float verticalOffset = -0.1f + index * 0.01f;
				Vector3 position = posePosition + new Vector3(0f, verticalOffset, 0f);
				platforms.Add(CreatePlatform(position, hand.rotation));
			}

			created = true;
			removed = false;
			RaisePlatformEvent(isLeftHand ? (byte)69 : (byte)70, posePosition, hand.rotation);
			return;
		}

		if (removed || platforms.Count == 0)
		{
			return;
		}

		DestroyPlatforms(platforms);
		created = false;
		removed = true;
		RaisePlatformEvent(isLeftHand ? (byte)71 : (byte)72, Vector3.zero, Quaternion.identity);
	}

	private static (Vector3 position, Quaternion rotation) GetHandPose(bool isLeftHand)
	{
		Transform hand = isLeftHand
			? GorillaTagger.Instance.leftHandTransform
			: GorillaTagger.Instance.rightHandTransform;
		GTPlayer player = GTPlayer.Instance;

		if (isLeftHand)
		{
			return (
				hand.position + hand.rotation * (player.LeftHand.handOffset * player.scale),
				hand.rotation * player.LeftHand.handRotOffset);
		}

		return (
			hand.position + hand.rotation * (player.RightHand.handOffset * player.scale),
			hand.rotation * player.RightHand.handRotOffset);
	}

	private static GameObject CreatePlatform(Vector3 position, Quaternion rotation)
	{
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
		platform.transform.position = position;
		platform.transform.rotation = rotation;
		platform.transform.localScale = PlatformScale;
		platform.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(PlatformColorSetting.CurrentColor);
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
		RefreshPlatformColors(_leftPlatforms);
		RefreshPlatformColors(_rightPlatforms);
	}

	private static void RefreshPlatformColors(List<GameObject> platforms)
	{
		foreach (GameObject platform in platforms)
		{
			if (platform != null)
			{
				platform.GetComponent<Renderer>().material.color = PlatformColorSetting.CurrentColor;
			}
		}
	}

	private static void DestroyPlatforms(List<GameObject> platforms)
	{
		foreach (GameObject platform in platforms)
		{
			if (platform != null)
			{
				Object.Destroy(platform);
			}
		}

		platforms.Clear();
	}

	private void OnDisable()
	{
		DestroyPlatforms(_leftPlatforms);
		DestroyPlatforms(_rightPlatforms);
		_leftCreated = false;
		_rightCreated = false;
		_leftRemoved = true;
		_rightRemoved = true;
	}
}
