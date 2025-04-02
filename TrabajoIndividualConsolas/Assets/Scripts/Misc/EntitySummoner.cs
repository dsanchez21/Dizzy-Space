using System.Collections.Generic;
using UnityEngine;

public class EntitySummoner : MonoBehaviour
{
    public static List<Enemy> EnemiesInGame;
    public static Dictionary<int, GameObject> EnemyPrefabs;
    public static Dictionary<int, Queue<Enemy>> EnemyObjectPools;

    private static bool IsInitialized;

    public static void Init()
    {
        if (!IsInitialized) // Cambiar la condición para inicializar solo si no está inicializado
        {
            EnemyPrefabs = new Dictionary<int, GameObject>();
            EnemyObjectPools = new Dictionary<int, Queue<Enemy>>();
            EnemiesInGame = new List<Enemy>();

            EnemySummonData[] Enemies = Resources.LoadAll<EnemySummonData>("Enemies");

            if (Enemies.Length == 0)
            {
                Debug.LogError("No EnemySummonData found in Resources/Enemies. Initialization failed.");
                return;
            }

            foreach (EnemySummonData enemy in Enemies)
            {
                if (!EnemyPrefabs.ContainsKey(enemy.m_EnemyID))
                {
                    EnemyPrefabs.Add(enemy.m_EnemyID, enemy.m_EnemyPrefab);
                    EnemyObjectPools.Add(enemy.m_EnemyID, new Queue<Enemy>());
                }
                else
                {
                    Debug.LogWarning($"Duplicate Enemy ID detected: {enemy.m_EnemyID}. Skipping.");
                }
            }

            IsInitialized = true;
            Debug.Log("EntitySummoner initialized successfully.");
        }
        else
        {
            Debug.Log("EntitySummoner is already initialized.");
        }
    }

    public static Enemy SummonEnemy(int m_EnemyID)
    {
        if (!IsInitialized)
        {
            Debug.LogError("EntitySummoner has not been initialized. Call Init() before using SummonEnemy.");
            return null;
        }

        Enemy SummonedEnemy = null;

        if (EnemyPrefabs.ContainsKey(m_EnemyID))
        {
            Queue<Enemy> ReferencedQueue = EnemyObjectPools[m_EnemyID];

            if (ReferencedQueue.Count > 0)
            {
                SummonedEnemy = ReferencedQueue.Dequeue();
                SummonedEnemy.Init();

                SummonedEnemy.gameObject.SetActive(true);
            }
            else
            {
                GameObject NewEnemy = Instantiate(EnemyPrefabs[m_EnemyID], Vector3.zero, Quaternion.identity);
                SummonedEnemy = NewEnemy.GetComponent<Enemy>();
            }

            EnemiesInGame.Add(SummonedEnemy);
        }
        else
        {
            Debug.LogError($"No enemy with ID: {m_EnemyID}");
            return null;
        }

        EnemiesInGame.Add(SummonedEnemy);
        SummonedEnemy.ID = m_EnemyID;
        return SummonedEnemy;
    }

    public static void RemoveEnemy(Enemy EnemyToRemove)
    {
        EnemyObjectPools[(int)EnemyToRemove.ID].Enqueue(EnemyToRemove);
        EnemyToRemove.gameObject.SetActive(false);
        EnemiesInGame.Remove(EnemyToRemove);
    }
}
