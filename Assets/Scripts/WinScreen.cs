using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private GameObject[] windows;
    [SerializeField] private float duration = 3;

    void OnEnable()
    {
    }

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            for (int i = 0; i < windows.Length; i++)
                windows[i].SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
