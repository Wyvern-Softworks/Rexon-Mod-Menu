// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.GunController
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Patches;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

internal static class GunController
{
	public enum TracerStyle
	{
		Straight,
		Wavy,
		Wiggle,
		Tether,
		Zigzag,
		Off
	}

	public readonly struct GunResult
	{
		public GameObject Pointer { get; }
		public RaycastHit Hit { get; }
		public bool IsShooting { get; }
		public bool CanFire { get; }
		public Player Target { get; }
		public bool IsActive { get; }
		public Player LockedTarget { get; }
		public Player[] LockedTargets { get; }

		public GunResult(
			GameObject pointer,
			RaycastHit hit,
			bool isShooting,
			bool canFire,
			Player target,
			bool isActive,
			Player lockedTarget,
			Player[] lockedTargets)
		{
			Pointer = pointer;
			Hit = hit;
			IsShooting = isShooting;
			CanFire = canFire;
			Target = target;
			IsActive = isActive;
			LockedTarget = lockedTarget;
			LockedTargets = lockedTargets;
		}
	}

	private const int TracerPointCount = 20;
	private const float SelectionDebounceSeconds = 0.5f;

	private static readonly Dictionary<string, float> lastFireTimes = new();
	private static readonly Dictionary<string, float> lastSelectionTimes = new();
	private static readonly Dictionary<string, Player> lockedTargets = new();
	private static readonly Dictionary<string, HashSet<Player>> multiLockedTargets = new();
	private static readonly Dictionary<string, List<GameObject>> multiLockMarkers = new();
	private static readonly HashSet<string> activeModules = new();

	private static readonly int transparentFxLayer = LayerMask.NameToLayer("TransparentFX");
	private static readonly int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
	private static readonly int zoneLayer = LayerMask.NameToLayer("Zone");
	private static readonly int gorillaTriggerLayer = LayerMask.NameToLayer("Gorilla Trigger");
	private static readonly int gorillaBoundaryLayer = LayerMask.NameToLayer("Gorilla Boundary");
	private static readonly int gorillaCosmeticsLayer = LayerMask.NameToLayer("GorillaCosmetics");
	private static readonly int gorillaParticleLayer = LayerMask.NameToLayer("GorillaParticle");

	private static GameObject pointer;
	private static GameObject tracerObject;
	private static LineRenderer tracer;
	private static GameObject singleLockMarker;
	private static Vector3[] tracerPoints = new Vector3[TracerPointCount];
	private static Vector3[] tracerVelocities = new Vector3[TracerPointCount];
	private static float wavePhase;
	private static bool triggerWasPressed;
	private static bool tracerPhysicsInitialized;

	public static TracerStyle CurrentTracer { get; set; } = TracerStyle.Straight;
	public static Color ColorIdle { get; set; } = new(0.35f, 0.01f, 0.41f, 1f);
	public static Color ColorShooting { get; set; } = new(0.7f, 0.01f, 0.82f, 1f);
	public static Color ColorLocked { get; set; } = new(0.6f, 0f, 1f, 1f);
	public static bool TracerEnabled { get; set; } = true;
	public static bool PointerEnabled { get; set; } = true;
	public static bool SoundEnabled { get; set; }
	public static bool IsRainbow { get; set; }
	internal static float RainbowHue { get; set; }

