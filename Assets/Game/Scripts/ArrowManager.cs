using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArrowManager : MonoBehaviour {
    public const float maxArrowSpeed = 70f;
    public const float minArrowSpeed = 30f;
    public const int baseDamage = 30;
    public float lifeTime = 2f;

    Rigidbody2D rigidBody;
    int shooterId = -1;

    public int ShooterId {
        get { return shooterId; }
        set {
            shooterId = value;
        }
    }

    void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    void Start() {
        // Initialize the arrow's rotation to the direction of its velocity
        transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(rigidBody.velocity, Vector3.forward));
    }

    void FixedUpdate() {
        // Always align the arrow's rotation with the direction of its velocity
        if (rigidBody.velocity.magnitude > 0) {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(rigidBody.velocity, Vector3.forward));
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        // An arrow does not hit its shooter
        if (collider.gameObject.tag == "Player" && collider.gameObject.GetComponentInParent<PlayerControl>().netId.GetHashCode() == shooterId) {
            return;
        }

        // Stick the arrow to the object it collides with
        rigidBody.bodyType = RigidbodyType2D.Kinematic;
        Destroy(gameObject.GetComponent<PolygonCollider2D>());
        transform.Translate(rigidBody.velocity / (2 * maxArrowSpeed), Space.World);
        transform.parent = collider.transform;
        rigidBody.angularVelocity = 0f;
        rigidBody.velocity = Vector2.zero;
    }
}
