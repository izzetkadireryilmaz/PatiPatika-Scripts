using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGridConnection : MonoBehaviour
{
    public static ButtonGridConnection Instance;
    public int ButtonGridX, ButtonGridY;
    public static List<int> AnimalsCell = new List<int>() { };

    private void Awake()
    {
        Instance = this;
    }
}
