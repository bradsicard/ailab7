using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Sensors
    [SerializeField]
    GameObject player;

    // Path following
    [SerializeField]
    GameObject[] waypoints;
    int curr = 0;
    int next = 1;

    // Physics
    Rigidbody2D rb;
    float speed = 5.0f;
    float ahead = 2.0f;

    public bool shotgun;
    public bool sniper;

    enum State
    {
        PATROL,
        ATTACK,
        FLEE
    }

    // Behaviour
    State state = State.PATROL;
    float playerDetectRadius = 5.0f;
    float obstacleDetectRadius = 2.5f;

    [SerializeField]
    GameObject bulletPrefab;

    Weapon weapon = null;

    public float health = 25.0f;

    private float timer;
    private bool onShotgun;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        weapon = null;
        timer = 5f;
    }

    void Update()
    {
        UpdateState();
        switch (state)
        {
            case State.PATROL:
                Patrol();
                break;
            case State.ATTACK:
                Attack();
                break;
            case State.FLEE:
                Flee();
                break;
        }
        Avoid();
        UpdateAngularVelocity();

        timer -= 1 * Time.deltaTime;

        if (timer <= 0.0f)
        {
            timer = 5f;
            if (sniper && shotgun)
            {
                weapon = onShotgun ? new Sniper() : new Shotgun();
                onShotgun = onShotgun ? false : true;
                FillWeapon();
                Debug.Log("ENEMY HAS BOTH WEAPONS: SWAPPED TO " + weapon + " (5s)");
            }
        }
    }

    void UpdateState()
    {
        // AB = B - A
        Vector3 toPlayer = (player.transform.position - transform.position).normalized;
        RaycastHit2D playerRaycast = Physics2D.Raycast(transform.position, toPlayer, playerDetectRadius);

        // In the future we may want to separate distance vs line-of-sight
        bool playerDetected = Vector2.Distance(player.transform.position, transform.position) <= playerDetectRadius;
        bool playerVisible = playerRaycast && playerRaycast.collider.CompareTag("Player");
        if (shotgun || sniper)
        {
            state = playerVisible ? State.ATTACK : State.PATROL;
        }
        else
        {
            state = playerVisible ? State.FLEE : State.PATROL;
        }
    }
        void FillWeapon()
        {
            weapon.weaponPrefab = bulletPrefab;
            weapon.owner = gameObject;
            weapon.team = Team.ENEMY;
        }

        // Seek waypoints
        void Patrol()
        {
            Vector2 force = Steering.FollowLine(gameObject, waypoints, ref curr, ref next, ahead, speed);
            rb.AddForce(force);
        }

        // Seek player
        void Attack()
        {
            float distance = Vector2.Distance(this.transform.position, player.transform.position);
            
            Vector2 force = Steering.Seek(rb, player.transform.position, speed);
            rb.AddForce(force);
        if (distance >= 2f && weapon.name == "Sniper")
        {
            weapon.Shoot((player.transform.position - transform.position).normalized);
        }
        else if (distance <= 2f && weapon.name == "Shotgun")
        {
            weapon.Shoot((player.transform.position - transform.position).normalized);
        }

        }

        void Flee()
        {
            Vector2 force = Steering.Flee(rb, player.transform.position, speed);
            rb.AddForce(force);
            if(weapon != null) { weapon.Shoot((player.transform.position - transform.position).normalized); }    
        }

        // Steer away from obstacles
        void Avoid()
        {
            Vector3 dirLeft = Quaternion.Euler(0.0f, 0.0f, 20.0f) * transform.up;
            Vector3 dirRight = Quaternion.Euler(0.0f, 0.0f, -20.0f) * transform.up;
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, dirLeft, obstacleDetectRadius);
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, dirRight, obstacleDetectRadius);
            if (hitLeft && hitLeft.collider.CompareTag("Obstacle"))
            {
                // Turn right
                rb.AddForce(Steering.Seek(rb, transform.right * obstacleDetectRadius, speed));
            }
            else if (hitRight && hitRight.collider.CompareTag("Obstacle"))
            {
                // Turn left
                rb.AddForce(Steering.Seek(rb, -transform.right * obstacleDetectRadius, speed));
            }
        }

        public void Pickup(TYPE type)
        {
            switch (type)
            {
                case TYPE.Shotgun:
                    if (!shotgun) { shotgun = true; weapon = new Shotgun(); FillWeapon(); }
                    break;
                case TYPE.Sniper:
                    if (!sniper) { sniper = true; weapon = new Sniper(); FillWeapon(); }
                    break;
            }
        }

        void UpdateAngularVelocity()
        {
            float av = 250.0f * Mathf.Deg2Rad * Time.deltaTime;
            transform.up = Vector3.RotateTowards(transform.up, rb.linearVelocity.normalized, av, 0.0f).normalized;
            Debug.DrawLine(transform.position, transform.position + transform.up * 5.0f, GetComponent<SpriteRenderer>().color);
        }
    }
