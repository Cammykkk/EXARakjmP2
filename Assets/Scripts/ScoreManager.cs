using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Configuración de vidas")]
    [SerializeField, Min(1)] private int vidasIniciales = 3;

    [Header("UI de Puntuación y Vidas (Asignar en el Inspector)")]
    [SerializeField] private TextMeshProUGUI textoPuntuacion;
    [SerializeField] private TextMeshProUGUI textoVidas;
    [SerializeField] private TextMeshProUGUI textoPuntuacionFinal;

    [Header("Controladores")]
    [SerializeField] private CanvasManager canvasManager;

    private int puntuacionTotal;
    private int vidasRestantes;
    private bool juegoFinalizado;

    public int PuntuacionTotal => puntuacionTotal;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        vidasIniciales = Mathf.Max(1, vidasIniciales);
        vidasRestantes = vidasIniciales;
    }

    private void Start()
    {
        // Validaciones en consola por si falta alguna asignación
        if (textoPuntuacion == null) Debug.LogWarning("Falta asignar el textoPuntuacion en el ScoreManager.");
        if (textoVidas == null) Debug.LogWarning("Falta asignar el textoVidas en el ScoreManager.");
        if (textoPuntuacionFinal == null) Debug.LogWarning("Falta asignar el textoPuntuacionFinal en el ScoreManager.");
        if (canvasManager == null) Debug.LogWarning("Falta asignar el canvasManager en el ScoreManager.");

        ActualizarTextos();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SumarPuntos(int puntos)
    {
        if (juegoFinalizado) return;

        puntuacionTotal += puntos;
        ActualizarTextos();
    }

    public void Reiniciar()
    {
        puntuacionTotal = 0;
        vidasRestantes = vidasIniciales;
        juegoFinalizado = false;
        ActualizarTextos();
    }

    public void PerderVidas(int cantidad)
    {
        if (juegoFinalizado || cantidad <= 0) return;

        vidasRestantes = Mathf.Max(0, vidasRestantes - cantidad);
        ActualizarTextos();

        if (vidasRestantes == 0)
        {
            juegoFinalizado = true;

            if (canvasManager != null)
            {
                canvasManager.FinalizarJuego();
            }
            else
            {
                Debug.LogError("ScoreManager no tiene asignado el CanvasManager para mostrar Game Over.");
            }
        }
    }

    private void ActualizarTextos()
    {
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = puntuacionTotal.ToString();
        }

        if (textoVidas != null)
        {
            textoVidas.text = vidasRestantes.ToString();
        }

        if (textoPuntuacionFinal != null)
        {
            textoPuntuacionFinal.text = puntuacionTotal.ToString();
        }
    }
}