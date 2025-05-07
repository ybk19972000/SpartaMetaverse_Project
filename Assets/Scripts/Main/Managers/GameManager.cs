using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private CameraFollow cameraFollow;

    public int LastScore { get; set; }
    public int BestScore { get; set; }
    public int BestCombo { get; set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //씬 변경시에도 존재
        }
        else
        {
            Destroy(gameObject);   
        }

    }

    public void SetStackGameResult(int score, int bestScore, int bestCombo)
    {
        LastScore = score;
        BestScore = bestScore;
        BestCombo = bestCombo;
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (scene.name == "TheStack")
        {
            Screen.SetResolution(1080, 1920, FullScreenMode.Windowed); 
        }
        else
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed); 
        }

        if (scene.name == "MainScene")
        {
            CameraFollow cameraFollow = FindAnyObjectByType<CameraFollow>();
            PlayerController player = FindAnyObjectByType<PlayerController>();
            Tilemap tilemap = FindAnyObjectByType<Tilemap>();

            if(cameraFollow != null && player != null && tilemap != null) 
            {
                cameraFollow.SetTargetAndTileMap(player.transform, tilemap);
            }

        }

    }
}
