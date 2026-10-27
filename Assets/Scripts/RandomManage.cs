using UnityEngine;
using System.Collections;

/// <summary>
/// Crea obstáculos en spawners elegidos al azar, esperando entre cada creación.
/// </summary>
public class RandomManage : MonoBehaviour
{
    /// <summary>Lista de spawners que pueden crear obstáculos.</summary>
    public ARSpawner1[] obstacleParent;

    /// <summary>Tiempo de espera, en segundos, entre la creación de obstáculos.</summary>
    [Min(0.1f)] public float tiempoSpawn = 1.5f;

    private void Start()
    {
        if (obstacleParent == null || obstacleParent.Length == 0)
        {
            Debug.LogError($"No hay spawners asignados en {name}.", this);
            return;
        }

        StartCoroutine(RandomSpawn());
    }

    /// <summary>
    /// Espera el tiempo indicado y pide a un spawner elegido al azar que cree un obstáculo.
    /// </summary>
    /// <returns>Una rutina que permite esperar sin detener el juego.</returns>
    private IEnumerator RandomSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoSpawn);
            int randomIndex = Random.Range(0, obstacleParent.Length);
            ARSpawner1 spawner = obstacleParent[randomIndex];
            if (spawner == null)
            {
                Debug.LogError($"El spawner en el índice {randomIndex} no está asignado en {name}.", this);
                continue;
            }

            spawner.instantiateObstacle();
        }
    }
}
