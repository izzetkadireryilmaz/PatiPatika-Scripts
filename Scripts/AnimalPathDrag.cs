using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class AnimalPathDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private string Type;
    private Sprite AnimalSprite;

    public static AnimalPathDrag Instance;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Transform originalSlot;

    public List<GameObject> path = new List<GameObject>();
    private bool isMoving = false;

    private Vector3 startPos; // sürükleme başlamadan önceki pozisyon
    public static int finish, FeedUsed, ComplatedPath;
    public static bool isPath = true;


    private void Awake()    
    {
        Instance = this;
        isPath = true;
        finish = 0;
        FeedUsed = 0;
        ComplatedPath = 0;
        Debug.Log("cp " + ComplatedPath);
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isMoving) return;

        path.Clear();
        startPos = rectTransform.position; // başlangıç pozisyonunu kaydet
        Type = GetComponent<AnimalSpecies>().Type;
        AnimalSprite = GetComponent<Image>().sprite;
        Debug.Log("Sürükleme başladı.");
        Debug.Log("Type: " + Type);

        GetComponent<Image>().raycastTarget = false;
        GridBuilder.Instance.PopSound.Play();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isMoving) return;

        // HAYVANI PARMAKLA HAREKET ETTİR
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        originalSlot = transform.parent;
        transform.SetParent(transform.root, true); // canvas root'a taşı
        transform.SetAsLastSibling();

        // Parmağın altındaki GridCell'leri bul
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("GridCell") && isPath == true)
            {
                // Yola zaten eklenmişse kontrol etme
                if (path.Contains(result.gameObject))
                    continue;

                // Eğer yol boşsa, ilk hücreyi direkt ekle
                if (path.Count == 0)
                {
                    path.Add(result.gameObject);
                    Debug.Log("Hücre eklendi: " + result.gameObject.name);
                    result.gameObject.GetComponent<Image>().color = GetComponent<AnimalSpecies>().PathColor;
                    result.gameObject.tag = GetComponent<AnimalSpecies>().Type;
                    FeedUsed++;
                    GridBuilder.Instance.FeedsControl();
                }
                else
                {
                    // Yeni hücrenin son hücreye komşu olup olmadığını kontrol et
                    GameObject lastCell = path[path.Count - 1];
                    if (IsAdjacent(lastCell, result.gameObject))
                    {
                        path.Add(result.gameObject);
                        Debug.Log("Hücre eklendi: " + result.gameObject.name);
                        result.gameObject.GetComponent<Image>().color = GetComponent<AnimalSpecies>().PathColor;
                        result.gameObject.tag = GetComponent<AnimalSpecies>().Type;
                        Debug.Log("path count: " + path.Count);
                        FeedUsed++;
                        GridBuilder.Instance.FeedsControl();
                    }
                    else
                    {
                        // Komşu değilse, hiçbir şey yapma ve döngüden çık
                        break;
                    }
                }
            }

            #region Çiçek Kontrolleri
            FlowerControl(result, "DogFlower", "Dog", "Animals_0");
            FlowerControl(result, "CatFlower", "Cat", "Animals_1");
            FlowerControl(result, "RabbitFlower", "Rabbit", "Animals_2");
            FlowerControl(result, "BirdFlower", "Bird", "Animals_3");
            FlowerControl(result, "BearFlower", "Bear", "Animals_4");
            FlowerControl(result, "FoxFlower", "Fox", "Animals_5");
            FlowerControl(result, "FrogFlower", "Frog", "Animals_6");
            FlowerControl(result, "TurtleFlower", "Turtle", "Animals_7");
            #endregion
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isMoving) return;

        Debug.Log("Sürükleme bitti. Yol uzunluğu: " + path.Count);
        transform.SetParent(originalSlot, true);
        GetComponent<Image>().raycastTarget = true;

        // Bir hücreye bırakılmazsa
        if (eventData.pointerCurrentRaycast.gameObject == null || (eventData.pointerCurrentRaycast.gameObject.tag != GetComponent<AnimalSpecies>().Type + "Shed") || eventData.pointerCurrentRaycast.gameObject.transform.parent.tag != GetComponent<AnimalSpecies>().Type)
        {
            Debug.Log("first wrong");
            WrongTransform(eventData);
            return;
        }

        // Eğer yol boşsa, hayvanı başlangıç pozisyonuna geri getir.
        if (path.Count == 0)
        {
            Debug.Log("second wrong");
            WrongTransform(eventData);
            return;
        }

        // Bir hücreye bırakılırsa
        rectTransform.position = startPos;
        ComplatedPath += path.Count;
        StartCoroutine(MoveAlongPath());
    }

    private IEnumerator MoveAlongPath()
    {
        isMoving = true;

        foreach (GameObject cell in path)
        {
            Vector3 targetPos = cell.transform.position;

            GridBuilder.Instance.FootStepSound.Play();
            while (Vector3.Distance(rectTransform.position, targetPos) > 0.1f)
            {
                rectTransform.position = Vector3.MoveTowards(rectTransform.position, targetPos, Time.deltaTime * 500f);
                yield return null;
            }

            rectTransform.position = targetPos;
            yield return new WaitForSeconds(0.05f);
        }

        // Hayvan yolun sonuna ulaştığında son hücrenin tag'ını kendi tag'ına çevir
        if (path.Count > 0)
        {
            // Bu kısım oyun mantığınıza göre değişebilir.
            // Örneğin, hedef hücrenin tag'ını değiştirebilirsiniz.
        }

        finish++;
        GridBuilder.Instance.CorrectShedSound.Play();
        GridBuilder.Instance.FootStepSound.Stop();
        GridBuilder.Instance.FinishControl();
        path.Clear();
        isMoving = false;
    }

    void WrongTransform(PointerEventData eventData)
    {
        Debug.Log("Geçerli hücreye bırakılmadı veya yol oluşturulmadı, resetleniyor.");
        GetComponent<AnimalSpecies>().Type = Type;
        GetComponent<Image>().sprite = AnimalSprite;
        // Renkleri ve tag'ları sıfırla
        foreach (var cell in path)
        {
            cell.GetComponent<Image>().color = Color.white; // varsayılan renge dön
            cell.tag = "GridCell"; // tag'ı geri al
        }

        rectTransform.position = startPos; // hayvanı geri al
        isPath = true;
        FeedUsed = 0;
        FeedUsed += ComplatedPath;
        Debug.Log("FeedUsed: " + FeedUsed);
        GridBuilder.Instance.FeedsControl();
        path.Clear();
    }

    // İki hücrenin komşu olup olmadığını kontrol eden yardımcı fonksiyon
    private bool IsAdjacent(GameObject cell1, GameObject cell2)
    {
        float tolerance = 1.0f;

        RectTransform rt1 = cell1.GetComponent<RectTransform>();
        RectTransform rt2 = cell2.GetComponent<RectTransform>();

        Vector2 pos1 = rt1.anchoredPosition;
        Vector2 pos2 = rt2.anchoredPosition;

        float distanceX = Mathf.Abs(pos1.x - pos2.x);
        float distanceY = Mathf.Abs(pos1.y - pos2.y);

        // 🔑 Hücre genişliği ve yüksekliği ayrı alınmalı
        float cellWidth = rt1.sizeDelta.x;
        float cellHeight = rt1.sizeDelta.y;

        bool isHorizontalAdjacent = Mathf.Abs(distanceX - cellWidth) < tolerance && distanceY < tolerance;
        bool isVerticalAdjacent = Mathf.Abs(distanceY - cellHeight) < tolerance && distanceX < tolerance;

        return isHorizontalAdjacent || isVerticalAdjacent;
    }

    public void FlowerControl(RaycastResult result,string FlowerTag, string TypeTag, string NewSpriteName)
    {
        if (result.gameObject.CompareTag(FlowerTag))
        {
            GetComponent<AnimalSpecies>().Type = TypeTag;
            Sprite NewSprite = GridBuilder.Instance.AnimalsSprite.Find(s => s.name == NewSpriteName);
            gameObject.GetComponent<Image>().sprite = NewSprite;
            GridBuilder.Instance.PoofSound.Play();
        }
    }
}