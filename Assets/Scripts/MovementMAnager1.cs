using UnityEngine;

/// <summary>
/// Mueve este objeto en dirección opuesta a la parte frontal de su objeto padre.
/// </summary>
public class MovementMAnager1 : MonoBehaviour
{
    /// <summary>Distancia que avanza el objeto por segundo.</summary>
    public float velocidad = 0.5f;

    /// <summary>
    /// Actualiza la posición en cada fotograma para que el movimiento sea continuo.
    /// </summary>
    private void Update()
    {
        transform.position += -transform.forward * velocidad * Time.deltaTime;
    }
}
