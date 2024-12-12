using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private IconHandler iconHandler;
    public int maxNumberOfBirds = 3;
    public int usedNumberOfShots;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        iconHandler = FindObjectOfType<IconHandler>();
    }

    public void UseShot()
    {
        usedNumberOfShots++;
        iconHandler.UseShot(usedNumberOfShots);
    }

    public bool HasEnoughBirds()
    {
        return usedNumberOfShots < maxNumberOfBirds;
    }
}
