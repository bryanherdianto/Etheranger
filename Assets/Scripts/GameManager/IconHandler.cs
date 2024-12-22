using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconHandler : MonoBehaviour
{
    [SerializeField] private Image iconPrefab;
    [SerializeField] private Color usedColor;
    [SerializeField] private SlingShotHandler slingShotHandler;

    private List<Image> icons = new List<Image>();

    private void Start()
    {
        GenerateIcons();
    }

    private void GenerateIcons()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        icons.Clear();

        Projectile[] projectilePrefabs = slingShotHandler.projectilePrefabs;

        foreach (var prefab in projectilePrefabs)
        {
            Image newIcon = Instantiate(iconPrefab, transform);
            newIcon.sprite = prefab.GetComponent<SpriteRenderer>().sprite;
            newIcon.preserveAspect = true;
            icons.Add(newIcon);
        }
    }

    public void UseShot(int shotNumber)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            if (i + 1 == shotNumber)
            {
                icons[i].color = usedColor;
                return;
            }
        }
    }
}
