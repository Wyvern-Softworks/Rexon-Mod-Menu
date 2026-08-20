// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.BackTrack
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Back Track", "Rig", "Fake lag with delayed rig.", false, 30, ModType.Toggle, false)]
internal class BackTrack : MonoBehaviour
{
	private const string GhostRigName = "bt_gh";
	private const string LeftSlideAudioPath = "VR Constraints/LeftArm/Left Arm IK/SlideAudio";
	private const string RightSlideAudioPath = "VR Constraints/RightArm/Right Arm IK/SlideAudio";

	private static readonly Color GhostColor = new(0.19f, 0.53f, 0.74f, 0.2f);
	private static readonly Quaternion HandRotationOffset = Quaternion.Euler(-21f, -5f, 175f);

	private sealed class PoseSample
	{
		internal readonly float Timestamp;
		internal readonly Vector3 Position;
		internal readonly Quaternion Rotation;

		internal PoseSample(float timestamp, Vector3 position, Quaternion rotation)
		{
			Timestamp = timestamp;
			Position = position;
			Rotation = rotation;
		}
	}

	internal static float DelaySeconds = 0.5f;
	internal static bool IsEnabled;
	internal static bool HasDelayedBodyPose;
	internal static Vector3 DelayedBodyPosition;
	internal static Quaternion DelayedBodyRotation;
	internal static Vector3 DelayedHeadPosition;
	internal static Quaternion DelayedHeadRotation;
	internal static Vector3 DelayedLeftHandPosition;
	internal static Quaternion DelayedLeftHandRotation;
	internal static Vector3 DelayedRightHandPosition;
	internal static Quaternion DelayedRightHandRotation;

	private static readonly List<PoseSample> HeadSamples = new();
	private static readonly List<PoseSample> BodySamples = new();
	private static readonly List<PoseSample> LeftHandSamples = new();
	private static readonly List<PoseSample> RightHandSamples = new();

	private static VRRig _ghostRig;
	private static Material _ghostMaterial;

	private void OnEnable()
	{
		IsEnabled = true;
		HasDelayedBodyPose = false;
	}

	private void OnDisable()
	{
		IsEnabled = false;
		HasDelayedBodyPose = false;
		HeadSamples.Clear();
		BodySamples.Clear();
		LeftHandSamples.Clear();
		RightHandSamples.Clear();

		if (!Rexon_Menu.Core.Modules.Rig.InvisibilityRig.IsActive && GorillaTagger.Instance != null)
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}

		if (_ghostRig != null)
		{
			Object.Destroy(_ghostRig.gameObject);
			_ghostRig = null;
			_ghostMaterial = null;
		}
	}

	private void Update()
	{
		float now = Time.time;
		CapturePose(now);
		AdvanceDelayedPose(now);

		if (_ghostRig == null)
		{
			CreateGhostRig();
		}

		UpdateGhostRig();
	}

	private static void CapturePose(float timestamp)
	{
		Transform head = GTPlayer.Instance.headCollider.transform;
		Quaternion headRotation = head.rotation;

		HeadSamples.Add(new PoseSample(timestamp, head.position, headRotation));
		BodySamples.Add(new PoseSample(
			timestamp,
			head.position,
			Quaternion.Euler(0f, headRotation.eulerAngles.y, 0f)));
		LeftHandSamples.Add(new PoseSample(
			timestamp,
			GorillaTagger.Instance.leftHandTransform.position,
			GorillaTagger.Instance.leftHandTransform.rotation));
		RightHandSamples.Add(new PoseSample(
			timestamp,
			GorillaTagger.Instance.rightHandTransform.position,
			GorillaTagger.Instance.rightHandTransform.rotation));
	}

	private static void AdvanceDelayedPose(float now)
	{
		while (TryDequeueReady(BodySamples, now, out PoseSample body))
		{
			DelayedBodyPosition = body.Position;
			DelayedBodyRotation = body.Rotation;
			HasDelayedBodyPose = true;
		}

		while (TryDequeueReady(HeadSamples, now, out PoseSample head))
		{
			DelayedHeadPosition = head.Position;
			DelayedHeadRotation = head.Rotation;
		}

		while (TryDequeueReady(LeftHandSamples, now, out PoseSample leftHand))
		{
			DelayedLeftHandPosition = leftHand.Position;
			DelayedLeftHandRotation = leftHand.Rotation * HandRotationOffset;
		}

		while (TryDequeueReady(RightHandSamples, now, out PoseSample rightHand))
		{
			DelayedRightHandPosition = rightHand.Position;
			DelayedRightHandRotation = rightHand.Rotation * HandRotationOffset;
		}
	}

	private static bool TryDequeueReady(List<PoseSample> samples, float now, out PoseSample sample)
	{
		if (samples.Count == 0 || now - samples[0].Timestamp < DelaySeconds)
		{
			sample = null;
			return false;
		}

		sample = samples[0];
		samples.RemoveAt(0);
		return true;
	}

	private static void CreateGhostRig()
	{
		GameObject temporaryParent = new(GhostRigName);
		temporaryParent.SetActive(false);

		_ghostRig = Object.Instantiate(
			GorillaTagger.Instance.offlineVRRig,
			GTPlayer.Instance.transform.position,
			GTPlayer.Instance.transform.rotation,
			temporaryParent.transform);
		_ghostRig.headBodyOffset = Vector3.zero;
		_ghostRig.gameObject.SetActive(false);
		_ghostRig.transform.SetParent(GorillaTagger.Instance.offlineVRRig.transform.parent);
		Object.Destroy(temporaryParent);

		DisableChild(LeftSlideAudioPath);
		DisableChild(RightSlideAudioPath);
		_ghostRig.GetComponent<OwnershipGaurd>().enabled = false;
		_ghostRig.gameObject.SetActive(true);
	}

	private static void DisableChild(string path)
	{
		Transform child = _ghostRig.transform.Find(path);
		if (child != null)
		{
			child.gameObject.SetActive(false);
		}
	}

	private static void UpdateGhostRig()
	{
		if (_ghostMaterial == null)
		{
			_ghostMaterial = ShaderPatch.CreateTransparentMaterial(GhostColor);
		}

		_ghostMaterial.color = GhostColor;
		((Renderer)_ghostRig.mainSkin).material = _ghostMaterial;

		Transform playerHead = GTPlayer.Instance.headCollider.transform;
		_ghostRig.headConstraint.transform.SetPositionAndRotation(playerHead.position, playerHead.rotation);
		_ghostRig.leftHandTransform.SetPositionAndRotation(
			GorillaTagger.Instance.leftHandTransform.position,
			GorillaTagger.Instance.leftHandTransform.rotation);
		_ghostRig.rightHandTransform.SetPositionAndRotation(
			GorillaTagger.Instance.rightHandTransform.position,
			GorillaTagger.Instance.rightHandTransform.rotation);
		_ghostRig.transform.SetPositionAndRotation(
			GTPlayer.Instance.transform.position,
			GTPlayer.Instance.transform.rotation);
	}
}
