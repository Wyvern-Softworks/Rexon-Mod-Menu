// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.WriteNameGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Write Name Gun", "Super Infection/Casual [MASTERCLIENT]", "Writes your name with gadgets.", false, 35, ModType.Toggle, false)]
internal class WriteNameGun : MonoBehaviour
{
	private const string GunKey = "WriteNameGun";
	private const float SpawnIntervalSeconds = 3f;
	private const float PixelSpacing = 0.2f;
	private const float CharacterSpacing = 1.2f;

	private static readonly string[] GadgetResourceNames =
	{
		"SI_Resource_WeirdGear",
		"SI_Resource_BouncySand",
		"SI_Resource_FloppyMetal",
		"SI_Resource_VibratingSpring"
	};

	private static readonly IReadOnlyDictionary<char, string[]> Glyphs =
		new Dictionary<char, string[]>
		{
			['A'] = Rows("01110/10001/11111/10001/10001"),
			['B'] = Rows("11110/10001/11110/10001/11110"),
			['C'] = Rows("01111/10000/10000/10000/01111"),
			['D'] = Rows("11110/10001/10001/10001/11110"),
			['E'] = Rows("11111/10000/11110/10000/11111"),
			['F'] = Rows("11111/10000/11110/10000/10000"),
			['G'] = Rows("01111/10000/10111/10001/01110"),
			['H'] = Rows("10001/10001/11111/10001/10001"),
			['I'] = Rows("11111/00100/00100/00100/11111"),
			['J'] = Rows("00001/00001/00001/10001/01110"),
			['K'] = Rows("10001/10010/11100/10010/10001"),
			['L'] = Rows("10000/10000/10000/10000/11111"),
			['M'] = Rows("10001/11011/10101/10001/10001"),
			['N'] = Rows("10001/11001/10101/10011/10001"),
			['O'] = Rows("01110/10001/10001/10001/01110"),
			['P'] = Rows("11110/10001/11110/10000/10000"),
			['Q'] = Rows("01110/10001/10001/10010/01101"),
			['R'] = Rows("11110/10001/11110/10010/10001"),
			['S'] = Rows("01111/10000/01110/00001/11110"),
			['T'] = Rows("11111/00100/00100/00100/00100"),
			['U'] = Rows("10001/10001/10001/10001/01110"),
			['V'] = Rows("10001/10001/10001/01010/00100"),
			['W'] = Rows("10001/10001/10101/11011/10001"),
			['X'] = Rows("10001/01010/00100/01010/10001"),
			['Y'] = Rows("10001/01010/00100/00100/00100"),
			['Z'] = Rows("11111/00010/00100/01000/11111"),
			['0'] = Rows("01110/10011/10101/11001/01110"),
			['1'] = Rows("00100/01100/00100/00100/01110"),
			['2'] = Rows("01110/10001/00110/01000/11111"),
			['3'] = Rows("11110/00001/01110/00001/11110"),
			['4'] = Rows("10010/10010/11111/00010/00010"),
			['5'] = Rows("11111/10000/11110/00001/11110"),
			['6'] = Rows("01110/10000/11110/10001/01110"),
			['7'] = Rows("11111/00001/00010/00100/00100"),
			['8'] = Rows("01110/10001/01110/10001/01110"),
			['9'] = Rows("01110/10001/01111/00001/01110"),
			[' '] = Rows("00000/00000/00000/00000/00000")
		};

	private readonly List<(Vector3 position, Quaternion rotation)> _pixelTransforms =
		new List<(Vector3 position, Quaternion rotation)>();

	private float _lastSpawnTime;

	private void Update()
	{
		try
		{
			if (!PhotonNetwork.InRoom ||
				!GameNetworkUtilities.IsSuperInfectionMode() ||
				!PhotonNetwork.IsMasterClient)
			{
				return;
			}

			GameNetworkUtilities.CacheGameEntityTypeIds();
			GunController.GunResult gun =
				GunController.GetGunResult(GunKey, targetPlayers: false);

			if (!gun.IsActive || !gun.IsShooting || !gun.CanFire)
			{
				return;
			}

			Vector3 hitPoint = gun.Hit.point;
			if (hitPoint == Vector3.zero)
			{
				return;
			}

			string textToWrite = ResolveTextToWrite(PhotonNetwork.LocalPlayer.NickName);
			BuildPixelTransforms(
				textToWrite,
				hitPoint + new Vector3(0f, 2f, 0f),
				Quaternion.LookRotation(GTPlayer.Instance.headCollider.transform.forward));

			if (Time.time > _lastSpawnTime + SpawnIntervalSeconds && _pixelTransforms.Count > 0)
			{
				SpawnGadgets();
			}
		}
		finally
		{
			GunController.Release(GunKey);
		}
	}

	private void BuildPixelTransforms(string text, Vector3 origin, Quaternion rotation)
	{
		_pixelTransforms.Clear();
		for (int characterIndex = 0; characterIndex < text.Length; characterIndex++)
		{
			char character = char.ToUpperInvariant(text[characterIndex]);
			if (!Glyphs.TryGetValue(character, out string[] rows))
			{
				continue;
			}

			for (int row = 0; row < rows.Length; row++)
			{
				for (int column = 0; column < rows[row].Length; column++)
				{
					if (rows[row][column] != '1')
					{
						continue;
					}

					Vector3 localOffset = new Vector3(
						column * PixelSpacing + characterIndex * CharacterSpacing,
						-row * PixelSpacing,
						0f);
					_pixelTransforms.Add((origin + rotation * localOffset, rotation));
				}
			}
		}
	}

	private void SpawnGadgets()
	{
		_lastSpawnTime = Time.time;
		GunController.MarkFired(GunKey);

		List<GameEntityCreateData> entities = new List<GameEntityCreateData>();
		foreach (string resourceName in GadgetResourceNames)
		{
			if (!GameNetworkUtilities.EntityTypeIdsByName.TryGetValue(
				resourceName,
				out int entityTypeId))
			{
				continue;
			}

			foreach ((Vector3 position, Quaternion rotation) pixel in _pixelTransforms)
			{
				entities.Add(new GameEntityCreateData
				{
					entityTypeId = entityTypeId,
					position = pixel.position,
					rotation = pixel.rotation,
					createData = 0L
				});
			}
		}

		GameNetworkUtilities.SpawnGameEntities(entities);
		_pixelTransforms.Clear();
	}

	private static string ResolveTextToWrite(string playerName)
	{
		playerName ??= string.Empty;
		return playerName.ToLowerInvariant() switch
		{
			"hardr" => "NIGGER",
			"lightr" => "NIGGA",
			"kname" => "KKK",
			"invite" => ".GGREXON",
			_ => playerName
		};
	}

	private static string[] Rows(string encodedRows)
	{
		return encodedRows.Split('/');
	}

	private void OnDisable()
	{
		_pixelTransforms.Clear();
		GunController.Release(GunKey);
	}
}
