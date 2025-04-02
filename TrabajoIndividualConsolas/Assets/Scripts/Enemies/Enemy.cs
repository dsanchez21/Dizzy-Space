using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    public float m_MaxHealth;
    public float m_CurrentHealth;
    public float m_Speed;
    public float ID;

    public void Init()
    {
        m_CurrentHealth = m_MaxHealth;
    }
}
