// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.AntiAFK
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Anti AFK", "Room", "Prevents AFK kick.", false, 5, ModType.Toggle, false)]
internal class AntiAFK : MonoBehaviour
{
	private float _lastKeepAliveTime;

	internal Coroutine _coroutine;

	private void Update()
	{
		if (PhotonNetwork.InRoom && Time.time > _lastKeepAliveTime + 30f && _coroutine == null)
		{
			_lastKeepAliveTime = Time.time;
			if (GorillaTagger.Instance != null && GorillaTagger.Instance.offlineVRRig != null)
			{
				_coroutine = GorillaTagger.Instance.StartCoroutine(Nudge());
			}
		}
	}

	private IEnumerator Nudge()
	{
		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		Transform rigTransform = localRig.transform;
		Vector3 originalPosition = rigTransform.position;
		localRig.enabled = false;
		rigTransform.position = originalPosition + new Vector3(0f, -0.05f, 0f);
		yield return new WaitForSeconds(0.2f);
		rigTransform.position = originalPosition;
		localRig.enabled = true;
		_coroutine = null;
	}

	private void OnDisable()
	{
		if (_coroutine != null)
		{
			GorillaTagger.Instance.StopCoroutine(_coroutine);
			_coroutine = null;
		}
	}
}
