using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RSLoadingBar : MonoBehaviour
{
    [SerializeField]
    private Image m_fillBar;
    [SerializeField]
    private Text m_loadText;
    private float m_duration;
    private float m_time;
    void OnEnable()
    {
        m_duration = Random.Range(1.0f, 3.0f);
        m_time = m_duration;
    }

    void Update()
    {
        m_time -= Time.deltaTime;
        if (m_time < 0)
        {
            m_time = 0;
            gameObject.SetActive(false);
        }

        float v = 1 -  m_time / m_duration;
        m_fillBar.fillAmount = v;
        m_loadText.text = "Loading game.. " + (int) (v * 100) + " %";
    }
}
