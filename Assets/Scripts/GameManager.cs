using System.ComponentModel.Design.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Levels")] 
    [SerializeField] private string[] levels;
    private int curSceneInd = 0;

    private enum StartColor
    {
        Red,
        Blue,
        Green,
        White
    }
    [SerializeField] private StartColor startColor;
    public static string curBlockColor;
    
    public static GameManager Instance { get; private set; }

    private GameObject walls;
    public Material disabledRed;
    public Material disabledBlue;
    public Material disabledGreen;
    public Material disabledWhite;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        walls = GameObject.Find("Walls");
    }
    
    private void Awake()
    {
        if (startColor == StartColor.Red)
        {
            curBlockColor = CONSTANTS.RED;
        }
        else if (startColor == StartColor.Blue)
        {
            curBlockColor = CONSTANTS.BLUE;
        }
        else if (startColor == StartColor.Green)
        {
            curBlockColor = CONSTANTS.GREEN;
        }
        else
        {
            curBlockColor = CONSTANTS.WHITE;
        }
        
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Duplicate GameManager detected. Destroying");
            Destroy(gameObject);
        }
        
        walls = GameObject.Find("Walls");
    }
    
    public void NextLevel()
    {
        if (curSceneInd < levels.Length - 1)
        {
            Debug.Log($"Loading next level: {levels[curSceneInd + 1]} (Index: {curSceneInd + 1})");
            string sceneName = levels[curSceneInd + 1];
            SceneManager.LoadScene(sceneName);
            curSceneInd++;
        }
        else
        {
            SceneManager.LoadScene("Credits");
        }
    }
    
    public void ReloadCurrentLevel()
    {
        if (curSceneInd >= 0 && curSceneInd < levels.Length && levels[curSceneInd] != null)
        {
            string sceneName = levels[curSceneInd];
            Debug.Log($"Reloading current level: {sceneName} (Index: {curSceneInd})");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Cannot reload level: Invalid current scene index or SceneAsset not assigned.");
        }
    }

    public void SetActiveWalls()
    {
        ChangeColor();
        for (int i = 0; i < walls.transform.childCount; i++)
        {
            walls.transform.GetChild(i).GetComponent<BlockBehaviour>().SetActiveBlock();
        }
    }

    private void ChangeColor()
    {
        if (curBlockColor == CONSTANTS.RED)
        {
            curBlockColor = CONSTANTS.GREEN;
        }
        else if (curBlockColor == CONSTANTS.GREEN)
        {
            curBlockColor = CONSTANTS.BLUE;
        }
        else if (curBlockColor == CONSTANTS.BLUE)
        {
            curBlockColor = CONSTANTS.WHITE;
        }
        else if (curBlockColor == CONSTANTS.WHITE)
        {
            curBlockColor = CONSTANTS.RED;
        }
    }
}
