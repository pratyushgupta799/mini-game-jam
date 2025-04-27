using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RawImage red;
    [SerializeField] private RawImage green;
    [SerializeField] private RawImage blue;
    [SerializeField] private RawImage white;
    
    [SerializeField] public Texture redOn;
    [SerializeField] public Texture greenOn;
    [SerializeField] public Texture blueOn;
    [SerializeField] public Texture whiteOn;
    
    [SerializeField] public Texture redOff;
    [SerializeField] public Texture greenOff;
    [SerializeField] public Texture blueOff;
    [SerializeField] public Texture whiteOff;

    [SerializeField] private TextMeshProUGUI levelText;

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
        char numberPart = SceneManager.GetActiveScene().name[5];
        levelText.text = "Level: " + numberPart;
    }

    void Update()
    {
        if (GameManager.curBlockColor == CONSTANTS.RED)
        {
            red.texture = redOn;
            green.texture = greenOff;
            blue.texture = blueOff;
            white.texture = whiteOff;
        }
        else if (GameManager.curBlockColor == CONSTANTS.GREEN)
        {
            red.texture = redOff;
            green.texture = greenOn;
            blue.texture = blueOff;
            white.texture = whiteOff;
        }
        else if (GameManager.curBlockColor == CONSTANTS.BLUE)
        {
            red.texture = redOff;
            green.texture = greenOff;
            blue.texture = blueOn;
            white.texture = whiteOff;
        }
        else if (GameManager.curBlockColor == CONSTANTS.WHITE)
        {
            red.texture = redOff;
            green.texture = greenOff;
            blue.texture = blueOff;
            white.texture = whiteOn;
        }
    }
}
