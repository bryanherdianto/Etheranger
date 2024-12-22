using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarHandler : MonoBehaviour
{
    [SerializeField] private Image[] starIcons;
    [SerializeField] private Sprite gainedStarSprite;
    [SerializeField] private Sprite notGainedStarSprite;

    public void GetStars(int starsGained)
    {
        StartCoroutine(PopulateStars(starsGained));
    }

    private IEnumerator PopulateStars(int starsGained)
    {
        for (int i = 0; i < starIcons.Length; i++)
        {
            starIcons[i].sprite = notGainedStarSprite;
        }

        for (int i = 0; i < starsGained; i++)
        {
            starIcons[i].sprite = gainedStarSprite;
            yield return new WaitForSeconds(0.5f);
        }
    }
}