using UnityEngine;


public class ObjectCollision : MonoBehaviour
{
    [Header("Configuración de Puntos")]
    [SerializeField] private int puntosQueDa = 10;
    [SerializeField] private int vidasQueQuita;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera") || other.CompareTag("Player"))
        {
            if (ScoreManager.Instance != null)
            {
                if (puntosQueDa > 0)
                {
                    ScoreManager.Instance.SumarPuntos(puntosQueDa);
                }

                if (vidasQueQuita > 0)
                {
                    ScoreManager.Instance.PerderVidas(vidasQueQuita);
                }
            }
            else
            {
                Debug.LogError("No se encontró el ScoreManager en la escena.");
            }

            Destroy(gameObject);
        }
    }
}