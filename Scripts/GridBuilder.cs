using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridBuilder : MonoBehaviour
{
    public static GridBuilder Instance;
    public GameObject GridCellPrefab; // UI hücre prefabı (Image/Button)
    public RectTransform GridParent;  // Canvas altındaki boş parent
    public List<Sprite> AnimalsSprite = new List<Sprite> { }; 
    int CellCount;

    public LevelDatabase levelDatabase;
    private LevelData currentLevelData;
    public TMP_Text FeedCount;

    public Canvas WinCanvas;



    public AudioSource CorrectShedSound, PopSound, PoofSound, FootStepSound;

    private void Awake()
    {
        //MusicManager.instance.PlayGameMusic();

        Instance = this;
        int levelIndex = PlayerPrefs.GetInt("ComplatedLevel", 0);
        currentLevelData = levelDatabase.allLevels[levelIndex];
    }
    void Start()
    {
        int x = PlayerPrefs.GetInt("GridX", 2); // Default 2
        int y = PlayerPrefs.GetInt("GridY", 2); // Default 2
         
        GenerateGrid(x, y);
        FeedText();
    }

    /*
    void GenerateGrid(int width, int height)
    {
        float cellSize = 150f; // px boyutu
        float spacing = 0f;
        float offset = cellSize + spacing;

        float startX = -(width - 1) * offset / 2f;
        float startY = -(height - 1) * offset / 2f;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                CellCount++;
                GameObject cell = Instantiate(GridCellPrefab, GridParent);
                RectTransform rt = cell.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(startX + i * offset, startY + j * offset);

                GridCell gridCell = cell.GetComponent<GridCell>();
                gridCell.Init(i, j);

                int SpawnCount = 0;
                int ShedSpawnCount = 0;
                int RootSpawnCount = 0;
                int FlowerSpawnCount = 0;
                BuildTheGrid(SpawnCount, cell, currentLevelData.AnimalsCell, currentLevelData.Animals);
                BuildTheGrid(ShedSpawnCount, cell, currentLevelData.ShedsCell, currentLevelData.Sheds);
                if (currentLevelData.FlowersCell != null)
                {
                    BuildTheGrid(FlowerSpawnCount, cell, currentLevelData.FlowersCell, currentLevelData.Flowers);
                }
                if (currentLevelData.RootsCell != null)
                {
                    foreach (int RootCellIndex in currentLevelData.RootsCell)
                    {
                        if (CellCount == RootCellIndex)
                        {
                            GameObject Root = Instantiate(currentLevelData.Root, cell.transform);
                            Root.transform.parent.tag = "Root";
                            Root.transform.SetAsLastSibling();

                            RectTransform RootRT = Root.GetComponent<RectTransform>();
                            if (RootRT != null)
                                RootRT.anchoredPosition = Vector2.zero;

                            Root.transform.localScale = Vector3.one;
                            break;
                        }
                        RootSpawnCount++;
                    }
                }
                
            }
        }
    }
    */
    void GenerateGrid(int width, int height)
    {
        // Grid alanının boyutlarını al
        float areaWidth = GridParent.rect.width;
        float areaHeight = GridParent.rect.height;

        // Hücre boyutunu hesapla
        float cellWidth = areaWidth / width;
        float cellHeight = areaHeight / height;

        // Hücreleri gridArea'nın içine yerleştir
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                CellCount++;

                // Hücre oluştur
                GameObject cell = Instantiate(GridCellPrefab, GridParent);
                RectTransform rt = cell.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(cellWidth, cellHeight);

                float posX = -areaWidth / 2f + (i + 0.5f) * cellWidth;
                float posY = -areaHeight / 2f + (j + 0.5f) * cellHeight;
                rt.anchoredPosition = new Vector2(posX, posY);

                // Hücreyi initialize et
                GridCell gridCell = cell.GetComponent<GridCell>();
                gridCell.Init(i, j);

                #region Hücrelere objeleri yerleştiriyoruz.
                int SpawnCount = 0;
                int ShedSpawnCount = 0;
                int RootSpawnCount = 0;
                int FlowerSpawnCount = 0;

                BuildTheGrid(SpawnCount, cell, currentLevelData.AnimalsCell, currentLevelData.Animals);
                BuildTheGrid(ShedSpawnCount, cell, currentLevelData.ShedsCell, currentLevelData.Sheds);

                if (currentLevelData.FlowersCell != null)
                {
                    BuildTheGrid(FlowerSpawnCount, cell, currentLevelData.FlowersCell, currentLevelData.Flowers);
                }

                if (currentLevelData.RootsCell != null)
                {
                    foreach (int RootCellIndex in currentLevelData.RootsCell)
                    {
                        if (CellCount == RootCellIndex)
                        {
                            GameObject Root = Instantiate(currentLevelData.Root, cell.transform);
                            Root.transform.SetAsLastSibling();

                            RectTransform RootRT = Root.GetComponent<RectTransform>();
                            if (RootRT != null)
                                RootRT.anchoredPosition = Vector2.zero;

                            Root.transform.localScale = Vector3.one;
                            Root.tag = "Root";
                            break;
                        }
                        RootSpawnCount++;
                    }
                }
                #endregion

            }
        }
    }
    public void FinishControl()
    {
        if (AnimalPathDrag.finish == currentLevelData.Animals.Length)
        {
            Debug.Log("Bitti");
            int ComplatedLevel = PlayerPrefs.GetInt("ComplatedLevel");
            int NewLevel = ComplatedLevel + 1;
            PlayerPrefs.SetInt("ComplatedLevel", NewLevel);
            //SceneManager.LoadScene("LevelsScene");
            WinCanvas.gameObject.SetActive(true);
        }
    }


    public void FeedsControl()
    {
        if (AnimalPathDrag.FeedUsed >= currentLevelData.FeedsCount)
        {
            AnimalPathDrag.isPath = false;
            Debug.Log("isPath: "+ AnimalPathDrag.isPath);
        }
        Debug.Log("FeedsControl içinde");
        FeedCount.text = (currentLevelData.FeedsCount - AnimalPathDrag.FeedUsed).ToString();
    }

    public void FeedText()
    {
        FeedCount.text = (currentLevelData.FeedsCount - AnimalPathDrag.FeedUsed).ToString();
    }


    public void BuildTheGrid(int SpawnCount, GameObject Cell, int[] CellData, GameObject[] Objects)
    {
        foreach (int CellIndex in CellData)
        {
            if (CellCount == CellIndex)
            {
                GameObject InstantiateObject = Instantiate(Objects[SpawnCount], Cell.transform);

                InstantiateObject.transform.SetAsLastSibling();

                RectTransform ioRT = InstantiateObject.GetComponent<RectTransform>();
                if (ioRT != null)
                    ioRT.anchoredPosition = Vector2.zero;

                InstantiateObject.transform.localScale = Vector3.one;
                break;
            }
            SpawnCount++;
        }
    }
}
