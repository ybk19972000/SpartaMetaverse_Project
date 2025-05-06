using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : BaseUI
{
    Button startButton;
    Button exitButton;

    Image gameDesc;
    bool isGameDescVisible = false;

    protected override UIState GetUIState()
    {
        return UIState.Home;
    }

    public override void Init(UIManager uiManager)
    {
        base.Init(uiManager);

        startButton = transform.Find("StartButton").GetComponent<Button>();
        exitButton = transform.Find("ExitButton").GetComponent <Button>();
        gameDesc = transform.Find("GameDesc").GetComponent<Image>();

        startButton.onClick.AddListener(OnClickStartButton);
        exitButton.onClick.AddListener(OnClickExitButton);
    }
    private void Update()
    {
        if(gameDesc.gameObject.activeSelf && Input.GetMouseButtonDown(0))
        {
            gameDesc.gameObject.SetActive(false);
        }
    }
    void OnClickStartButton()
    {
        uiManager.OnClickStart();
    }

    void OnClickExitButton() 
    {
        uiManager.OnClickExit();
    }
}   
