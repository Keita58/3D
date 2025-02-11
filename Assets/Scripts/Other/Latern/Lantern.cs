using UnityEngine;
using UnityEngine.InputSystem;

public class Lantern : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Public variables

    // Private variables
    private Light flashlight;
    private InputSystem_Actions _InputActions;
    private void Start()
    {
        _InputActions = new InputSystem_Actions();
        _InputActions.Player.Flashlight.started += FlashLight;
        _InputActions.Player.Enable();

        // Get Light component in the same GameObject
        flashlight = GetComponent<Light>();

        if (flashlight == null)
        {
            Debug.LogWarning("Light component is not attached. Attach a Light component manually.");
        }
        else
        {
            flashlight.enabled = false;
        }
    }

    private void FlashLight(InputAction.CallbackContext context)
    {
        if (flashlight != null)
        {
            flashlight.enabled = !flashlight.enabled;

            // Play audio effect based on flashlight state
        }
        else
        {
            Debug.LogWarning("Cannot control flashlight as Light component is not attached.");
        }
    }
}
