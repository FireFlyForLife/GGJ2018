using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private GameObject[] windows;

    void OnEnable()
    {
        
    }

    void OnClick()
    {
        for(int i = 0; i < windows.Length; i++)
            windows[i].SetActive(false);
    }
}
