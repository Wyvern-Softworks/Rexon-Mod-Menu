// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.LightningHands
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Lightning Hands", "Visuals", "Electric arc between your hands.", false, 28, ModType.Toggle, false)]
internal class LightningHands : MonoBehaviour
{
	private const int BaseSegmentCount = 6;
	private LineRenderer _line;

	private void OnEnable()
	{
		GameObject lineObject = new GameObject("LightningArc");
		_line = lineObject.AddComponent<LineRenderer>();
		_line.positionCount = BaseSegmentCount + 2;
		_line.useWorldSpace = true;
		((Renderer)_line).material = ShaderPatch.CreateTransparentMaterial(Color.cyan);
		_line.startWidth = 0.015f;
		_line.endWidth = 0.015f;
		_line.startColor = Color.cyan;
		_line.endColor = Color.white;
	}

	private void Update()
	{
		if (_line == null)
		{
			return;
		}
		Vector3 leftPosition = GorillaTagger.Instance.leftHandTransform.position;
		Vector3 rightPosition = GorillaTagger.Instance.rightHandTransform.position;
		float intensity = Mathf.Clamp01(1f - Vector3.Distance(leftPosition, rightPosition) / 2f);
		float jitter = 0.04f * intensity + 0.01f;
		_line.startWidth = 0.01f + 0.01f * intensity;
		_line.endWidth = 0.01f + 0.01f * intensity;
		Color color = Color.Lerp(Color.cyan, Color.white, intensity * 0.5f);
		color.a = 0.5f + 0.5f * intensity;
		((Renderer)_line).material.color = color;
		_line.startColor = color;
		_line.endColor = color;
		int positionCount = BaseSegmentCount + (int)(intensity * 2f) + 2;
		if (_line.positionCount != positionCount)
		{
			_line.positionCount = positionCount;
		}
		_line.SetPosition(0, leftPosition);
		_line.SetPosition(positionCount - 1, rightPosition);
		for (int index = 1; index < positionCount - 1; index++)
		{
			float progress = (float)index / (positionCount - 1);
			Vector3 offset = new Vector3(
				Random.Range(-jitter, jitter),
				Random.Range(-jitter, jitter),
				Random.Range(-jitter, jitter));
			_line.SetPosition(index, Vector3.Lerp(leftPosition, rightPosition, progress) + offset);
		}
	}

	private void OnDisable() => DestroyLine();
	private void OnDestroy() => DestroyLine();

	private void DestroyLine()
	{
		if (_line == null)
		{
			return;
		}
		Object.Destroy(_line.gameObject);
		_line = null;
	}
}
