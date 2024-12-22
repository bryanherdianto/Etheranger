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
        for (int i = 0; i < starIcons.Length; i++)
        {
            if (i < starsGained)
            {
                starIcons[i].sprite = gainedStarSprite;
            }
            else
            {
                starIcons[i].sprite = notGainedStarSprite; 
            }
        }
    }
}
