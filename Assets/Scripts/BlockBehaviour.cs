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
            if(material.name == CONSTANTS.RED)
            {
                gameObject.GetComponent<Renderer>().material = GameManager.Instance.disabledRed;
            }
            else if(material.name == CONSTANTS.BLUE)
            {
                gameObject.GetComponent<Renderer>().material = GameManager.Instance.disabledBlue;
            }
            else if(material.name == CONSTANTS.GREEN)
            {
                gameObject.GetComponent<Renderer>().material = GameManager.Instance.disabledGreen;
            }
            else if(material.name == CONSTANTS.WHITE)
            {
                gameObject.GetComponent<Renderer>().material = GameManager.Instance.disabledWhite;
            }
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
}
