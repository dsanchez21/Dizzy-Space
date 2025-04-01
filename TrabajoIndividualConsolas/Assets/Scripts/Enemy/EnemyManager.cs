using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int m_health = 50; // Puntos de vida del enemigo
    public int m_damage = 10; // Daño que inflige a la torre
    public float m_speed = 2f; // Velocidad de movimiento del enemigo

    [SerializeField]
    private TowerManager m_tower; // Referencia a la torre central (asignada desde el Inspector)

    void Start()
    {
        // Verificar si la referencia a la torre no está asignada
        if (m_tower == null)
        {
            Debug.LogError("La referencia a TowerManager no está asignada en el Inspector.");
        }
    }

    void Update()
    {
        // Mover al enemigo hacia la torre
        if (m_tower != null)
        {
            MoveTowardsTower();
        }
    }

    void MoveTowardsTower()
    {
        // Mover al enemigo hacia la posición de la torre
        Vector3 direction = (m_tower.transform.position - transform.position).normalized;
        transform.position += direction * m_speed * Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        // Reducir los puntos de vida del enemigo
        m_health -= damage;

        // Verificar si el enemigo ha muerto
        if (m_health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Destruir al enemigo
        Destroy(gameObject);
        Debug.Log("Enemigo destruido.");
    }

    void OnTriggerEnter(Collider other)
    {
        // Verificar si el enemigo ha alcanzado la torre
        if (other.CompareTag("Tower"))
        {
            // Infligir daño a la torre
            if (m_tower != null)
            {
                m_tower.TakeDamage(m_damage);
            }

            // Destruir al enemigo después de atacar
            Destroy(gameObject);
        }
    }
}
