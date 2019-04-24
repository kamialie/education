using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public int health = 3;

    public event Action<Player> onPlayerDeath;

    void collidedWithEnemy(Enemy enemy)
    {
        enemy.Attack(this);
        if (health <= 0)
        {
            if (onPlayerDeath != null)
            {
                onPlayerDeath(this);
            }
        }
    }

    void OnCollisionEnter(Collision col) // triggers when two objects collide, col argument contains info like contact point and impact velocities
    {
        Enemy enemy = col.collider.gameObject.GetComponent<Enemy>();
        if (enemy)
            collidedWithEnemy(enemy);
    }
}
