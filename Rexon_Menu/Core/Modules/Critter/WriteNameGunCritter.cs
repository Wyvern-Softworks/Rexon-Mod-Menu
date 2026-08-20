// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.WriteNameGunCritter
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Write Name Gun [CRITTER]", "Critter", "Write your name in honey at aim point.", false, 14, ModType.Toggle, false)]
internal class WriteNameGunCritter : MonoBehaviour
{
	private const string GunKey = "WriteNameGunCritter";
	private const float WriteCooldownSeconds = 3f;
	private const float PixelSpacing = 0.2f;
	private const float CharacterSpacing = 1.2f;
	private const float CharacterDelaySeconds = 0.2f;

	private bool _isWriting;

	private float _lastWriteTime;

	private Coroutine _writeCoroutine;

	private static readonly Dictionary<string, bool[][]> Glyphs;

	private void Update()
	{
		GunController.GunResult gun =
			GunController.GetGunResult(GunKey, targetPlayers: false);

		if (!gun.IsActive ||
			!gun.IsShooting ||
			gun.Hit.collider == null ||
			_isWriting ||
			Time.time <= _lastWriteTime + WriteCooldownSeconds)
		{
			return;
		}

		_lastWriteTime = Time.time;
		_isWriting = true;
		_writeCoroutine = StartCoroutine(WriteText(gun.Hit.point));
	}

	private IEnumerator WriteText(Vector3 hitPoint)
	{
		string playerName = (PhotonNetwork.LocalPlayer?.NickName ?? string.Empty).ToUpperInvariant();
		Vector3 origin = hitPoint + new Vector3(0f, 2f, 0f);
		Quaternion rotation = Quaternion.LookRotation(GTPlayer.Instance.headCollider.transform.forward);

		for (int characterIndex = 0; characterIndex < playerName.Length; characterIndex++)
		{
			if (!Glyphs.TryGetValue(playerName[characterIndex].ToString(), out bool[][] glyph))
			{
				yield return new WaitForSeconds(CharacterDelaySeconds);
				continue;
			}

			for (int row = 0; row < glyph.Length; row++)
			{
				for (int column = 0; column < glyph[row].Length; column++)
				{
					if (!glyph[row][column])
					{
						continue;
					}

					Vector3 localOffset = new Vector3(
						column * PixelSpacing + characterIndex * CharacterSpacing,
						-row * PixelSpacing,
						0f);
					CritterUtilities.SpawnHoney(
						origin + rotation * localOffset,
						rotation * Quaternion.Euler(90f, 0f, 0f),
						0f);
				}
			}

			yield return new WaitForSeconds(CharacterDelaySeconds);
		}

		_isWriting = false;
		_writeCoroutine = null;
	}

	private void OnDisable()
	{
		if (_writeCoroutine != null)
		{
			StopCoroutine(_writeCoroutine);
			_writeCoroutine = null;
		}
		_isWriting = false;
		GunController.Release(GunKey);
	}

