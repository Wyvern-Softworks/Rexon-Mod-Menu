// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.ShadowClone
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Shadow Clone", "Visuals", "Delayed ghost copies follow you.", false, 30, ModType.Toggle, false)]
internal class ShadowClone : MonoBehaviour
{
	private const string CloneNamePrefix = "ShadowClone_";
	private const int PositionHistoryLength = 30;
	private const float CloneScale = 0.3f;

	private static readonly int[] CloneFrameDelays = { 10, 20, 30 };
	private static readonly Color CloneColor = new(0.2f, 0.2f, 0.3f, 0.3f);

	private readonly Vector3[] _positionHistory = new Vector3[PositionHistoryLength];
	private readonly GameObject[] _clones = new GameObject[CloneFrameDelays.Length];

	private int _nextHistoryIndex;
	private bool _historyIsFull;

	private void OnEnable()
	{
		_nextHistoryIndex = 0;
		_historyIsFull = false;

		for (int cloneIndex = 0; cloneIndex < _clones.Length; cloneIndex++)
		{
			GameObject clone = GameObject.CreatePrimitive((PrimitiveType)0);
			clone.name = CloneNamePrefix + cloneIndex;
			clone.transform.localScale = Vector3.one * CloneScale;

			Collider collider = clone.GetComponent<Collider>();
			if (collider != null)
			{
				Object.Destroy(collider);
			}

			Renderer renderer = clone.GetComponent<Renderer>();
			if (renderer != null)
			{
				renderer.material = ShaderPatch.CreateTransparentMaterial(CloneColor);
			}

			clone.SetActive(false);
			_clones[cloneIndex] = clone;
		}
	}

	private void Update()
	{
		_positionHistory[_nextHistoryIndex] = GorillaTagger.Instance.offlineVRRig.transform.position;
		_nextHistoryIndex++;
		if (_nextHistoryIndex >= _positionHistory.Length)
		{
			_nextHistoryIndex = 0;
			_historyIsFull = true;
		}

		if (!_historyIsFull)
		{
			return;
		}

		for (int cloneIndex = 0; cloneIndex < _clones.Length; cloneIndex++)
		{
			GameObject clone = _clones[cloneIndex];
			if (clone == null)
			{
				continue;
			}

			int historyIndex = (_nextHistoryIndex - CloneFrameDelays[cloneIndex] + _positionHistory.Length) % _positionHistory.Length;
			clone.transform.position = _positionHistory[historyIndex];
			if (!clone.activeSelf)
			{
				clone.SetActive(true);
			}
		}
	}

	private void OnDisable()
	{
		DestroyClones();
	}

	private void OnDestroy()
	{
		DestroyClones();
	}

	private void DestroyClones()
	{
		for (int cloneIndex = 0; cloneIndex < _clones.Length; cloneIndex++)
		{
			if (_clones[cloneIndex] != null)
			{
				Object.Destroy(_clones[cloneIndex]);
				_clones[cloneIndex] = null;
			}
		}

		_nextHistoryIndex = 0;
		_historyIsFull = false;
	}
}
