// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SetToDayTime
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Set To Day Time", "World", "Force daytime visuals.", false, 37, ModType.Toggle, false)]
internal class SetToDayTime : MonoBehaviour
{
	internal int _originalTimeIndex;

	private void OnEnable()
	{
		this.StartCoroutine(ApplyDayTime());
	}

	private void SetDayTime()
	{
		if (_originalTimeIndex >= 0 && BetterDayNightManager.instance != null)
		{
			((BetterDayNightManager)BetterDayNightManager.instance).SetTimeOfDay(_originalTimeIndex, false);
			_originalTimeIndex = -1;
		}
	}

	private void OnDisable()
	{
		SetDayTime();
	}

	private IEnumerator ApplyDayTime()
	{
		for (int attempt = 0; attempt < 10; attempt++)
		{
			if (BetterDayNightManager.instance != null)
			{
				_originalTimeIndex = ((BetterDayNightManager)BetterDayNightManager.instance).currentTimeIndex;
				((BetterDayNightManager)BetterDayNightManager.instance).SetTimeOfDay(0, false);
				yield break;
			}

			yield return new WaitForSeconds(0.1f);
		}
	}
}

