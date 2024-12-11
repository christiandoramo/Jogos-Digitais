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

    [Tooltip("`Probabilidade de uma wave instanciar no modo nightmare")]
    [SerializeField] int nightmareModeProb;
    [Tooltip("Multiplicador de inimigos")]
    [SerializeField] int enemiesMultiplier = 2;


    [Tooltip("Lista de portais por onde inimigos instanciam")]
    public List<GameObject> portals = new();
    [Tooltip("Objeto vazio com inimigos")]

    [SerializeField] GameObject zombiePrefab;
    [SerializeField] GameObject portalPrefab;

    public int currentEnemies;
    public float spawnEnemyInterval = 4f;
    [Tooltip("Total de waves")]
    public int waveAmount;
    public int currentWave = 0;
    public int currentEnemiesWaveAmount;
    private int previousEnemiesWaveAmount = 1;

    private GameObject portalsEmpty;
    private GameObject enemiesEmpty;

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
        if (portalsEmpty == null)
        {
            PortalSpawn();
        }
    }

    private void PortalSpawn()
    {
        GameObject map = GameObject.Find("Environment/Map");
        if (map != null)
        {
            portalsEmpty = new GameObject("Portals");
            portalsEmpty.transform.parent = map.transform;

            // limitar de  X=-6 Y=1.5Y ate X=38 Y=1.5Y
            Vector3 randomPosition = new Vector3((float)Random.Range(6, 38), 1.5f, 0f);
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
        currentEnemiesWaveAmount = previousEnemiesWaveAmount * enemiesMultiplier;
        previousEnemiesWaveAmount = currentEnemiesWaveAmount;

        if (Random.Range(0, 2) > 0) currentEnemiesWaveAmount *= enemiesMultiplier;
        currentEnemies = currentEnemiesWaveAmount;

        if (Random.Range(0, 2) > 0) PortalSpawn();

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
