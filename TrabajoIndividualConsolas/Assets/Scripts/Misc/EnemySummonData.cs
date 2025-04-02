using UnityEngine;

[CreateAssetMenu(fileName = "New EnemySummonData", menuName = "Create EnemySummonData")]
public class EnemySummonData : ScriptableObject
{
    public GameObject m_EnemyPrefab;
    public int m_EnemyID;
}
