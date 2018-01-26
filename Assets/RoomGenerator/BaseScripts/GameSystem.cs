using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSystem : BaseClass
{
    protected GameManager m_gameManager;
    public virtual void Initialize(GameManager a_gameManager)
    {
        m_gameManager = a_gameManager;
    }
    public virtual void Update() { }
    public abstract void Clear();
}
