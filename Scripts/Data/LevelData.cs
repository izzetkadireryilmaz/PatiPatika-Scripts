using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    public int FeedsCount; // o bölümde kullanýlacak yem sayýsý

    public int[] AnimalsCell; // hayvanlarýn instantiate edileceði index numarasý
    public int[] ShedsCell; // kulübelerin instanate edileceði index numarasý
    public int[] RootsCell; // köklerin instanate edileceði index numarasý
    public int[] FlowersCell; // çiçeklerin instanate edileceði index numarasý

    public GameObject Root; // kök prefabý
    public GameObject[] Animals; // o levelde instantiate edilecek hayvanlar
    public GameObject[] Sheds; // o levelde instantiate edilecek kulubeler
    public GameObject[] Flowers; // o levelde instantiate edilecek çiçekler
}