using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetworkAnimator))]
public class PlayerManager : NetworkBehaviour {
    public GameObject utilities;
    public CharacterManager[] characters;

    static bool[] characterUsed = new bool[] { false, false };

    PlayerHealth playerHealth;
    PlayerControl playerControl;
    Rigidbody2D rigidBody;
    Animator animator;
    NetworkAnimator networkAnimator;
    CharacterManager character;

    void Awake() {
        playerHealth = GetComponent<PlayerHealth>();
        playerControl = GetComponent<PlayerControl>();
        rigidBody = GetComponent<Rigidbody2D>();
        networkAnimator = GetComponent<NetworkAnimator>();

        // Handle changing character
        int characterIndex = -1;
        for (int i = 0; i < characterUsed.Length; i++) {
            if (!characterUsed[i]) {
                characterUsed[i] = true;
                characterIndex = i;
                break;
            }
        }

        if (characterIndex == -1) {
            characterIndex = Random.Range(0, characters.Length - 1);
        }

        character = characters[characterIndex];
        for (int i = 0; i < characters.Length; i++) {
            if (i != characterIndex) {
                characters[i].gameObject.SetActive(false);
            }
        }
        animator = character.animator;
        playerControl.character = character;
        networkAnimator.animator = animator;
        for (int i = 0; i < animator.parameterCount; i++) {
            networkAnimator.SetParameterAutoSend(i, true);
        }
    }

    void Start () {
        // FOR BOTH LOCAL AND NON-LOCAL PLAYER
        // Set sprite rendering order to the unique netId of this session
        SpriteRenderer[] sprites = character.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites) {
            sprite.sortingOrder = netId.GetHashCode();
        }

        // FOR NON-LOCAL PLAYER ONLY
        if (!isLocalPlayer) {
            // Enable unitities for local player only
            utilities.SetActive(false);
            return;
        }

        // FOR LOCAL PLAYER ONLY
        // Tell main camera to follow this local player instance
        Camera.main.GetComponent<CameraFollow>().setTarget(transform);
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
