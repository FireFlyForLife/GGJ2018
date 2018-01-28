using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    [SerializeField] private Sprite m_default, m_selected;
    private Image m_image;
    private bool m_clicked;
    private float m_clickDelay;
    private FPSGameMode gm;

    // Use this for initialization
    void Start()
    {
        m_image = GetComponent<Image>();
        m_clicked = false;
        gm = GameObject.FindObjectOfType<FPSGameMode>();
    }

    public void OnClick(bool focus)
    {
        if (focus && m_clicked && m_clickDelay < .5f)
            gm.IsOpened = true;

            m_clicked = focus;
        if (m_clicked) m_image.sprite = m_selected;
        else m_image.sprite = m_default;

        if (m_clicked)
            m_clickDelay = 0;
    }

    public void Update()
    {
        if (m_clicked)
        {
            m_clickDelay += Time.deltaTime;
        }
    }
}
