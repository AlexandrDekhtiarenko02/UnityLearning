using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class Enemy : MonoBehaviour
{
    private int target = 0;
    [SerializeField]
    Transform exit;
    [SerializeField]
    Transform[] wayPoints;
    [SerializeField]
    float navigation;
    [SerializeField]
    int Health;
    [SerializeField]
    int rewardMoney;

    Transform enemy;
    float navigationTime = 0;
    bool isDead = false;
    Collider2D enemyCollider;
    Animator anim;

    public bool IsDead {
        get
        {
            return isDead;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Transform>();
        Manager.Instance.RegisterEnemy(this);
        enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (wayPoints != null && isDead == false)
        {
            navigationTime += Time.deltaTime;
            if (navigationTime > navigation)
            {
                if (target < wayPoints.Length)
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, wayPoints[target].position, navigationTime);
                }

                else
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, exit.position, navigationTime);
                }
                navigationTime = 0;
            }

        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            if (collision.tag == "MovingPoint")
            {
                target += 1;
            }
            else if (collision.tag == "Finish")
            {
                Manager.Instance.UnRegisterEnemy(this);
                Manager.Instance.TotalEscaped += 1;
                Manager.Instance.playBtnLabel.text = "Play Again!";
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            }
            else if (collision.tag == "ProjectTitle")
            {
                ProjectTitles newP = collision.gameObject.GetComponent<ProjectTitles>();
                HitEnemy(newP.AttackDamage);
                Destroy(collision.gameObject);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("промах");
        }
       
       
          
 
    }
    public void HitEnemy(int points)
    {
        if (Health - points > 0)
        {
            Health -= points;
            anim.Play("Hurt");
        }
        else
        {
            anim.SetTrigger("didDie");
            Die();
        }
    }
    public void Die()
    {
        enemyCollider.enabled = false;
        isDead = true;
        Manager.Instance.TotalKilled += 1;
        Manager.Instance.AddMoney(rewardMoney);
        Manager.Instance.isLevelOver();
    }
}