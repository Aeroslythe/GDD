using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Movement_variables
    public float movespeed;
    #endregion

    #region Physics_components
    Rigidbody2D EnemyRB;
    CircleCollider2D Collision;
    #endregion

    #region Targeting_variables
    public Transform player;
    #endregion

    #region Attack_variables
    public float explosionDamage;
    public float explosionRadius;
    public GameObject explosionObj;
    #endregion

    #region Health_variables
    public float maxHealth;
    float currHealth;
    #endregion

    #region Unity_functions

    //runs on creation

    private void Awake()
    {
        EnemyRB = GetComponent<Rigidbody2D>();

        Collision = GetComponent<CircleCollider2D>();

        currHealth = maxHealth;

        Collision.enabled = false;
    }

    //runs once every frame

    private void Update()
    {
        //check to see if we know where player is
        if(player == null)
        {
            return;
        }
        if (player.transform.CompareTag("Invisible"))
        {
            EnemyRB.velocity = new Vector2(0f, 0f);
            Collision.enabled = false;
            return;
        }
        Collision.enabled = true;
        Move();
    }

    #endregion

    #region Movement_functions

    //move directly toward player
    private void Move()
    {
        //calc movement vector player position - enemy position = direction of player relative to 
        Vector2 direction = player.position - transform.position;

        EnemyRB.velocity = direction.normalized * movespeed;
    }
    #endregion

    #region Attack_functions

    private void Explode()
    {
        FindObjectOfType<AudioManager>().Play("Explosion");

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Player"))
            {
                //cause damage
                Debug.Log("Hit Player with explosion");

                //spawn Explosion prefab
                Instantiate(explosionObj, transform.position, transform.rotation);
                hit.transform.GetComponent<PlayerContoller>().TakeDamage(explosionDamage);
                Destroy(this.gameObject);
            }
        }
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            Explode();
        }
    }

    #endregion

    #region Health_functions

    public void TakeDamage(float value)
    {
        FindObjectOfType<AudioManager>().Play("GhostHurt");

        currHealth -= value;

        Debug.Log("Health is now" + currHealth.ToString());

        if (currHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
    #endregion
}
