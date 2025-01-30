using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float gravityScale;

    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float originalScale;

    [SerializeField]
    private float scaleWhenCrouched;

    [SerializeField]
    private float crouchedSpeed;

    [Header("Ground Detection")]
    [SerializeField]
    private Transform feet;

    [SerializeField]
    private float detectionRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    private Vector3 verticalMovement;
    private CharacterController controller;
    private Camera cam;

    private PlayerInput playerInput;

    private Vector2 input;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void OnEnable()
    {
        playerInput.actions["Jump"].started += Jump;
        playerInput.actions["Move"].performed += Move;
        playerInput.actions["Move"].canceled += MoveCanceled;



        //Se puede hacer cambio de mapas con
        //playerInput.SwitchCurrentActionMap("UI"); lo de dentro de parentesis es el mapa que quiero
        //Los cambiode de device los es con playerInput.deviceLostEvent.AddListener( (x ,esto es un parametro de entrada) => Debug.Log("Device perdido"));  y se pueden poner Lambdas dentro
    }
    private void Move(InputAction.CallbackContext ctx)
    {
        input = ctx.ReadValue<Vector2>();
    }

    private void MoveCanceled(InputAction.CallbackContext ctx)
    {
        input = Vector2.zero;
       
    }


    private void Jump(InputAction.CallbackContext ctx)
    {
        if (IsGrounded())
        {
            verticalMovement.y = 0;
            verticalMovement.y = Mathf.Sqrt(-2 * gravityScale * jumpHeight);
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveAndRotate();
        ApplyGravity();
        Crouch();
        

    }

    private void Crouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            transform.localScale /= 2; //A la mitad TODO para que no deforme objetos.
        }
        else if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            transform.localScale *= 2;
        }
    }

    private void MoveAndRotate()
    {
       

        //Se aplica al cuerpo la rotación que tenga la cámara.
        transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

        ////Si hay input...
        if (input.sqrMagnitude > 0)
        {
            //Se calcula el ángulo en base a los inputs
            float angleToRotate = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;

            //Se rota el vector (0, 0, 1) a dicho ángulo
            Vector3 movementInput = Quaternion.Euler(0, angleToRotate, 0) * Vector3.forward;

            //Se aplica movimiento en dicha dirección.
            controller.Move(movementInput * movementSpeed * Time.deltaTime);
        }
    }

    private void Jump()
    {
        if(IsGrounded())
        {
            verticalMovement.y = 0;

            if(Input.GetKeyDown(KeyCode.Space))
            {
                verticalMovement.y = Mathf.Sqrt(-2 * gravityScale * jumpHeight);
            }
        }
    }

    private void ApplyGravity()
    {
        verticalMovement.y += gravityScale * Time.deltaTime;
        controller.Move(verticalMovement * Time.deltaTime);
    }
    private bool IsGrounded()
    {
        return Physics.CheckSphere(feet.position, detectionRadius, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(feet.position, detectionRadius);
    }
}
