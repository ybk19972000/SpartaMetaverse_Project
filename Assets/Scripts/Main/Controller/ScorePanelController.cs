using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePanelController : MonoBehaviour
{
    [SerializeField] private GameObject mainScoreUI;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (mainScoreUI != null)
            {
                mainScoreUI.SetActive(true);
                MainScore scoreUI = mainScoreUI.GetComponent<MainScore>();

                if (scoreUI != null)
                {
                    scoreUI.SetScore(
                        GameManager.Instance.LastScore,
                        GameManager.Instance.BestScore,
                        GameManager.Instance.BestCombo
                    );
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (mainScoreUI != null)
            {
                mainScoreUI.SetActive(false);
            }
        }
    }
}