	public static GunResult GetGunResult(
		string moduleId,
		bool targetPlayers = true,
		float fireCooldown = 0f,
		bool allowSingleTargetLock = false,
		bool allowMultipleTargetLocks = false)
	{
		RegisterModule(moduleId);
		RemoveDisconnectedLocks(moduleId);

		Player lockedTarget = lockedTargets[moduleId];
		Player[] selectedTargets = multiLockedTargets[moduleId].ToArray();

		if (XRSettings.isDeviceActive)
		{
			if (GorillaTagger.Instance == null || GorillaTagger.Instance.offlineVRRig == null)
			{
				HideAimObjects();
				HideSingleLockMarker();
				return InactiveResult(lockedTarget, selectedTargets);
			}

			if (!ControllerInputPoller.instance.rightGrab)
			{
				HideAimObjects();
				UpdateLockMarkers(moduleId, allowSingleTargetLock, allowMultipleTargetLocks);
				selectedTargets = multiLockedTargets[moduleId].ToArray();
				bool canFire = allowMultipleTargetLocks &&
					selectedTargets.Length > 0 &&
					IsCooldownReady(moduleId, fireCooldown);
				return new GunResult(null, default, false, canFire, null, false, lockedTarget, selectedTargets);
			}

			EnsureAimObjects();
			Vector3 origin = GetControllerOrigin();
			Transform rightHand = GorillaTagger.Instance.rightHandTransform;
			Vector3 direction = Vector3.Lerp(-rightHand.up, rightHand.forward, 0.5f);
			Physics.Raycast(origin, direction, out RaycastHit hit, 512f, GetAimLayerMask());
			bool triggerPressed = ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.5f;
			return ProcessAim(
				moduleId,
				targetPlayers,
				fireCooldown,
				allowSingleTargetLock,
				allowMultipleTargetLocks,
				origin,
				hit,
				triggerPressed);
		}

		EnsureAimObjects();
		Camera aimCamera = GameObject.Find("Shoulder Camera")?.GetComponent<Camera>() ?? Camera.main;
		if (aimCamera == null)
		{
			HideAimObjects();
			UpdateLockMarkers(moduleId, allowSingleTargetLock, allowMultipleTargetLocks);
			selectedTargets = multiLockedTargets[moduleId].ToArray();
			bool canFire = allowMultipleTargetLocks &&
				selectedTargets.Length > 0 &&
				IsCooldownReady(moduleId, fireCooldown);
			return new GunResult(null, default, false, canFire, null, false, lockedTarget, selectedTargets);
		}

		Physics.Raycast(
			aimCamera.ScreenPointToRay(UnityInput.Current.mousePosition),
			out RaycastHit desktopHit,
			512f,
			GetAimLayerMask());
		bool mousePressed = Mouse.current != null && Mouse.current.leftButton.isPressed;
		return ProcessAim(
			moduleId,
			targetPlayers,
			fireCooldown,
			allowSingleTargetLock,
			allowMultipleTargetLocks,
			GetControllerOrigin(),
			desktopHit,
			mousePressed);
	}

	private static GunResult ProcessAim(
		string moduleId,
		bool targetPlayers,
		float fireCooldown,
		bool allowSingleTargetLock,
		bool allowMultipleTargetLocks,
		Vector3 origin,
		RaycastHit hit,
		bool triggerPressed)
	{
		Player target = targetPlayers && hit.collider != null ? GetPlayer(hit.collider) : null;

		if (allowMultipleTargetLocks)
		{
			if (triggerPressed && !triggerWasPressed && target != null && CanToggleSelection(moduleId))
			{
				HashSet<Player> selection = multiLockedTargets[moduleId];
				if (!selection.Add(target))
				{
					selection.Remove(target);
				}
			}
		}
		else if (allowSingleTargetLock &&
			triggerPressed &&
			!triggerWasPressed &&
			target != null &&
			CanToggleSelection(moduleId))
		{
			lockedTargets[moduleId] = lockedTargets[moduleId] == target ? null : target;
		}

		RemoveDisconnectedLocks(moduleId);
		UpdateLockMarkers(moduleId, allowSingleTargetLock, allowMultipleTargetLocks);

		Player lockedTarget = lockedTargets[moduleId];
		Player[] selectedTargets = multiLockedTargets[moduleId].ToArray();
		Vector3 targetPoint = GetTargetPoint(hit, target);
		UpdatePointer(targetPoint, triggerPressed);
		UpdateTracer(origin, targetPoint, triggerPressed);
		PlayTriggerSound(triggerPressed);

		bool canFire = IsCooldownReady(moduleId, fireCooldown) &&
			(!allowMultipleTargetLocks || selectedTargets.Length > 0);
		triggerWasPressed = triggerPressed;

		return new GunResult(
			pointer,
			hit,
			triggerPressed,
			canFire,
			target,
			true,
			lockedTarget,
			selectedTargets);
	}

	private static void RegisterModule(string moduleId)
	{
		activeModules.Add(moduleId);
		if (!lastFireTimes.ContainsKey(moduleId))
		{
			lastFireTimes[moduleId] = 0f;
		}
		if (!lastSelectionTimes.ContainsKey(moduleId))
		{
			lastSelectionTimes[moduleId] = 0f;
		}
		if (!lockedTargets.ContainsKey(moduleId))
		{
			lockedTargets[moduleId] = null;
		}
		if (!multiLockedTargets.ContainsKey(moduleId))
		{
			multiLockedTargets[moduleId] = new HashSet<Player>();
		}
		if (!multiLockMarkers.ContainsKey(moduleId))
		{
			multiLockMarkers[moduleId] = new List<GameObject>();
		}
	}

	private static GunResult InactiveResult(Player lockedTarget, Player[] selectedTargets)
	{
		return new GunResult(null, default, false, false, null, false, lockedTarget, selectedTargets);
	}

