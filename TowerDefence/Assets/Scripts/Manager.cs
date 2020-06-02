using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameStatus
{
    next, play, gameover, win
}
public class Manager : Loader<Manager>
{
    [SerializeField]
    int totalLevels = 5;
    [SerializeField]
    Text currentMoney;
    [SerializeField]
    Text currentLevel;
    [SerializeField]
    public Text playBtnLabel;
    [SerializeField]
    Button playBtn;
    [SerializeField]
    GameObject spawnPoint;
    [SerializeField]
    GameObject[] enemies;
    [SerializeField]
    int maxEnemiesOnScreen;
    [SerializeField]
    int totalEnemies;
    [SerializeField]
    int enemiesPerSpawn;

    public List<Enemy> EnemyList = new List<Enemy>();
    const float spawnDelay = 3f;
    int levelNumber = 0;
    int totalMoney = 15;
    int totalEscaped = 0;
    int totalKilled = 0;
    int WhichEnemiesToSpawn = 0;
    gameStatus CurrentStatus = gameStatus.play;
    public int TotalEscaped
    {
        get
        {
            return totalEscaped;
        }
        set
        {
            totalEscaped = value;
        }
    }
     public int TotalKilled
    {
        get
        {
            return totalKilled;
        }
        set
        {
            totalKilled = value;
        }
    }
    public void isLevelOver()
    {
       if((TotalKilled == totalEnemies))
        {
            SetCurrentGameState();
            ShowMenu();
        }
    }
    public void SetCurrentGameState()
    {
        if(TotalEscaped > 0)
        {
            CurrentStatus = gameStatus.gameover;
        }
        else if(levelNumber == 0 && TotalKilled == 0)
        {
            CurrentStatus = gameStatus.play;
        }
        else if (levelNumber >= totalLevels)
        {
            CurrentStatus = gameStatus.win;
        }
        else
        {
            CurrentStatus = gameStatus.next;
        }
    }
    public int TotalMoney
    {
        get
        {
            return totalMoney;
        }
        set
        {
            totalMoney = value;
            currentMoney.text = TotalMoney.ToString();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playBtn.gameObject.SetActive(false);
        ShowMenu();
    }

    // Update is called once per frame
    
    IEnumerator Spawn()
    {
        if(enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies)
        {
            for(int i = 0; i < enemiesPerSpawn; i++)
            {
                if(EnemyList.Count < maxEnemiesOnScreen)
                {
                    GameObject newEnemy = Instantiate(enemies[0]) as GameObject;
                    newEnemy.transform.position = spawnPoint.transform.position;
                  
                }
            }
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(Spawn());
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        EnemyList.Add(enemy);
    }
    public void UnRegisterEnemy(Enemy enemy)
    {
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
    public void DestroyEnemies()
    {
        foreach(Enemy enemy in EnemyList)
        {
            Destroy(enemy.gameObject);
        }

        EnemyList.Clear();
    }
    public void PlayBtnPressed()
    {
        switch (CurrentStatus)
        {
            case gameStatus.next:
                levelNumber += 1;
                totalEnemies += levelNumber * 3;
                break;

            default:
                totalEnemies = 5;
                TotalEscaped = 0;
                TotalMoney = 15;
                TotalKilled = 0;
                currentMoney.text = TotalMoney.ToString();
                break;
        }
        DestroyEnemies();
        TotalKilled = 0;
        currentLevel.text = "lv " + (levelNumber + 1);
        StartCoroutine(Spawn());
        playBtn.gameObject.SetActive(false);
    }
    public void AddMoney(int amount)
    {
        TotalMoney += amount;
    }
    public void SubtractMoney(int amount)
    {
        TotalMoney -= amount;
    }
    public void ShowMenu()
    {
        switch (CurrentStatus) {

            case gameStatus.gameover:
                playBtnLabel.text = "Play Again!";
                    break;
            case gameStatus.next:
                playBtnLabel.text = "Next Level!";
                break;
            case gameStatus.play:
                playBtnLabel.text = "Play!";
                break;
            case gameStatus.win:
                playBtnLabel.text = "Play!";
                break;
        }
        playBtn.gameObject.SetActive(true);
    }
}
