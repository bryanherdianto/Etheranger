using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private IconHandler iconHandler;
    private List<Pig> pigs = new List<Pig>();
    
    public int maxNumberOfBirds = 3;
    public int usedNumberOfShots;
    [SerializeField] private GameObject restartScreen;
    [SerializeField] private SlingShotHandler slingShotHandler;
    [SerializeField] private float secondsToWaitBeforeWinCheck = 3f;
    [SerializeField] private Image nextLevelImage;
    [SerializeField] private Image restartImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        iconHandler = FindObjectOfType<IconHandler>();

        nextLevelImage.enabled = false;

        nextLevelImage.rectTransform.anchoredPosition = new Vector2(0, 0);
        restartImage.rectTransform.anchoredPosition = new Vector2(0, 0);

        Pig[] pigsArray = FindObjectsOfType<Pig>();

        for(int i = 0; i < pigsArray.Length; i++)
        {
            pigs.Add(pigsArray[i]);
        }
    }

    public void UseShot()
    {
        usedNumberOfShots++;
        iconHandler.UseShot(usedNumberOfShots);

        CheckForLastShot();
    }

    public bool HasEnoughBirds()
    {
        return usedNumberOfShots < maxNumberOfBirds;
    }

    public void CheckForLastShot()
    {
        if (usedNumberOfShots == maxNumberOfBirds)
        {
            StartCoroutine(CheckAfterWait());
        }
    }

    private IEnumerator CheckAfterWait()
    {
        yield return new WaitForSeconds(secondsToWaitBeforeWinCheck);

        if(pigs.Count == 0)
        {
            Win();
        }

        else
        {
            RestartGame();
        }
    }

    public void RemovePig(Pig pig)
    {
        pigs.Remove(pig);
        CheckForAllDeadPigs();
    }

    public void CheckForAllDeadPigs()
    {
        if(pigs.Count == 0)
        {
            Win();
        }
    }

    #region WinLose

    public void Win()
    {
        restartScreen.SetActive(true);
        slingShotHandler.enabled = false;

        // check if there is any more levels
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels = SceneManager.sceneCountInBuildSettings;
        if (currentSceneIndex + 1 < maxLevels)
        {
            nextLevelImage.enabled = true;
            nextLevelImage.rectTransform.anchoredPosition = new Vector2(70, 0);
            restartImage.rectTransform.anchoredPosition = new Vector2(-70, 0);
        }
    }

    public void RestartGame()
    {
        DOTween.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        DOTween.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion
}