	private static Vector3 GetControllerOrigin()
	{
		Transform rightHand = GorillaTagger.Instance.rightHandTransform;
		GTPlayer player = GTPlayer.Instance;
		return rightHand.position + rightHand.rotation * (player.RightHand.handOffset * player.scale);
	}

	private static Vector3 GetTargetPoint(RaycastHit hit, Player target)
	{
		if (target == null)
		{
			return hit.point;
		}

		VRRig rig = GetRig(target);
		return rig != null ? rig.transform.position : hit.point;
	}

	private static Player GetPlayer(Collider collider)
	{
		VRRig rig = collider.GetComponentInParent<VRRig>();
		if (rig == null)
		{
			return null;
		}

		FieldInfo photonViewField = typeof(VRRig).GetField(
			"photonView",
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (photonViewField?.GetValue(rig) is PhotonView view &&
			view.Owner != null &&
			view.Owner != PhotonNetwork.LocalPlayer)
		{
			return view.Owner;
		}

		foreach (Player player in PhotonNetwork.PlayerList)
		{
			if (player != PhotonNetwork.LocalPlayer && GetRig(player) == rig)
			{
				return player;
			}
		}
		return null;
	}

	private static VRRig GetRig(Player player)
	{
		return GorillaGameManager.StaticFindRigForPlayer((NetPlayer)player);
	}

	private static bool IsCooldownReady(string moduleId, float cooldown)
	{
		return cooldown <= 0f || Time.time - lastFireTimes[moduleId] >= cooldown;
	}

	private static bool CanToggleSelection(string moduleId)
	{
		if (Time.time - lastSelectionTimes[moduleId] < SelectionDebounceSeconds)
		{
			return false;
		}

		lastSelectionTimes[moduleId] = Time.time;
		return true;
	}

	private static void RemoveDisconnectedLocks(string moduleId)
	{
		Player lockedTarget = lockedTargets[moduleId];
		if (lockedTarget != null && !PhotonNetwork.PlayerListOthers.Contains(lockedTarget))
		{
			lockedTargets[moduleId] = null;
		}
		multiLockedTargets[moduleId].RemoveWhere(
			player => !PhotonNetwork.PlayerListOthers.Contains(player));
	}

	private static void UpdateLockMarkers(
		string moduleId,
		bool allowSingleTargetLock,
		bool allowMultipleTargetLocks)
	{
		if (allowMultipleTargetLocks)
		{
			UpdateMultiLockMarkers(moduleId);
			HideSingleLockMarker();
		}
		else if (allowSingleTargetLock)
		{
			ShowSingleLockMarker(lockedTargets[moduleId]);
		}
		else
		{
			HideSingleLockMarker();
		}
	}

	private static void ShowSingleLockMarker(Player target)
	{
		VRRig rig = target == null ? null : GetRig(target);
		if (rig == null)
		{
			HideSingleLockMarker();
			return;
		}

		if (singleLockMarker == null)
		{
			singleLockMarker = CreateMarker(0.15f, ColorLocked);
		}
		singleLockMarker.SetActive(true);
		singleLockMarker.transform.position = rig.transform.position + Vector3.up * 0.3f;
		singleLockMarker.GetComponent<Renderer>().material.color = ColorLocked;
	}

	private static void HideSingleLockMarker()
	{
		if (singleLockMarker != null)
		{
			singleLockMarker.SetActive(false);
		}
	}

	private static void UpdateMultiLockMarkers(string moduleId)
	{
		HashSet<Player> selection = multiLockedTargets[moduleId];
		List<GameObject> markers = multiLockMarkers[moduleId];
		while (markers.Count < selection.Count)
		{
			markers.Add(CreateMarker(0.15f, ColorLocked));
		}

		Player[] players = selection.ToArray();
		for (int index = 0; index < markers.Count; index++)
		{
			GameObject marker = markers[index];
			if (marker == null)
			{
				continue;
			}

			VRRig rig = index < players.Length ? GetRig(players[index]) : null;
			if (rig == null)
			{
				marker.SetActive(false);
				continue;
			}

			marker.SetActive(true);
			marker.transform.position = rig.transform.position + Vector3.up * 0.3f;
			marker.GetComponent<Renderer>().material.color = ColorLocked;
		}
	}

	private static GameObject CreateMarker(float scale, Color color)
	{
		GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		marker.transform.localScale = Vector3.one * scale;
		Object.Destroy(marker.GetComponent<SphereCollider>());
		marker.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(color);
		return marker;
	}

	private static void EnsureAimObjects()
	{
		if (pointer == null)
		{
			pointer = CreateMarker(0.1f, ColorIdle);
		}
		if (tracerObject == null)
		{
			tracerObject = new GameObject("GunTracer");
			tracer = tracerObject.AddComponent<LineRenderer>();
			tracer.startWidth = 0.01f;
			tracer.endWidth = 0.01f;
			tracer.useWorldSpace = true;
			tracer.material = ShaderPatch.CreateTransparentMaterial(Color.white);
			tracerPoints = new Vector3[TracerPointCount];
			tracerVelocities = new Vector3[TracerPointCount];
			tracerPhysicsInitialized = false;
		}
	}

	private static void HideAimObjects()
	{
		if (pointer != null)
		{
			pointer.SetActive(false);
		}
		if (tracerObject != null)
		{
			tracerObject.SetActive(false);
		}
	}

	private static void UpdatePointer(Vector3 position, bool isShooting)
	{
		if (pointer == null)
		{
			return;
		}

		pointer.SetActive(PointerEnabled);
		if (!PointerEnabled)
		{
			return;
		}
		pointer.transform.position = position;
		pointer.GetComponent<Renderer>().material.color = isShooting ? ColorShooting : ColorIdle;
	}

	private static void UpdateTracer(Vector3 origin, Vector3 target, bool isShooting)
	{
		if (tracer == null)
		{
			return;
		}

		bool visible = TracerEnabled && CurrentTracer != TracerStyle.Off;
		tracerObject.SetActive(visible);
		if (!visible)
		{
			return;
		}

		Color color = isShooting ? ColorShooting : ColorIdle;
		tracer.startColor = color;
		tracer.endColor = color;

		switch (CurrentTracer)
		{
			case TracerStyle.Wavy:
				RenderWavyTracer(origin, target);
				break;
			case TracerStyle.Wiggle:
				RenderWiggleTracer(origin, target);
				break;
			case TracerStyle.Tether:
				RenderTetherTracer(origin, target);
				break;
			case TracerStyle.Zigzag:
				RenderZigzagTracer(origin, target);
				break;
			default:
				tracer.positionCount = 2;
				tracer.SetPosition(0, origin);
				tracer.SetPosition(1, target);
				break;
		}
	}

	private static void RenderWavyTracer(Vector3 origin, Vector3 target)
	{
		wavePhase += Time.deltaTime * 8f;
		tracer.positionCount = TracerPointCount;
		Vector3 direction = target - origin;
		Vector3 perpendicular = GetPerpendicular(direction.normalized);
		for (int index = 0; index < TracerPointCount; index++)
		{
			float t = index / (TracerPointCount - 1f);
			Vector3 point = Vector3.Lerp(origin, target, t);
			float offset = Mathf.Sin(wavePhase + t * 12f) * 0.03f * Mathf.Sin(t * Mathf.PI);
			tracer.SetPosition(index, point + perpendicular * offset);
		}
	}

	private static void RenderWiggleTracer(Vector3 origin, Vector3 target)
	{
		InitializeTracerPhysics(origin, target);
		tracer.positionCount = TracerPointCount;
		tracerPoints[0] = origin;
		tracerVelocities[0] = Vector3.zero;
		for (int index = 1; index < TracerPointCount; index++)
		{
			float t = index / (TracerPointCount - 1f);
			Vector3 restPoint = Vector3.Lerp(origin, target, t);
			Vector3 previousPull = tracerPoints[index - 1] - tracerPoints[index];
			Vector3 restPull = restPoint - tracerPoints[index];
			tracerVelocities[index] += previousPull.normalized * previousPull.magnitude * 25f * Time.deltaTime;
			tracerVelocities[index] += restPull * (10f * (1f + t)) * Time.deltaTime;
			tracerVelocities[index] *= 1f - 6f * Time.deltaTime;
			tracerVelocities[index] = Vector3.ClampMagnitude(tracerVelocities[index], 15f);
			tracerPoints[index] += tracerVelocities[index] * Time.deltaTime;
		}
		ApplyTracerPoints();
	}

	private static void RenderTetherTracer(Vector3 origin, Vector3 target)
	{
		InitializeTracerPhysics(origin, target);
		tracer.positionCount = TracerPointCount;
		tracerPoints[0] = origin;
		tracerPoints[TracerPointCount - 1] = target;
		tracerVelocities[0] = Vector3.zero;
		tracerVelocities[TracerPointCount - 1] = Vector3.zero;
		for (int index = 1; index < TracerPointCount - 1; index++)
		{
			float t = index / (TracerPointCount - 1f);
			Vector3 restPoint = Vector3.Lerp(origin, target, t);
			Vector3 previousPull = tracerPoints[index - 1] - tracerPoints[index];
			Vector3 nextPull = tracerPoints[index + 1] - tracerPoints[index];
			Vector3 restPull = restPoint - tracerPoints[index];
			tracerVelocities[index] += previousPull.normalized * previousPull.magnitude * 20f * Time.deltaTime;
			tracerVelocities[index] += nextPull.normalized * nextPull.magnitude * 20f * Time.deltaTime;
			tracerVelocities[index] += restPull * 8f * Time.deltaTime;
			tracerVelocities[index] *= 1f - 5f * Time.deltaTime;
			tracerVelocities[index] = Vector3.ClampMagnitude(tracerVelocities[index], 12f);
			tracerPoints[index] += tracerVelocities[index] * Time.deltaTime;
		}
		ApplyTracerPoints();
	}

	private static void RenderZigzagTracer(Vector3 origin, Vector3 target)
	{
		const int pointCount = 11;
		tracer.positionCount = pointCount;
		Vector3 perpendicular = GetPerpendicular((target - origin).normalized);
		tracer.SetPosition(0, origin);
		for (int index = 1; index < pointCount - 1; index++)
		{
			float t = index / (pointCount - 1f);
			Vector3 point = Vector3.Lerp(origin, target, t);
			float offset = (index % 2 == 0 ? 0.04f : -0.04f) * Mathf.Sin(t * Mathf.PI);
			tracer.SetPosition(index, point + perpendicular * offset);
		}
		tracer.SetPosition(pointCount - 1, target);
	}

	private static Vector3 GetPerpendicular(Vector3 direction)
	{
		Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
		return perpendicular.magnitude < 0.1f
			? Vector3.Cross(direction, Vector3.right).normalized
			: perpendicular;
	}

	private static void InitializeTracerPhysics(Vector3 origin, Vector3 target)
	{
		if (tracerPhysicsInitialized)
		{
			return;
		}
		for (int index = 0; index < TracerPointCount; index++)
		{
			float t = index / (TracerPointCount - 1f);
			tracerPoints[index] = Vector3.Lerp(origin, target, t);
			tracerVelocities[index] = Vector3.zero;
		}
		tracerPhysicsInitialized = true;
	}

	private static void ApplyTracerPoints()
	{
		for (int index = 0; index < TracerPointCount; index++)
		{
			tracer.SetPosition(index, tracerPoints[index]);
		}
	}

	private static void PlayTriggerSound(bool triggerPressed)
	{
		if (SoundEnabled && triggerPressed && !triggerWasPressed)
		{
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, false, 0.5f);
		}
	}

