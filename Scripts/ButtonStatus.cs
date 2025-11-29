using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonStatus : MonoBehaviour
{
    public List<Button> Levels;
    public int GridX, GridY;
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("ComplatedLevel"))
        {
            PlayerPrefs.SetInt("ComplatedLevel", 0);
        }
        int ComplatedLevel = PlayerPrefs.GetInt("ComplatedLevel");
        Levels[ComplatedLevel].interactable = true;
        Transform lockChild = Levels[ComplatedLevel].transform.Find("Lock");
        if (lockChild != null)
        {
            lockChild.gameObject.SetActive(false);
        }
    }
    void Start()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            Debug.Log("Fordayým");
            if (Levels[i].interactable == true)
            {
                // Oyun sahnesinde grid oluþturmak için
                GridX = Levels[i].GetComponent<ButtonGridConnection>().ButtonGridX;
                GridY = Levels[i].GetComponent<ButtonGridConnection>().ButtonGridY;
                PlayerPrefs.SetInt("GridX", GridX);
                PlayerPrefs.SetInt("GridY", GridY);
                PlayerPrefs.Save();

                Debug.Log("gridx " + PlayerPrefs.GetInt("GridX"));
                Debug.Log("gridy " + PlayerPrefs.GetInt("GridY"));
                return;
            }
        }
    }

    public void GameButtonClicked()
    {
        SceneManager.LoadScene("GameScene");
    }
}
