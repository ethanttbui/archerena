using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControl : NetworkBehaviour {

    public float walkingSpeed = 15f;
    public float runningSpeed = 20f;
    public float jumpSpeed = 35f;
    public float offGroundAcceleration = 1.5f;
    public float arrowAccelertation = 2f;
    public float aimingAngleBound = 20f;
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask obstacleLayerMask;
    public GameObject arrowPrefab;
    public Slider arrowStrengthBar;
    public CharacterManager character;

    bool onGround = true;
    bool onWall = false;
    float arrowSpeed = 0f;
    Rigidbody2D rigidBody;
    Transform arrowSpawn;
    Transform upperBody;
    Animator animator;

    [SyncVar]
    float aimingAngle = 0f;

    [SyncVar]
    Vector3 localScale = Vector3.one;

    void Awake () {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start() {
        arrowSpawn = character.arrowSpawn;
        upperBody = character.upperBody;
        animator = character.animator;

        arrowStrengthBar.value = 0f;
        transform.localScale = localScale;
    }

    void FixedUpdate() {
        // FOR NON-LOCAL PLAYER ONLY
        if (!isLocalPlayer) {
            return;
        }

        // FOR LOCAL PLAYER ONLY
        // Check if the player is on ground or on wall
        onGround = Physics2D.OverlapCircle(groundCheck.position, 0.3f, obstacleLayerMask);
        animator.SetBool("OnGround", onGround);
        onWall = Physics2D.OverlapCircle(wallCheck.position, 0.3f, obstacleLayerMask);
        animator.SetBool("OnWall", onWall);
        animator.SetFloat("VerticalSpeed", rigidBody.velocity.y);

        // Horizontal movements on ground
        // Run if left shift is held down and player is not drawing bow
        // Walk otherwise
        float horizMove = Input.GetAxisRaw("Horizontal");
        if (onGround) {
            float speed = Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(0) ? horizMove * runningSpeed : horizMove * walkingSpeed;
            animator.SetFloat("HorizontalSpeed", Mathf.Abs(speed));
            rigidBody.velocity = new Vector2(speed, rigidBody.velocity.y);
        } 
        // Horizontal movements off ground
        else if (Mathf.Abs(rigidBody.velocity.x) < 10f) {
            rigidBody.velocity += Vector2.right * horizMove * offGroundAcceleration;
        }

        // Change facing direction
        // Need to sync the local scale over the network
        if (horizMove * localScale.x < 0) {
            Vector3 scale = Vector3.one;
            scale.x = localScale.x * -1f;
            CmdSetLocalScale(scale);
            localScale = scale;
        }

        // Player should fall down faster than jump up (triple the effect of gravity)
        // Only apply when the player is not on wall
        if (!onWall && rigidBody.velocity.y < 0) {
            rigidBody.velocity += Vector2.up * Physics2D.gravity.y * Time.deltaTime * 2;
        }

        // Increase arrow speed over time when player is drawing bow
        if (Input.GetKey(KeyCode.Mouse0)) {
            if (arrowSpeed < ArrowManager.maxArrowSpeed) {
                arrowSpeed += arrowAccelertation;
                arrowStrengthBar.value = arrowSpeed / ArrowManager.maxArrowSpeed;
            }
        } else if (arrowStrengthBar.value > 0) {
            arrowStrengthBar.value -= 0.1f;
        }
    }

    void Update() {
        // FOR BOTH LOCAL AND NON-LOCAL PLAYER
        transform.localScale = localScale;

        // FOR NON-LOCAL PLAYER ONLY
        if (!isLocalPlayer) {
            return;
        }

        // FOR LOCAL PLAYER ONLY
        // Jump when space is pressed and the player is on ground or on wall
        if ((onGround || onWall) && (Input.GetKeyDown(KeyCode.Space))) {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
        }

        // Start drawing bow on left mouse down
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            animator.SetBool("DrawingBow", true);
        }

        // Get the aiming angle when left mouse is held down
        if (Input.GetMouseButton(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Atan2(mousePos.y - upperBody.position.y, Mathf.Abs(mousePos.x - upperBody.position.x)) * Mathf.Rad2Deg;
            angle = angle <= aimingAngleBound ? angle : aimingAngleBound;
            angle = angle >= -aimingAngleBound ? angle : -aimingAngleBound;
            CmdSetAimingAngle(angle);
        }

        // Shoot an arrow on left mouse up
        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            // Only spawn the arrow if arrowSpeed is large enough
            if (arrowSpeed >= ArrowManager.minArrowSpeed) {
                Vector2 velocity;
                if (localScale.x > 0) {
                    velocity = arrowSpawn.right * arrowSpeed;
                } else {
                    velocity = -arrowSpawn.right * arrowSpeed;
                }
                CmdShoot(netId.GetHashCode(), velocity, arrowSpawn.position, arrowSpawn.rotation);
            }

            // Reset variables
            animator.SetBool("DrawingBow", false);
            arrowSpeed = 0f;
            CmdSetAimingAngle(0f);
        }
    }

    void LateUpdate() {
        // Aiming rotation has to be set on LateUpdate
        upperBody.Rotate(new Vector3(0f, 0f, aimingAngle), Space.Self);
    }

    [Command]
    void CmdSetLocalScale(Vector3 scale) {
        localScale = scale;
    }

    [Command]
    void CmdSetAimingAngle(float angle) {
        aimingAngle = angle;
    }

    [Command]
    void CmdShoot(int shooterId, Vector2 velocity, Vector3 arrowPos, Quaternion arrowRot) {
        GameObject arrow = Instantiate(arrowPrefab, arrowPos, arrowRot);
        arrow.GetComponent<ArrowManager>().ShooterId = shooterId;
        arrow.GetComponent<Rigidbody2D>().velocity = velocity;
        RpcSpawnArrow(shooterId, velocity, arrowPos, arrowRot);
        //NetworkServer.Spawn(arrow);
    }

    [ClientRpc]
    void RpcSpawnArrow(int shooterId, Vector2 velocity, Vector3 arrowPos, Quaternion arrowRot) {
        GameObject arrow = Instantiate(arrowPrefab, arrowPos, arrowRot);
        arrow.GetComponent<ArrowManager>().ShooterId = shooterId;
        arrow.GetComponent<Rigidbody2D>().velocity = velocity;
    }
}
