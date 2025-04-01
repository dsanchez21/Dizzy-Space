using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [Header("Tower Stats")]
    public int m_health = 100; // Puntos de vida de la torre
    public int m_attackDamage = 10; // Daño que inflige la torre
    public float m_attackRange = 5f; // Rango de detección de enemigos
    public float m_attackCooldown = 1f; // Tiempo entre ataques

    private float m_attackTimer = 0f; // Temporizador para controlar el ataque

    [Header("Game Over")]
    public GameObject m_gameOverUI; // UI que se muestra al perder el juego

    void Update()
    {
        // Verificar si hay enemigos en el rango de ataque
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, m_attackRange, LayerMask.GetMask("Enemy"));

        if (enemiesInRange.Length > 0)
        {
            m_attackTimer += Time.deltaTime;

            if (m_attackTimer >= m_attackCooldown)
            {
                AttackEnemy(enemiesInRange[0].gameObject); // Atacar al primer enemigo detectado
                m_attackTimer = 0f; // Reiniciar el temporizador
            }
        }

        // Verificar si la torre ha perdido todos los puntos de vida
        if (m_health <= 0)
        {
            LoseGame();
        }
    }

    void AttackEnemy(GameObject enemy)
    {
        // Reducir la vida del enemigo
        EnemyManager enemyScript = enemy.GetComponent<EnemyManager>();
        if (enemyScript != null)
        {
            enemyScript.TakeDamage(m_attackDamage);

            // Dibujar un rayo desde la torre hasta el enemigo
            Debug.DrawLine(transform.position, enemy.transform.position, Color.red, 0.5f);
        }
    }

    public void TakeDamage(int damage)
    {
        // Reducir los puntos de vida de la torre
        m_health -= damage;

        // Mostrar en consola los puntos de vida restantes
        Debug.Log($"Torre dañada. Vida restante: {m_health}");
    }

    void LoseGame()
    {
        // Mostrar la UI de Game Over
        if (m_gameOverUI != null)
        {
            m_gameOverUI.SetActive(true);
        }

        // Detener el juego (opcional)
        Time.timeScale = 0f;

        Debug.Log("¡Has perdido el juego!");
    }

    void OnDrawGizmosSelected()
    {
        // Dibujar el rango de ataque en la vista de escena
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_attackRange);
    }
}
