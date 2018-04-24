using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {
    public const int maxHealth = 100;
    public Slider healthBar;

    [SyncVar(hook = "OnHealthChange")]
    int health = maxHealth;

    public int Health {
        get { return health; }
    }

    void Start () {
        healthBar.value = 1f;
	}
	
    public void TakeDamage(int damage) {
        if (!isServer) {
            return;
        }
        health -= damage;
        if (health <= 0) {
            health = 0;
        }        
    }

    void OnHealthChange (int health) {
        this.health = health;
        healthBar.value = (float)health / maxHealth;
    }
}
