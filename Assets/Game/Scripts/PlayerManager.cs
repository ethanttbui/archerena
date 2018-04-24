using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : NetworkBehaviour {
    public GameObject characterRig;
    public GameObject utilities;
    
    PlayerHealth playerHealth;
    PlayerControl playerControl;
    Animator animator;
    Rigidbody2D rigidBody;

    private void Awake() {
        playerHealth = GetComponent<PlayerHealth>();
        playerControl = GetComponent<PlayerControl>();
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start () {
        if (!isLocalPlayer) {
            // Enable unitities for local player only
            utilities.SetActive(false);
        } else {
            // Tell main camera to follow this local player instance
            Camera.main.GetComponent<CameraFollow>().setTarget(transform);
        }

        // Set sprite rendering order to the unique netId of this session
        SpriteRenderer[] sprites = characterRig.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites) {
            sprite.sortingOrder = netId.GetHashCode();
        }
    }

    void Update() {
        // Game over when health is 0
        if (playerHealth.Health == 0) {
            animator.SetBool("Dead", true);
            playerControl.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        // Deduct health if player is hit by an arrow
         if (collider.gameObject.tag == "Arrow") {
            playerHealth.TakeDamage(ArrowManager.baseDamage);
        }
    }
}
