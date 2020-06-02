using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerControl : MonoBehaviour
{
    [SerializeField]
    float timeBetweenAttack;
    [SerializeField]
    float attackRadius;
    [SerializeField]
    ProjectTitles projectTile;
    Enemy targetEnemy = null;
    float attackCounter;
    bool isAttacing = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        attackCounter -= Time.deltaTime;

        if(targetEnemy == null || targetEnemy.IsDead)
        {
            Enemy nearestEnemy = GetNetherEnemy();

            if(nearestEnemy != null && Vector2.Distance(transform.localPosition, nearestEnemy.transform.localPosition) <= attackRadius)
            {
                targetEnemy = nearestEnemy;
            }
        }
        else
        {
            if(attackCounter <= 0)
            {
                isAttacing = true;

                attackCounter = timeBetweenAttack;
            }
            else
            {
                isAttacing = false;
            }
            if (Vector2.Distance(transform.localPosition, targetEnemy.transform.localPosition) > attackRadius)
            {
                targetEnemy = null;
            }
        }
    }
    public void FixedUpdate()
    {
        if(isAttacing == true)
        {
            Attack();
        }
    }
    public void Attack()
    {
        isAttacing = false;
        ProjectTitles newProjectTile = Instantiate(projectTile) as ProjectTitles;
        newProjectTile.transform.localPosition = transform.localPosition;

        if (targetEnemy == null)
        {
            Destroy(newProjectTile);
        }
        else
        {
            StartCoroutine(MoveProjectTile(newProjectTile));
        }
    }
    IEnumerator MoveProjectTile(ProjectTitles projectile)
    {
        while(GetTargetDistance(targetEnemy) > 0.20f && projectile != null && targetEnemy != null)
        {
            var dir = targetEnemy.transform.position - transform.position;
            var angleDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
            projectile.transform.localPosition = Vector2.MoveTowards(projectile.transform.localPosition,targetEnemy.transform.localPosition, 5f*Time.deltaTime);
            yield return null; 
        }
        if(projectile != null || targetEnemy == null)
        {
            Destroy(projectile);
        }
    }
    private float GetTargetDistance(Enemy thisEnemy)
    {
        if(thisEnemy == null)
        {
            thisEnemy = GetNetherEnemy();
            if(thisEnemy == null)
            {
                return 0f;
            }
        }
        return Mathf.Abs(Vector2.Distance(transform.localPosition, thisEnemy.transform.localPosition));
    }
    private List<Enemy> GetListEnemiesRange()
    {
        List<Enemy> enemiesInRange = new List<Enemy>();

        foreach(Enemy enemy in Manager.Instance.EnemyList)
        {
            if(Vector2.Distance(transform.localPosition, enemy.transform.localPosition) <= attackRadius)
            {
                enemiesInRange.Add(enemy);
            }
        }
        return enemiesInRange;
    }
    private Enemy GetNetherEnemy()
    {
        Enemy nearesEnemy = null;
        float smallesDistance = float.PositiveInfinity;

        foreach(Enemy enemy in GetListEnemiesRange())
        {
            if(Vector2.Distance(transform.localPosition, enemy.transform.localPosition) < smallesDistance)
            {
                smallesDistance = Vector2.Distance(transform.localPosition, enemy.transform.localPosition);
                nearesEnemy = enemy;
            }
        }

        return nearesEnemy;
    }
}
