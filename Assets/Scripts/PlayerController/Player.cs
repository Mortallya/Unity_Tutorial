using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 3f; // Sensibilité de la souris
    [SerializeField] private float movementSpeed = 5f; // Vitesse de deplacement
    [SerializeField] private float jumpSpeed = 5f; // Vitesse de saut
    [SerializeField] private float playerMass = 1f; // Masse du personnage
    [SerializeField] private float acceleration = 20f; // Masse du personnage

    public event Action OnBeforeMove;

    internal float movementSpeedMultiplier;

    private Camera playerCamera; // Camera du personnage
    private Vector2 mouseLook; // Vecteur de rotation de la souris
    internal Vector3 velocity; // Vitesse du personnage
    private CharacterController characterController; // Controleur du personnage

    private PlayerInput playerInput; // Input action asset du personnage
    private InputAction moveAction; // Input du personnage pour le déplacement
    private InputAction lookAction; // Input du personnage pour la rotation
    private InputAction jumpAction; // Input du personnage pour le saut
    private InputAction sprintAction; // Input du perso pour le sprint
    void Awake()
    {
        characterController = GetComponent<CharacterController>(); // Récupère le CharacterController
        playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>(); // Récupère la camera du personnage
        playerInput = GetComponent<PlayerInput>(); // Récupère l'input action asset du personnage

        moveAction = playerInput.actions["move"]; // Récupère l'input du personnage pour l'action de deplacement (clavier WASD )
        lookAction = playerInput.actions["look"]; // Récupère l'input du personnage pour l'action de rotation (souris)
        jumpAction = playerInput.actions["jump"]; // Récupère l'input du personnage pour l'action de saut (barre espace)
        sprintAction = playerInput.actions["sprint"]; //Récupère l'input du perso pour l'action du sprint
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Mode de verrouillage du curseur de la souris (cacher le curseur0)
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGravity();
        UpdateMovement();  
        UpdateLook();
    }

    void UpdateGravity(){
        Vector3 gravity = Physics.gravity * playerMass *Time.deltaTime; // Calcul de la gravité
        velocity.y = characterController.isGrounded ? -1f : velocity.y + gravity.y; // Annulation de la gravité si le personnage est au sol et sinon ajout de la gravité
    }

    Vector3 GetMovementInput()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>(); // Récupère la valeur (x,y) de l'axe vertical et horizontal del'input de déplacement

        Vector3 input = new Vector3(); // Vecteur de deplacement 3D
        input += transform.forward * moveInput.y; // Ajoute la valeur de l'axe vertical au vecteur de deplacement
        input += transform.right * moveInput.x; // Ajoute la valeur de l'axe horizontal au vecteur de deplacement
        input = Vector3.ClampMagnitude(input, 1f); // Limitation du vecteur de deplacement
        float sprintInput = sprintAction.ReadValue<float>();
        var multiplier = sprintInput > 0 ? 1.5f : 1f;
        input *= movementSpeed * movementSpeedMultiplier; // Multiplication du vecteur de deplacement par la vitesse de deplacement
        return input; // Retourne le vecteur de deplacement
    }

    void UpdateMovement()
    {
        movementSpeedMultiplier = 1f;
        OnBeforeMove?.Invoke();
        
        Vector3 input = GetMovementInput(); // Récupère le vecteur de deplacement via la fonction correspondante       

        velocity.x = Mathf.Lerp(velocity.x, input.x, acceleration * Time.deltaTime); // Ajoute la vitesse de deplacement au vecteur de deplacement x en prenant en compte l'accélération progressive (Lerp)
        velocity.z = Mathf.Lerp(velocity.z, input.z, acceleration * Time.deltaTime); // Ajoute la vitesse de deplacement au vecteur de deplacement z en prenant en compte l'accélération progressive (Lerp)

        float jumpInput = jumpAction.ReadValue<float>(); // Récupère la valeur de l'axe de saut

        if(jumpInput > 0 && characterController.isGrounded){
            velocity.y += jumpSpeed; // Ajoute la vitesse de saut au vecteur de deplacement
        }

        characterController.Move((velocity) * Time.deltaTime); // Deplacement du personnage via le vecteur de deplacement dans le characterController
    }

    void UpdateLook()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        mouseLook.x += lookInput.x * mouseSensitivity; // Récupère la valeur de l'axe horizontal de la souris
        mouseLook.y += lookInput.y * mouseSensitivity; // Récupère la valeur de l'axe vertical de la souris
        mouseLook.y = Mathf.Clamp(mouseLook.y, -20f, 20f); // Limite la valeur de l'axe vertical de la souris

        playerCamera.transform.localRotation = Quaternion.Euler(-mouseLook.y * mouseSensitivity, 0.0f, 0.0f); // Rotation de la camera (verticale)


        transform.localRotation = Quaternion.Euler(0.0f, mouseLook.x * mouseSensitivity, 0.0f); // Rotation du personnage (horizontale)
    }
}

