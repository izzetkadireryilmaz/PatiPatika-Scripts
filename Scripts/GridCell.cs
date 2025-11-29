using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    public int gridX;
    public int gridY;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnCellClicked);
    }

    public void Init(int x, int y)
    {
        gridX = x;
        gridY = y;
    }

    void OnCellClicked()
    {
        // Hücre merkezini bul
        Vector3 centerPos = transform.position;
    }
}
