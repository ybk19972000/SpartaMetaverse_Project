using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManger : MonoBehaviour
{
    public static GameManger Instace { get; private set; }

    [SerializeField] private CameraFollow cameraFollow;

    private void Awake()
    {
        if(Instace == null)
        {
            Instace = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);   
        }

        Init();
    }

    private void Init()
    {

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
