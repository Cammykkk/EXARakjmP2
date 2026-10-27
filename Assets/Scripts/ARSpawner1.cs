using System.Collections;
using UnityEngine;

public class ARSpawner1 : MonoBehaviour
{
    [SerializeField] private GameObject prefabObstacle;
    private GameObject currentObstacle; // Nombre corregido para evitar confusión

    public void instantiateObstacle()
    {
        if (prefabObstacle == null)
        {
            Debug.LogError($"No hay un prefab de obstáculo asignado en {name}.", this);
            return;
            
        }

        else
        {
            Debug.Log($"Instanciando obstáculo en {name}.", this);
        }

        // Si ya hay un obstáculo activo y quieres reemplazarlo, puedes destruirlo primero:
        if (currentObstacle != null)
        {
            Destroy(currentObstacle);
        }

        // Instanciar usando la variable global
        currentObstacle = Instantiate(prefabObstacle, transform.position, transform.rotation);
        
        foreach (Collider obstacleCollider in currentObstacle.GetComponentsInChildren<Collider>())
        {
            Rigidbody body = obstacleCollider.GetComponentInParent<Rigidbody>();
            if (body == null)
            {
                body = obstacleCollider.gameObject.AddComponent<Rigidbody>();
            }

            body.useGravity = false;
            body.isKinematic = true;
        }

        StartCoroutine(DestroyObstacle(currentObstacle));
    }

    private IEnumerator DestroyObstacle(GameObject obstacleInstance)
    {
        yield return new WaitForSeconds(1.3f);
        if (obstacleInstance != null)
        {
            Destroy(obstacleInstance);
        }
    }
}