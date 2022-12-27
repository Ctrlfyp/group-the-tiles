using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialView : MonoBehaviour
{
    public Text pageCount;
    public Button skipButton;
    public Button prevButton;
    public Button nextButton;
    public Canvas tutorialCanvas;

    public int currentPage;
    public SpriteRenderer currentSpriteRenderer;
    public List<Texture2D> pages;

    void Start()
    {
        SetPage(0);
    }

    private void SetPage(int page)
    {
        if (page >= pages.Count)
        {
            tutorialCanvas.gameObject.SetActive(false);
            return;
        }

        if (page < 0)
        {
            return;
        }

        currentPage = page;
        currentSpriteRenderer.sprite = Sprite.Create(pages[currentPage], new Rect(0, 0, pages[currentPage].width, pages[currentPage].height), new Vector2(0.5f, 0.5f));
        pageCount.text = $"{currentPage + 1} / {pages.Count}";
    }

    public void ActivateTutorial()
    {
        tutorialCanvas.gameObject.SetActive(true);
    }

    public void OnSkipButton()
    {
        tutorialCanvas.gameObject.SetActive(false);
    }

    public void OnNextButton()
    {
        SetPage(currentPage + 1);
    }

    public void OnPrevButton()
    {
        SetPage(currentPage - 1);
    }
}