	public static void ClearLockedTarget(string moduleId)
	{
		if (lockedTargets.ContainsKey(moduleId))
		{
			lockedTargets[moduleId] = null;
		}
	}

	public static void Release(string moduleId)
	{
		activeModules.Remove(moduleId);
		lastFireTimes.Remove(moduleId);
		lastSelectionTimes.Remove(moduleId);
		lockedTargets.Remove(moduleId);

		if (multiLockMarkers.TryGetValue(moduleId, out List<GameObject> markers))
		{
			foreach (GameObject marker in markers)
			{
				if (marker != null)
				{
					Object.Destroy(marker);
				}
			}
			multiLockMarkers.Remove(moduleId);
		}
		multiLockedTargets.Remove(moduleId);

		if (activeModules.Count > 0)
		{
			return;
		}

		DestroyGlobalObject(ref pointer);
		if (tracer != null)
		{
			Object.Destroy(tracer);
			tracer = null;
		}
		DestroyGlobalObject(ref tracerObject);
		DestroyGlobalObject(ref singleLockMarker);

		foreach (List<GameObject> remainingMarkers in multiLockMarkers.Values)
		{
			foreach (GameObject marker in remainingMarkers)
			{
				if (marker != null)
				{
					Object.Destroy(marker);
				}
			}
			remainingMarkers.Clear();
		}
		multiLockMarkers.Clear();
		triggerWasPressed = false;
		tracerPhysicsInitialized = false;
	}

	private static void DestroyGlobalObject(ref GameObject value)
	{
		if (value != null)
		{
			Object.Destroy(value);
			value = null;
		}
	}

	public static int GetAimLayerMask()
	{
		return ~(
			(1 << transparentFxLayer) |
			(1 << ignoreRaycastLayer) |
			(1 << zoneLayer) |
			(1 << gorillaTriggerLayer) |
			(1 << gorillaBoundaryLayer) |
			(1 << gorillaCosmeticsLayer) |
			(1 << gorillaParticleLayer));
	}

	public static void MarkFired(string moduleId)
	{
		lastFireTimes[moduleId] = Time.time;
	}
}
