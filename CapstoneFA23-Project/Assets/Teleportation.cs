using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Teleportation : MonoBehaviour
{
    public Tilemap teleportationTilemap;
    public Vector3 teleportDestination;
    public Tilemap returnTeleportationTilemap;
    public Vector3 returnTeleportDestination;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the player's position in world coordinates
            Vector3Int playerTilePosition = teleportationTilemap.WorldToCell(other.transform.position);

            // Check if the player is on the teleportation tile
            if (teleportationTilemap.GetTile(playerTilePosition) != null)
            {
                // Teleport the player to the destination
                other.transform.position = teleportDestination;
            }

            // Check if the player is on the return teleportation tile
            Vector3Int returnPlayerTilePosition = returnTeleportationTilemap.WorldToCell(other.transform.position);
            if (returnTeleportationTilemap.GetTile(returnPlayerTilePosition) != null)
            {
                // Teleport the player back to the return destination
                other.transform.position = returnTeleportDestination;
            }
        }
    }
}
