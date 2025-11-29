using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollLevel : MonoBehaviour
{
    public ScrollRect scrollRect;         // Inspector’dan atayacaksýn
    public RectTransform content, view;         // Scroll’un Content objesi
    public RectTransform[] levelButtons;  // Bütün level butonlarýnýn RectTransform’larý

    void Start()
    {
        int lastCompletedLevel = PlayerPrefs.GetInt("ComplatedLevel", 0);

        // Sahneye gelince bu levele kaydýr
        ScrollToLevel(lastCompletedLevel);
    }

    void ScrollToLevel(int levelIndex)  
    {
        // Butonun content içindeki Y pozisyonu (pozitif olsun diye abs alýyoruz)
        float targetY = Mathf.Abs(levelButtons[levelIndex].anchoredPosition.y);

        Vector2 bottomLeft = view.offsetMin;
        Vector2 topRight = view.offsetMax;
        bottomLeft.y = targetY;
        topRight.y = targetY * -1;
    }

}