	static WriteNameGunCritter()
	{
		Glyphs = new Dictionary<string, bool[][]>
		{
			{
				"A",
				new bool[5][]
				{
					new bool[5] { false, true, true, true, false },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, true, true, true, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true }
				}
			},
			{
				"B",
				new bool[5][]
				{
					new bool[5] { true, true, true, true, false },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, true, true, true, false },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, true, true, true, false }
				}
			},
			{
				"C",
				new bool[5][]
				{
					new bool[5] { false, true, true, true, false },
					new bool[5] { true, false, false, false, false },
					new bool[5] { true, false, false, false, false },
					new bool[5] { true, false, false, false, false },
					new bool[5] { false, true, true, true, false }
				}
			},
			{
				"D",
				new bool[5][]
				{
					new bool[5] { true, true, true, true, false },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, true, true, true, false }
				}
			},
			{
				"E",
				new bool[5][]
				{
					new bool[5] { true, true, true, true, true },
					new bool[5] { true, false, false, false, false },
					new bool[5] { true, true, true, false, false },
					new bool[5] { true, false, false, false, false },
					new bool[5] { true, true, true, true, true }
				}
			},
			{
				"F",
				new bool[5][]
				{
					new bool[5] { true, true, true, true, true },
					new bool[5] { true, false, false, false, false },
					new bool[5] { true, true, true, false, false },
					new bool[5] { true, false, false, false, false },
					new bool[5] { true, false, false, false, false }
				}
			},
			{
				"G",
				new bool[5][]
				{
					new bool[5] { false, true, true, true, false },
					new bool[5] { true, false, false, false, false },
					new bool[5] { true, false, true, true, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { false, true, true, true, false }
				}
			},
			{
				"H",
				new bool[5][]
				{
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, true, true, true, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true }
				}
			},
			{
				"I",
				new bool[5][]
				{
					new bool[5] { true, true, true, true, true },
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, false, true, false, false },
					new bool[5] { true, true, true, true, true }
				}
			},
			{
				"J",
				new bool[5][]
				{
					new bool[5] { false, false, false, false, true },
					new bool[5] { false, false, false, false, true },
					new bool[5] { false, false, false, false, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { false, true, true, true, false }
				}
			},
			{
				"K",
				new bool[5][]
				{
					new bool[5] { true, false, false, true, false },
					new bool[5] { true, false, true, false, false },
					new bool[5] { true, true, false, false, false },
					new bool[5] { true, false, true, false, false },
					new bool[5] { true, false, false, true, false }
				}
			},
			{
				"L",
				new bool[5][]
				{
					new bool[5] { true, false, false, false, false },
					new bool[5] { true, false, false, false, false },
					new bool[5] { true, false, false, false, false },
					new bool[5] { true, false, false, false, false },
					new bool[5] { true, true, true, true, true }
				}
			},
			{
				"M",
				new bool[5][]
				{
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, true, false, true, true },
					new bool[5] { true, false, true, false, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true }
				}
			},
			{
				"N",
				new bool[5][]
				{
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, true, false, false, true },
					new bool[5] { true, false, true, false, true },
					new bool[5] { true, false, false, true, true },
					new bool[5] { true, false, false, false, true }
				}
			},
			{
				"O",
				new bool[5][]
				{
					new bool[5] { false, true, true, true, false },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { false, true, true, true, false }
				}
			},
			{
				"P",
				new bool[5][]
				{
					new bool[5] { true, true, true, true, false },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, true, true, true, false },
					new bool[5] { true, false, false, false, false },
					new bool[5] { true, false, false, false, false }
				}
			},
			{
				"Q",
				new bool[5][]
				{
					new bool[5] { false, true, true, true, false },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, true, false, true },
					new bool[5] { true, false, false, true, false },
					new bool[5] { false, true, true, false, true }
				}
			},
			{
				"R",
				new bool[5][]
				{
					new bool[5] { true, true, true, true, false },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, true, true, true, false },
					new bool[5] { true, false, true, false, false },
					new bool[5] { true, false, false, true, false }
				}
			},
			{
				"S",
				new bool[5][]
				{
					new bool[5] { false, true, true, true, true },
					new bool[5] { true, false, false, false, false },
					new bool[5] { false, true, true, true, false },
					new bool[5] { false, false, false, false, true },
					new bool[5] { true, true, true, true, false }
				}
			},
			{
				"T",
				new bool[5][]
				{
					new bool[5] { true, true, true, true, true },
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, false, true, false, false }
				}
			},
			{
				"U",
				new bool[5][]
				{
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { false, true, true, true, false }
				}
			},
			{
				"V",
				new bool[5][]
				{
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { false, true, false, true, false },
					new bool[5] { false, true, false, true, false },
					new bool[5] { false, false, true, false, false }
				}
			},
			{
				"W",
				new bool[5][]
				{
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, false, false, true },
					new bool[5] { true, false, true, false, true },
					new bool[5] { true, true, false, true, true },
					new bool[5] { true, false, false, false, true }
				}
			},
			{
				"X",
				new bool[5][]
				{
					new bool[5] { true, false, false, false, true },
					new bool[5] { false, true, false, true, false },
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, true, false, true, false },
					new bool[5] { true, false, false, false, true }
				}
			},
			{
				"Y",
				new bool[5][]
				{
					new bool[5] { true, false, false, false, true },
					new bool[5] { false, true, false, true, false },
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, false, true, false, false }
				}
			},
			{
				"Z",
				new bool[5][]
				{
					new bool[5] { true, true, true, true, true },
					new bool[5] { false, false, false, true, false },
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, true, false, false, false },
					new bool[5] { true, true, true, true, true }
				}
			},
			{
				"0",
				new bool[5][]
				{
					new bool[5] { false, true, true, true, false },
					new bool[5] { true, false, false, true, true },
					new bool[5] { true, false, true, false, true },
					new bool[5] { true, true, false, false, true },
					new bool[5] { false, true, true, true, false }
				}
			},
			{
				"1",
				new bool[5][]
				{
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, true, true, false, false },
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, true, true, true, false }
				}
			},
			{
				"2",
				new bool[5][]
				{
					new bool[5] { false, true, true, true, false },
					new bool[5] { true, false, false, false, true },
					new bool[5] { false, false, false, true, false },
					new bool[5] { false, false, true, false, false },
					new bool[5] { true, true, true, true, true }
				}
			},
			{
				"3",
				new bool[5][]
				{
					new bool[5] { true, true, true, true, false },
					new bool[5] { false, false, false, false, true },
					new bool[5] { false, true, true, true, false },
					new bool[5] { false, false, false, false, true },
					new bool[5] { true, true, true, true, false }
				}
			},
			{
				" ",
				new bool[5][]
				{
					new bool[5],
					new bool[5],
					new bool[5],
					new bool[5],
					new bool[5]
				}
			},
			{
				".",
				new bool[5][]
				{
					new bool[5],
					new bool[5],
					new bool[5],
					new bool[5],
					new bool[5] { false, false, true, false, false }
				}
			},
			{
				"/",
				new bool[5][]
				{
					new bool[5] { false, false, false, false, true },
					new bool[5] { false, false, false, true, false },
					new bool[5] { false, false, true, false, false },
					new bool[5] { false, true, false, false, false },
					new bool[5] { true, false, false, false, false }
				}
			}
		};
	}
}
