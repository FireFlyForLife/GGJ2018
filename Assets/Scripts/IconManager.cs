using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconManager : MonoBehaviour
{
    [SerializeField] private List<Icon> m_icons;
    private int m_storedID = -1;

    public AudioClip clickSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnClick(int id)
    {
        if (m_storedID >= 0 && m_storedID != id)
            m_icons[m_storedID].OnClick(false);
        m_icons[id].OnClick(true);
        m_storedID = id;

        audioSource.PlayOneShot(clickSound);
    }
}
