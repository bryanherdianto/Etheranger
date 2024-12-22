using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private IconHandler iconHandler;
    private StarHandler starHandler;
    private List<Enemy> enemies = new List<Enemy>();
    
    private int maxNumberOfProjectiles;
    private int usedNumberOfShots;
    private int starsGained = 3;

    [SerializeField] private GameObject restartScreen;
    [SerializeField] private SlingShotHandler slingShotHandler;
    [SerializeField] private float secondsToWaitBeforeWinCheck = 3f;
    [SerializeField] private Image nextLevelImage;
    [SerializeField] private Image backLevelImage;
    [SerializeField] private Image restartImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        maxNumberOfProjectiles = slingShotHandler.projectilePrefabs.Length;

        iconHandler = FindObjectOfType<IconHandler>();

        nextLevelImage.enabled = false;
        backLevelImage.enabled = false;

        Enemy[] enemiesArray = FindObjectsOfType<Enemy>();

        for(int i = 0; i < enemiesArray.Length; i++)
        {
            enemies.Add(enemiesArray[i]);
        }
    }

    public void UseShot()
    {
        usedNumberOfShots++;
        
        iconHandler.UseShot(usedNumberOfShots);

        CheckForLastShot();
    }

    public bool HasEnoughProjectiles()
    {
        return usedNumberOfShots < maxNumberOfProjectiles;
    }

    public void CheckForLastShot()
    {
        if (usedNumberOfShots == maxNumberOfProjectiles)
        {
            StartCoroutine(CheckAfterWait());
        }
    }

    private IEnumerator CheckAfterWait()
    {
        yield return new WaitForSeconds(secondsToWaitBeforeWinCheck);

        if(enemies.Count == 0)
        {
            Win();
        }

        else
        {
            RestartGame();
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        CheckForAllDeadEnemys();
    }

    public void CheckForAllDeadEnemys()
    {
        if(enemies.Count == 0)
        {
            Win();
        }
    }

    #region WinLose

    public void Win()
    {
        restartScreen.SetActive(true);

        if (enemies.Count == 0 && usedNumberOfShots <= maxNumberOfProjectiles / 2)
        {
            starsGained = 3;
        }
        else if (enemies.Count == 0 && usedNumberOfShots <= maxNumberOfProjectiles / 1.5)
        {
            starsGained = 2;
        }
        else if (enemies.Count == 0 && usedNumberOfShots <= maxNumberOfProjectiles)
        {
            starsGained = 1;
        }

        starHandler = FindObjectOfType<StarHandler>();
        starHandler.GetStars(starsGained);

        slingShotHandler.enabled = false;

        restartImage.rectTransform.anchoredPosition = new Vector2(0, -100);

        // check if there is any more levels
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels = SceneManager.sceneCountInBuildSettings;
        if (currentSceneIndex + 1 < maxLevels)
        {
            nextLevelImage.enabled = true;
            nextLevelImage.rectTransform.anchoredPosition = new Vector2(140, -100);
        }

        if (currentSceneIndex > 1)
        {
            backLevelImage.enabled = true;
            backLevelImage.rectTransform.anchoredPosition = new Vector2(-140, -100);
        }

        PlayerPrefs.SetInt("UnlockedLevel", currentSceneIndex + 1);
        PlayerPrefs.SetInt("ReachedIndex", currentSceneIndex);
    }

    public void RestartGame()
    {
        DOTween.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackLevel()
    {
        DOTween.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void NextLevel()
    {
        DOTween.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion
}
