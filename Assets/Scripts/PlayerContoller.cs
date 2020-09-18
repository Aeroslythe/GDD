using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerContoller : MonoBehaviour
{
    #region Movement_variables
    public float movespeed;
    float x_input;
    float y_input;
    Vector2 upRight = new Vector2(0.5f, 0.5f);
    Vector2 upLeft = new Vector2(-0.5f, 0.5f);
    Vector2 downRight = new Vector2(0.5f, -0.5f);
    Vector2 downLeft = new Vector2(-0.5f, -0.5f);
    #endregion

    #region Physics_components
    Rigidbody2D PlayerRB;
    #endregion

    #region Attack_variables
    public float Damage;
    public float attackspeed = 1;
    float attackTimer;
    public float hitboxtiming;
    public float endanimationtiming;
    bool isAttacking;
    Vector2 currDirection;
    #endregion

    #region Ability_variables
    float cloakTiming = 1;
    bool isCloaked;
    bool isTransitioning;
    bool isDashing;
    #endregion

    #region Health_variables
    public float maxHealth;
    float currHealth;
    public Slider HPSlider;
    #endregion

    #region Animation_Components
    Animator anim;
    SpriteRenderer sprite; 
    #endregion

    #region Utility_functions
    private void Awake()
    {
        PlayerRB = GetComponent<Rigidbody2D>();

        isTransitioning = false;

        sprite = GetComponent<SpriteRenderer>();
        attackTimer = 0;

        anim = GetComponent<Animator>();

        currHealth = maxHealth;

        HPSlider.value = currHealth / maxHealth;

        isCloaked = false;
    }

    private void Update()
    {
        if (isAttacking)
        {
            return;
        }
        x_input = Input.GetAxisRaw("Horizontal");
        y_input = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.J) && attackTimer <= 0)
        {
            Attack();
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Interact();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (isTransitioning)
            {
                return;
            }
            Dash();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isTransitioning)
            {
                return;
            }
            if (!isCloaked)
            {
                Cloak();
            } else
            {
                deCloak();
            }
        }

        Move();
    }
    #endregion

    #region Movement_functions
    private void Dash()
    {
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        isTransitioning = true;
        movespeed *= 2;
        Debug.Log("Dashing now");
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.1f);
        }
        movespeed = movespeed / 2;
        isTransitioning = false;
        Debug.Log("Done dashing");
        yield return null;
    }

    private void Move()
    {
        anim.SetBool("Moving", true);

        if (x_input > 0 && y_input > 0)
        {
            PlayerRB.velocity = upRight * movespeed;
            currDirection = Vector2.up;
        } else
        if (x_input < 0 && y_input > 0)
        {
            PlayerRB.velocity = upLeft * movespeed;
            currDirection = Vector2.up;
        }
        else
        if (x_input > 0 && y_input < 0)
        {
            PlayerRB.velocity = downRight * movespeed;
            currDirection = Vector2.down;
        }
        else
        if (x_input < 0 && y_input < 0)
        {
            PlayerRB.velocity = downLeft * movespeed;
            currDirection = Vector2.down;
        }
        else
        if (x_input > 0)
        {
            PlayerRB.velocity = Vector2.right * movespeed;
            currDirection = Vector2.right;
        }
        else
        if (x_input < 0)
        {
            PlayerRB.velocity = Vector2.left * movespeed;
            currDirection = Vector2.left;
        }
        else
        if (y_input > 0)
        {
            PlayerRB.velocity = Vector2.up * movespeed;
            currDirection = Vector2.up;
        }
        else
        if (y_input < 0)
        {
            PlayerRB.velocity = Vector2.down * movespeed;
            currDirection = Vector2.down;
        }
        else
        if (x_input == 0 && y_input == 0)
        {
            PlayerRB.velocity = Vector2.zero;
            anim.SetBool("Moving", false);
        }


        anim.SetFloat("DirX", currDirection.x);
        anim.SetFloat("DirY", currDirection.y);
    }
    #endregion

    #region Attack_functions
    private void Attack()
    {
        Debug.Log("Attacking now");
        Debug.Log(currDirection);
        attackTimer = attackspeed;
        // handels animations and hit boxes
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        PlayerRB.velocity = Vector2.zero;

        anim.SetTrigger("Attacking");

        FindObjectOfType<AudioManager>().Play("PlayerAttack");

        yield return new WaitForSeconds(hitboxtiming);
        Debug.Log("Casting Hitbox Now");
        RaycastHit2D[] hits = Physics2D.BoxCastAll(PlayerRB.position + currDirection, Vector2.one, 0f, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log(hit.transform.name);
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("TONS OF DAMAGE");
                hit.transform.GetComponent<EnemyController>().TakeDamage(Damage);
            }
        }
        yield return new WaitForSeconds(hitboxtiming);
        isAttacking = false;

        yield return null;
    }
    #endregion

    #region Ability_functions
    private void Cloak()
    {
        Debug.Log("Cloaking now");
        // handels animations and tags
        StartCoroutine(CloakRoutine());
    }

    IEnumerator CloakRoutine()
    {
        //FindObjectOfType<AudioManager>().Play("PlayerAttack");
        float r = 1f;
        float g = 1f;
        float a = 1f;
        isTransitioning = true;
        for (int i = 0; i < 9; i++)
        {
            //yield return new WaitForSeconds(1);
            r -= .1f;
            g -= .1f;
            a -= .1f;
            yield return new WaitForSeconds(0.1f);
            Debug.Log(sprite.color);
            sprite.material.color = new Color(r, g, 1f, a);
        }
        isCloaked = true;
        isTransitioning = false;
        gameObject.tag = "Invisible"; 
        yield return null;
    }

    private void deCloak()
    {
        Debug.Log("decloaking now");
        // handels animations and tags
        StartCoroutine(deCloakRoutine());
    }

    IEnumerator deCloakRoutine()
    {
        //FindObjectOfType<AudioManager>().Play("PlayerAttack");
        float r = 0f;
        float g = 0f;
        float a = 0f;
        isTransitioning = true;
        for (int i = 0; i < 9; i++)
        {
            //yield return new WaitForSeconds(1);
            r += .1f;
            g += .1f;
            a += .1f;
            yield return new WaitForSeconds(0.1f);
            Debug.Log(sprite.color);
            sprite.material.color = new Color(r, g, 1f, a);
        }
        yield return null;
        isCloaked = false;
        isTransitioning = false;
        gameObject.tag = "Player";
    }
        #endregion

    #region Health_functions

        public void TakeDamage(float value)
    {
        FindObjectOfType<AudioManager>().Play("PlayerHurt");

        currHealth -= value;
        Debug.Log("Health is now:" + currHealth.ToString());

        HPSlider.value = currHealth / maxHealth;

        if (currHealth <= 0)
        {
            FindObjectOfType<AudioManager>().Play("PlayerDeath");
            Die();
        }
    }

    public void Heal(float value)
    {
        currHealth += value;
        currHealth = Mathf.Min(currHealth, maxHealth);
        Debug.Log("Health is now:" + currHealth.ToString());
        HPSlider.value = currHealth / maxHealth;
    }

    //destroys player object and triggers end scene
    private void Die()
    {
        Destroy(this.gameObject);
        GameObject gm = GameObject.FindWithTag("GameController");
        gm.GetComponent<GameManager>().LoseGame();
    }

    #endregion

    #region Interact_functions

    private void Interact()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(PlayerRB.position + currDirection, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0f);
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Chest"))
            {
                hit.transform.GetComponent<chest>().Interact();
            }
        }
    }
    #endregion
}
