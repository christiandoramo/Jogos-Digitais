using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

[CreateAssetMenu(fileName = "WaveManager", menuName = "ScriptableObjects/WaveManager", order = 1)]
public class WaveManager : ScriptableObject
{
    [Header("Regras do jogo")]
    [Tooltip("`Probabilidade de uma wave instanciar no modo nightmare")]
    [SerializeField] int nightmareModeProb;
    [Tooltip("Multiplicador de inimigos")]
    [SerializeField] float enemiesMultiplier = 1.5f;


    [Tooltip("Lista de portais por onde inimigos instanciam")]
    public List<GameObject> portals = new();
    [Tooltip("Objeto vazio com inimigos")]

    [SerializeField] GameObject zombiePrefab;
    [SerializeField] GameObject portalPrefab;

    public int currentEnemies;
    public float spawnEnemyInterval = 2f;
    [Tooltip("Total de waves")]
    public int waveAmount;
    public int currentWave = 0;
    public int currentEnemiesWaveAmount;
    public int previousEnemiesWaveAmount = 1;

    private GameObject portalsEmpty;
    private GameObject enemiesEmpty;

    public Transform playerTransform;

    public void Initialize()
    {
        if (enemiesEmpty == null)
        {
            GameObject env = GameObject.Find("Environment");
            if (env != null)
            {
                // cria "Enemies" como filho de "Map"
                enemiesEmpty = new GameObject("Enemies");
                enemiesEmpty.transform.parent = env.transform;
            }
            else
            {
                Debug.Log("GameObject.Find(\"Environment\") falhou");
            }
        }
    }

    private void PortalSpawn()
    {
        if (playerTransform == null) return;
        GameObject map = GameObject.Find("Environment/Map");
        if (map != null)
        {
            portalsEmpty = new GameObject("Portals");
            portalsEmpty.transform.parent = map.transform;

            // alturas possíveis: -1.2, 2, 6.5, 10.5, 26.5, 30, 35, 38, 44, 54, 79, 83, 97, 101, 106.5, 111
            float[] possibleHeights = { -1.2f, 2f, 6.5f, 10.5f, 26.5f, 30f, 35f, 38f, 44f, 54f, 79f, 83f, 97f, 101f, 106.5f, 111f };
            float upHeight = possibleHeights.Where(a => a > playerTransform.position.y).OrderBy(a => a).FirstOrDefault();
            float downHeight = possibleHeights.Where(a => a <= playerTransform.position.y).OrderByDescending(a => a).FirstOrDefault();
            float chosenHeight = UnityEngine.Random.Range(0, 2) == 0 ? upHeight : downHeight;


            Vector3 randomPosition = new Vector3((float)Random.Range(-8, 2), chosenHeight, 0f);
            GameObject newPortal = Instantiate(portalPrefab, randomPosition, Quaternion.identity);
            Debug.Log("Instanciou em: " + newPortal.transform.position);


            newPortal.transform.parent = portalsEmpty.transform;

            portals.Add(newPortal);
        }
        else
        {
            Debug.Log("GameObject.Find(\"Environment/Map\") falhou");
        }
    }

    public void GenerateNewWave()
    {
        currentWave++;
        if (currentWave > waveAmount) return;

        PortalSpawn();

        currentEnemiesWaveAmount = (int)(previousEnemiesWaveAmount * enemiesMultiplier) +1;
        previousEnemiesWaveAmount = currentEnemiesWaveAmount;

        // 25% de chance de vir uma wave dobrada
        if (Random.Range(0, 4) < 1) currentEnemiesWaveAmount = (int)(currentEnemiesWaveAmount * enemiesMultiplier);
        currentEnemies = currentEnemiesWaveAmount;

        MonoBehaviour behaviour = FindAnyObjectByType<MonoBehaviour>();
        if (behaviour != null)
        {
            behaviour.StartCoroutine(SpawnEnemies(currentEnemiesWaveAmount));
        }
    }

    private IEnumerator SpawnEnemies(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Transform portalChosen = portals[Random.Range(0, portals.Count())].transform;
            GameObject enemy = Instantiate(zombiePrefab, portalChosen.position, Quaternion.identity);
            if (enemiesEmpty != null)
            {
                enemy.transform.parent = enemiesEmpty.transform;
            }
            yield return new WaitForSeconds(spawnEnemyInterval); // aguardando X segundos para continuar o resto do loop
        }
    }
}
