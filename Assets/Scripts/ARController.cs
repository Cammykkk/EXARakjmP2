using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

public class ARController : MonoBehaviour
{
    [Header("AR Settings")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARSession arSession;
    [SerializeField] private Camera arCamera;
    [SerializeField] private MovementManager movementManager;

    [Header("Controllers")]
    [SerializeField] private CanvasManager canvasManager; // <-- Referencia añadida

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabIndicador;
    [SerializeField] private GameObject prefabDinosaurio;
    [SerializeField] private GameObject prefabSpawns;

    [Header("UI")]
    [SerializeField] private Button buttonColocar;

    private GameObject indicador;
    private GameObject dinosaurio;
    private GameObject puntosInstanciados;

    private bool planoDetectado = false;
    private bool objetoColocado = false;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Start()
    {
        if (prefabIndicador != null)
        {
            indicador = Instantiate(prefabIndicador);
            indicador.SetActive(false);
        }

        if (planeManager != null)
        {
            planeManager.planesChanged += OnPlanesChanged;
        }

        if (buttonColocar != null)
        {
            buttonColocar.interactable = false;
        }
    }

    private void OnDestroy()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged -= OnPlanesChanged;
        }
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added.Count > 0 && !planoDetectado)
        {
            planoDetectado = true;
        }
    }

    private void Update()
    {
        if (objetoColocado || !planoDetectado) return;

        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        
        if (raycastManager != null && raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            if (indicador != null)
            {
                indicador.SetActive(true);
                indicador.transform.position = hitPose.position;
                indicador.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }

            if (buttonColocar != null)
            {
                buttonColocar.interactable = true;
            }
        }
        else
        {
            if (indicador != null) indicador.SetActive(false);
            if (buttonColocar != null) buttonColocar.interactable = false;
        }
    }

    /// <summary>
    /// Coloca el modelo del dinosaurio y los spawns en la escena.
    /// </summary>
    public void ColocarModelo()
    {
        if (indicador == null || dinosaurio != null || prefabDinosaurio == null ||
            prefabSpawns == null || arCamera == null || movementManager == null)
        {
            Debug.LogError("ARController no está configurado correctamente para colocar el modelo.");
            return;
        }

        if (!indicador.activeSelf) return;

        Vector3 posicionSpawn = indicador.transform.position;
        Vector3 rotacionCamera = arCamera.transform.eulerAngles;
        Quaternion rotacionSpawn = Quaternion.Euler(0f, rotacionCamera.y, 0f);

        // Instanciar dinosaurio
        dinosaurio = Instantiate(prefabDinosaurio, posicionSpawn, rotacionSpawn);
        Collider[] collidersDinosaurio = dinosaurio.GetComponentsInChildren<Collider>();
        
        if (collidersDinosaurio.Length == 0)
        {
            Debug.LogError("El prefab del dinosaurio no contiene colliders.");
        }

        foreach (Collider col in collidersDinosaurio)
        {
            col.gameObject.tag = "Player";
        }

        // Instanciar spawns
        puntosInstanciados = Instantiate(prefabSpawns, posicionSpawn, rotacionSpawn);

        indicador.SetActive(false);
        objetoColocado = true;
        movementManager.setDinosaurio(dinosaurio.transform);
        DesactivarPlanos();

        // Si necesitas avisar al CanvasManager o hacer algo al colocar el modelo:
        if (canvasManager != null)
        {
            // Ejemplo: canvasManager.MetodoDespuesDeColocar();
        }
    }

    /// <summary>
    /// Reinicia la sesión de AR y limpia los objetos creados.
    /// </summary>
    public void ReiniciarSesion()
    {
        if (planeManager == null || arSession == null)
        {
            Debug.LogError("ARController requiere un ARPlaneManager y un ARSession para reiniciar.");
            return;
        }

        planoDetectado = false;
        objetoColocado = false;

        if (dinosaurio != null) Destroy(dinosaurio);
        if (puntosInstanciados != null) Destroy(puntosInstanciados);
        dinosaurio = null;
        puntosInstanciados = null;

        if (indicador != null) indicador.SetActive(false);
        if (buttonColocar != null) buttonColocar.interactable = false;

        planeManager.enabled = true;
        foreach (ARPlane plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(true);
        }

        arSession.Reset();
    }

    private void DesactivarPlanos()
    {
        if (planeManager == null) return;

        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        planeManager.enabled = false;
    }
}