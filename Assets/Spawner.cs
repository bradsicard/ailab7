using System.Threading;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private GameObject shotgunPref;
    [SerializeField] private GameObject sniperPref;
    private float timer = 3f;

    private void Update()
    {
        timer -= 1f * Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 3f;
            if(!player.shotgun || !enemy.shotgun)
            {
                int random = Random.Range(0, spawnPoints.Length);
                Instantiate(shotgunPref, spawnPoints[random]);
            }
            if (!player.sniper || !enemy.sniper)
            {
                int random = Random.Range(0, spawnPoints.Length);
                Instantiate(sniperPref, spawnPoints[random]);
            }
        }
    }

}
