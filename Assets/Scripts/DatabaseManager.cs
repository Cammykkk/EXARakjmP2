using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Networking;

/// <summary>
/// Permite enviar el usuario y su puntuación a la tabla PuntuacionesExamen en Supabase.
/// </summary>
public class DataBaseManager : MonoBehaviour
{
    [Header("UI Elements")]
    /// <summary>Cuadro de texto para escribir el nombre del usuario.</summary>
    public TMP_InputField ifUsuario;
    
    /// <summary>Texto de TextMeshProUGUI que muestra la puntuación actual.</summary>
    public TextMeshProUGUI puntuacionText;

    [Header("Controllers")]
    /// <summary>Componente que administra la interfaz en pantalla.</summary>
    [SerializeField] private CanvasManager canvasManager;

    [Header("Datos de conexion")]
    /// <summary>Dirección que usa el juego para comunicarse con la base de datos.</summary>
    [SerializeField] private string url = "https://kdbonvitnajbddernrur.supabase.co/rest/v1/";
    /// <summary>Clave pública de Supabase.</summary>
    private const string PublishableKey = "sb_publishable__gKfVmX944OZJQVGT-xyAQ_eXBR76JK";
   

    /// <summary>
    /// Toma el usuario del InputField y la puntuación del TextMeshPro, y los envía a la base de datos.
    /// </summary>
    public void SaveScore()
    {
        if (ifUsuario == null || puntuacionText == null)
        {
            Debug.LogError("Falta asignar el InputField de usuario o el TextMeshPro de puntuación en el Inspector.");
            return;
        }

        // Convertir el texto de la puntuación a un número entero
        if (int.TryParse(puntuacionText.text, out int scoreValue))
        {
            PuntuacionesExamen dataToSend = new PuntuacionesExamen
            {
                Usuario = ifUsuario.text,
                Puntuacion = scoreValue
            };

            StartCoroutine(PostScoreRequest(dataToSend));
        }
        else
        {
            Debug.LogError("No se pudo convertir el texto de la puntuación a un número válido: " + puntuacionText.text);
        }
    }

    private IEnumerator PostScoreRequest(PuntuacionesExamen data)
    {
        // Apunta directamente a la tabla PuntuacionesExamen
        string endpoint = url + "PuntuacionesExamen"; 
        string json = JsonUtility.ToJson(data);

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        UnityWebRequest www = new UnityWebRequest(endpoint, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("apikey", PublishableKey);
        www.SetRequestHeader("Authorization", "Bearer " + PublishableKey);
        www.SetRequestHeader("Prefer", "return=representation");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("¡Puntuación guardada con éxito en PuntuacionesExamen!: " + www.downloadHandler.text);

            // Notificar al CanvasManager si está asignado
            if (canvasManager != null)
            {
                // canvasManager.MetodoExito();
            }
        }
        else
        {
            Debug.LogError($"Error al guardar en la base de datos [{www.responseCode}]: {www.error} \n {www.downloadHandler.text}");

            // Notificar al CanvasManager si hay error
            if (canvasManager != null)
            {
                // canvasManager.MetodoError();
            }
        }
    }
}

/// <summary>Estructura que mapea los campos exactos de la tabla PuntuacionesExamen.</summary>
[System.Serializable]
public class PuntuacionesExamen
{
    public string Usuario;
    public int Puntuacion;
}