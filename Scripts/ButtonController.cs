using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public GameObject SceneLoadObject;

    private void Start()
    {
        if (PlayerPrefs.GetInt("StartAnim") == 1)
        {
            if (SceneLoadObject != null)
            {
                SceneLoadObject.SetActive(true);
                Animator animator = SceneLoadObject.GetComponent<Animator>();
                animator.Play("SceneLoadStartAnim");
                PlayerPrefs.SetInt("StartAnim", 0);
            } else { Debug.Log("SceneLoadObject Boþ"); }
        }
    }
    public void PlayButton()
    {
        SceneLoadObject.SetActive(true);
        SceneLoadObject.transform.SetAsLastSibling();
        Animator animator = SceneLoadObject.GetComponent<Animator>();
        animator.Play("SceneLoadAnim");
        PlayerPrefs.SetInt("StartAnim", 1);
    }

    public void LevelButtonClicked()
    {
        if (SceneLoadObject != null)
        {
            SceneLoadObject.SetActive(true);
            Animator animator = SceneLoadObject.GetComponent<Animator>();
            animator.Play("GameLoadAnim");
            PlayerPrefs.SetInt("StartAnim", 1);
        }
        else { Debug.Log("SceneLoadObject Boþ"); }
    }

    public void TryAgainButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void BackButton()
    {
        GridBuilder.Instance.WinCanvas.gameObject.SetActive(false);
        SceneLoadObject.SetActive(true);
        SceneLoadObject.transform.SetAsLastSibling();
        Animator animator = SceneLoadObject.GetComponent<Animator>();
        animator.Play("SceneLoadAnim");
        PlayerPrefs.SetInt("StartAnim", 1);
    }
    public void NextLevelButton()
    {
        GridBuilder.Instance.WinCanvas.gameObject.SetActive(false);
        SceneLoadObject.SetActive(true);
        SceneLoadObject.transform.SetAsLastSibling();
        Animator animator = SceneLoadObject.GetComponent<Animator>();
        animator.Play("GameLoadAnim");
        PlayerPrefs.SetInt("StartAnim", 1);
    }


}

