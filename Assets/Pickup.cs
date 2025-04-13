using UnityEngine;

public enum TYPE
{
    Shotgun,
    Sniper
}

public class Pickup : MonoBehaviour
{
    
    [SerializeField] TYPE type;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.tag + " PICKED UP " + type);
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().Pickup(type);
            Destroy(this.gameObject);
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().Pickup(type);
            Destroy(this.gameObject);
        }
    }
}
