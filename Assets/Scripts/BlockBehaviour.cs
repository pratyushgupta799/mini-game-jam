using System;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    private string materialName;
    private Material material;

    [SerializeField] private Material disabledMat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        materialName = GetMaterialName();
        material = gameObject.GetComponent<Renderer>().material;
        //Debug.Log(material);
        SetActiveBlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveBlock()
    {
        Debug.Log(materialName + ", " + GameManager.curBlockColor);
        if(GameManager.curBlockColor != materialName)
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<Renderer>().material = disabledMat;
            //gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("yo?");
            gameObject.GetComponent<BoxCollider>().enabled = true;
            gameObject.GetComponent<Renderer>().material = material;
            //gameObject.SetActive(true);
        }
    }

    string GetMaterialName()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = renderer.material;
            if (material != null)
            {
                return material.name;
            }
        }

        Debug.Log("No Renderer found on the GameObject.");

        return null;
    }

    private void OnCollisionExit(Collision other)
    {
        Debug.Log("Collision exit");
        if (GameManager.curBlockColor == CONSTANTS.RED)
        {
            GameManager.curBlockColor = CONSTANTS.BLUE;
        }
        else if (GameManager.curBlockColor == CONSTANTS.BLUE)
        {
            GameManager.curBlockColor = CONSTANTS.GREEN;
        }
        else if (GameManager.curBlockColor == CONSTANTS.GREEN)
        {
            GameManager.curBlockColor = CONSTANTS.WHITE;
        }
        else if (GameManager.curBlockColor == CONSTANTS.WHITE)
        {
            GameManager.curBlockColor = CONSTANTS.RED;
        }
        GameManager.Instance.SetActiveWalls();
        Debug.Log(GameManager.curBlockColor);
    }
}
