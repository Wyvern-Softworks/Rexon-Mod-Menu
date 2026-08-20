// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Utilities.ModManager
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using UnityEngine;

namespace Rexon_Menu.Core.Utilities;

public class ModManager : MonoBehaviour
{
	public static ModManager Instance;

	private void Awake()
	{
		if (Instance != null)
		{
			Object.Destroy(this.gameObject);
			return;
		}
		Instance = this;
		Object.DontDestroyOnLoad(this.gameObject);
	}
}
