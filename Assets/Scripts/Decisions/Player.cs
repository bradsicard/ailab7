using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    GameObject bulletPrefab;

    Weapon weapon = null;
    public float health = 100.0f;

    [HideInInspector] public bool shotgun;
    [HideInInspector] public bool sniper;

    void Start()
    {
        weapon = new Shotgun();
        FillWeapon();
        
    }

    void Update()
    {
        Move();
        Shoot();

        if (health <= 0.0f)
            Debug.Log("Player died");
    }

    void FillWeapon()
    {
        weapon.weaponPrefab = bulletPrefab;
        weapon.owner = gameObject;
        weapon.team = Team.PLAYER;
    }

    void Move()
    {
        Vector3 direction = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }
        direction = direction.normalized;

        const float speed = 15.0f;
        transform.position += direction * speed * Time.deltaTime;
    }

    void Shoot()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0.0f;
        Vector3 direction = (mouse - transform.position).normalized;
        Debug.DrawLine(transform.position, transform.position + direction * 5.0f, GetComponent<SpriteRenderer>().color);

        if (Input.GetKey(KeyCode.Space))
        {
            weapon.Shoot(direction);
        }
    }
}
