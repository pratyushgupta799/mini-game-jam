using System;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform player;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update() {
        transform.position = player.transform.position;
    }
}
