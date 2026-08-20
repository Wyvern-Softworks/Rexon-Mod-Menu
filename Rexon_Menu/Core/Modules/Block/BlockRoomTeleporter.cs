// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Block.BlockRoomTeleporter
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections;
using GorillaLocomotion;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Block;

public static class BlockRoomTeleporter
{
	private static bool _collidersAreShrunk;

	public static IEnumerator TeleportSequence()
	{
		const string mallToBlocksPath = "Environment Objects/LocalObjects_Prefab/MallToMonkeBlocks";
		const string floorCollisionPath = "Environment Objects/MonkeBlocksRoomPersistent/RoomGeo/BuilderFactory (1)/BuilderFactory_FloorCollision/Cube";

		if (!_collidersAreShrunk)
		{
			foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
			{
				collider.transform.localScale /= 10000f;
			}
			_collidersAreShrunk = true;
		}

		GTPlayer.Instance.transform.position += new Vector3(0f, 30f, 0f);
		yield return new WaitForSeconds(0.2f);

		GameObject mallToBlocks = GameObject.Find(mallToBlocksPath);
		if (mallToBlocks != null)
		{
			mallToBlocks.SetActive(true);
		}

		GameObject floorCollision = GameObject.Find(floorCollisionPath);
		if (floorCollision != null)
		{
			floorCollision.SetActive(true);
		}

		yield return new WaitForSeconds(1.6f);
		GTPlayer.Instance.transform.position = new Vector3(-122.2268f, 20.7753f, -220.8281f);
		yield return new WaitForSeconds(0.4f);

		if (_collidersAreShrunk)
		{
			foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
			{
				collider.transform.localScale *= 10000f;
			}
			_collidersAreShrunk = false;
		}

		if (mallToBlocks != null)
		{
			mallToBlocks.SetActive(true);
		}
		if (floorCollision != null)
		{
			floorCollision.SetActive(true);
		}
	}
}
