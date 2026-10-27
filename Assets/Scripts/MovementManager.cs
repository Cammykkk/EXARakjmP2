using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MovementManager : MonoBehaviour
{
    [SerializeField] private Transform dinosaurio;
    [Header("Rango")]

    private Vector3 posicionInicial;
    private Vector3 ejeX;

    [Header("Limite de movimiento")]
      [SerializeField] private float limiteIzquierda = -0.5f;
      [SerializeField] private float limiteDerecha = 0.5f;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    public void setDinosaurio(Transform dinosaurioTransform)
    {
        dinosaurio = dinosaurioTransform;

        if (dinosaurio == null)
        {
            Debug.LogError("El objeto dinosaurio no está asignado en el inspector.");
            return;
        }

        posicionInicial = dinosaurio.position;
        ejeX = dinosaurio.right;
    }

    private void Update()
    {
        if (dinosaurio == null) return;

        var activeTouches = Touch.activeTouches;
        if (activeTouches.Count > 0)
        {
            var touch = activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began ||
                touch.phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary)
            {
                moverDinosaurio(touch.screenPosition);
            }

            return;
        }

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            moverDinosaurio(Mouse.current.position.ReadValue());
        }
    }

    private void moverDinosaurio(Vector2 touchPosition)
    {
        float normalizedX = Mathf.Clamp01(touchPosition.x / Screen.width);
        float movementX = Mathf.Lerp(limiteIzquierda, limiteDerecha, normalizedX);
        dinosaurio.position = posicionInicial + ejeX * movementX;
    }

   
}
