using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [Header("Pantallas (GameObjects)")]
    [SerializeField] private GameObject canvasBienvenida;
    [SerializeField] private GameObject canvasJuego;
    [SerializeField] private GameObject canvasGameOver;

    [Header("Controladores")]
    [SerializeField] private ARController arController;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private DataBaseManager dataBaseManager;

    [Header("UI - Game Over (CanvasManager)")]
    [SerializeField] private TMP_InputField inputNombre;
    [SerializeField] private Button buttonRegistrar;
    [SerializeField] private TMP_Text textoButtonRegistrar;

    private bool enviandoPuntuacion;
    private bool puntuacionRegistrada;

    private void Start()
    {
        // Validaciones básicas de referencias
        if (arController == null) Debug.LogWarning("Falta asignar ARController en el CanvasManager.");
        if (scoreManager == null) Debug.LogWarning("Falta asignar ScoreManager en el CanvasManager.");
        if (dataBaseManager == null) Debug.LogWarning("Falta asignar DataBaseManager en el CanvasManager.");

        // Mostrar pantalla de bienvenida al iniciar
        MostrarCanvas(canvasBienvenida);
    }

    /// <summary>
    /// Cambia a la pantalla de juego. (Asignar en el OnClick del botón Start).
    /// </summary>
    public void IniciarJuego()
    {
        MostrarCanvas(canvasJuego);
    }

    /// <summary>
    /// Cambia a la pantalla de Game Over. (Llamado normalmente por el ScoreManager).
    /// </summary>
    public void FinalizarJuego()
    {
        MostrarCanvas(canvasGameOver);
    }

    /// <summary>
    /// Reinicia el juego por completo y vuelve al menú. (Asignar en el OnClick del botón Reiniciar / Volver).
    /// </summary>
    public void ReiniciarJuego()
    {
        if (arController != null) arController.ReiniciarSesion();
        if (scoreManager != null) scoreManager.Reiniciar();

        if (inputNombre != null)
        {
            inputNombre.text = string.Empty;
            inputNombre.DeactivateInputField();
        }

        enviandoPuntuacion = false;
        puntuacionRegistrada = false;

        if (buttonRegistrar != null)
        {
            buttonRegistrar.interactable = true;
        }

        if (textoButtonRegistrar != null)
        {
            textoButtonRegistrar.text = "Registrar";
        }

        MostrarCanvas(canvasBienvenida);
    }

    private void MostrarCanvas(GameObject canvasActivo)
    {
        if (canvasBienvenida != null) canvasBienvenida.SetActive(canvasActivo == canvasBienvenida);
        if (canvasJuego != null) canvasJuego.SetActive(canvasActivo == canvasJuego);
        if (canvasGameOver != null) canvasGameOver.SetActive(canvasActivo == canvasGameOver);
    }

    /// <summary>
    /// Envía la puntuación usando el DataBaseManager. (Asignar en el OnClick del botón Registrar).
    /// </summary>
    public void RegistrarPuntuacion()
    {
        if (enviandoPuntuacion || puntuacionRegistrada)
        {
            return;
        }

        if (inputNombre == null || scoreManager == null || dataBaseManager == null)
        {
            Debug.LogError("Faltan referencias en el CanvasManager para registrar la puntuación.");
            return;
        }

        string nombre = inputNombre.text.Trim();
        if (string.IsNullOrEmpty(nombre))
        {
            Debug.LogError("Escribe tu nombre antes de registrar la puntuación.");
            return;
        }

        enviandoPuntuacion = true;
        if (buttonRegistrar != null) buttonRegistrar.interactable = false;
        if (textoButtonRegistrar != null) textoButtonRegistrar.text = "Enviando...";

        StartCoroutine(EnviarDatosCoroutine(nombre));
    }

    private System.Collections.IEnumerator EnviarDatosCoroutine(string nombre)
    {
        // El DatabaseManager lee directamente de su propio InputField o podemos pasárselo. 
        // Aseguramos que el input de DatabaseManager coincida o se llene aquí si es necesario:
        if (dataBaseManager.ifUsuario != null)
        {
            dataBaseManager.ifUsuario.text = nombre;
        }

        // Llamamos al método de guardado en Supabase
        dataBaseManager.SaveScore();

        yield return new WaitForSeconds(1f); // Pequeña espera de feedback visual

        enviandoPuntuacion = false;
        puntuacionRegistrada = true;
        if (textoButtonRegistrar != null) textoButtonRegistrar.text = "Registrado";
    }
}