using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This script represents the changes that you will need to do in your custom
/// network manager
/// </summary>
public class ArcherenaNetworkManager : NetworkManager {
    // Set this in the inspector
    public UnetGameRoom GameRoom;

    void Awake() {
        if (GameRoom == null) {
            Debug.LogError("Game Room property is not set on NetworkManager");
            return;
        }

        // Subscribe to events
        GameRoom.PlayerJoined += OnPlayerJoined;
        GameRoom.PlayerLeft += OnPlayerLeft;
    }

    private void OnPlayerJoined(UnetMsfPlayer player) {
        // Spawn the player object (https://docs.unity3d.com/Manual/UNetPlayers.html)
        // This is just a dummy example, you'll need to create your own object (or not)
    }

    private void OnPlayerLeft(UnetMsfPlayer player) {
    }

}