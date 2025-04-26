using System.ComponentModel.Design.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Levels")] 
    [SerializeField] private SceneAsset[] levels;
    private int curSceneInd = 0;
    public static string curBlockColor;
    
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject walls;
    [SerializeField] private Material disabled;

    private void Awake()
    {
        curBlockColor = CONSTANTS.BLUE;
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
    }
    
    public void NextLevel()
    {
        if (curSceneInd < levels.Length - 1)
        {
            Debug.Log($"Loading next level: {levels[curSceneInd + 1].name} (Index: {curSceneInd + 1})");
            string sceneName = levels[curSceneInd + 1].name;
            SceneManager.LoadScene(sceneName);
            curSceneInd++;
        }
        else
        {
            Debug.Log("Game Finished!");
        }
    }
    
    public void ReloadCurrentLevel()
    {
        if (curSceneInd >= 0 && curSceneInd < levels.Length && levels[curSceneInd] != null)
        {
            string sceneName = levels[curSceneInd].name;
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
        for (int i = 0; i < walls.transform.childCount; i++)
        {
            walls.transform.GetChild(i).GetComponent<BlockBehaviour>().SetActiveBlock();
        }
    }
}
