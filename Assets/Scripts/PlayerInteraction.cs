using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] private Interactable currentInteractable;

    //[SerializeField] private Camera camera;

    private PlayerInput playerInput;
    private InputAction useAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        useAction = playerInput.actions["use"];
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerOrientation = cameraTransform.forward;

        if(Physics.Raycast(transform.position, playerOrientation, out RaycastHit hitObj, 1.5f,LayerMask.GetMask("Usables")))
        {
            if(hitObj.transform.TryGetComponent<Interactable>(out Interactable interactable))
            {
                currentInteractable = interactable;
            }
        } else {currentInteractable = null;
                }
        float useInput = useAction.ReadValue<float>();
        if(useInput > 0 && currentInteractable) currentInteractable.Interact();
    }
}
