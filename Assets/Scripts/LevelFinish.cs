using System;
using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision");
        if (other.gameObject.tag == "Player")
        {
            GameManager.Instance.NextLevel();
        }
    }
}
