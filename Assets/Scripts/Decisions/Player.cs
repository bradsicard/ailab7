using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    GameObject bulletPrefab;

    Weapon weapon = null;
    public float health = 100.0f;

    public bool shotgun;
    public bool sniper;
    private float timer;
    private bool onShotgun;

    void Start()
    {
        weapon = null;
        timer = 5f;
    }

    void Update()
    {
        Move();
        Shoot();
            
        timer -= 1 * Time.deltaTime;

        if(timer <= 0.0f)
        {
            timer = 5f;
            if(sniper && shotgun)
            {
                weapon = onShotgun ? new Sniper() : new Shotgun();
                FillWeapon();
                onShotgun = onShotgun ? false : true;
                Debug.Log("PLAYER HAS BOTH WEAPONS: SWAPPED TO " + weapon + " (5s)");
            }
        }
        
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

        if (Input.GetKey(KeyCode.Space) && weapon != null)
        {
            weapon.Shoot(direction);
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
}
