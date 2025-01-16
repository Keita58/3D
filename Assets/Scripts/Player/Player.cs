using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject camara;
    InputSystem_Actions _inputActions;
    [SerializeField] Transform puntoDisparo;
    [SerializeField] GameObject pistola;

    InputAction _MoveAction;
    InputAction _LookAction;
    InputAction _AttackAction;
    InputAction _JumpAction;
    InputAction _CrouchAction;
    //Rigidbody rb;

    [Tooltip("Velocitat de moviment del jugador.")]
    [Range(0.1f, 20f)]
    [SerializeField] private float _Velocity = 3;

    [Tooltip("Velocitat de mouse en graus per segon.")]
    [Range(10f, 360f)]
    [SerializeField] private float _LookVelocity = 100;

    [SerializeField] private bool _InvertY = false;
    private Vector2 _LookRotation = Vector2.zero;
    Animator animator;
    CharacterController characterController;

    [SerializeField] float hp = 50.0f;

    float maxAngle = 45.0f;
    float minAngle = -30.0f;
    float vSpeed = 0;
    float gravity = 9.8f;
    float jumpSpeed = 4.0f;

    [SerializeField] LayerMask layerMask;
    [SerializeField] Collider[] colliders;
    Vector3 camaraInitialPosition;
    bool salto = false;
    bool moving=false;

    Vector3 localScaleCollider;
    Vector3 localPositionCollider;
    bool agachado = false;


    private void Awake()
    {
        Debug.Assert(camara is not null, "Camera no assignada, espabila Hector!");
        _inputActions = new InputSystem_Actions();
        _MoveAction = _inputActions.Player.Move;
        _LookAction = _inputActions.Player.Look;
        _inputActions.Player.Attack.performed += Attack;
        _inputActions.Player.Jump.performed += Jump;
        _inputActions.Player.Crouch.performed += Crouch;
        localScaleCollider = this.transform.localScale;

        _inputActions.Player.Enable();
        //rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        camaraInitialPosition = camara.transform.localPosition;
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        if (!agachado)
        {
            this.gameObject.GetComponent<CapsuleCollider>().transform.localScale = transform.localScale / 2;
            this.gameObject.GetComponent<CapsuleCollider>().transform.localPosition = transform.localPosition / 2;
            agachado = true;
        }
        else
        {
            this.gameObject.GetComponent<CapsuleCollider>().transform.localScale =localScaleCollider;
            this.gameObject.GetComponent<CapsuleCollider>().transform.localPosition = this.transform.localPosition;
            agachado=false;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded)
        {
            salto = true;
        }
        
    }

    private void Attack(InputAction.CallbackContext context)
    {
        Debug.DrawRay(puntoDisparo.transform.position, puntoDisparo.transform.forward, Color.magenta, 5f);
        Debug.Log("TIRO DEBUGRAY");
    }

    enum PlayerStates { IDLE, MOVE, RUN, HURT }
    [SerializeField] PlayerStates actualState;
    [SerializeField] float stateTime;


    void Start()
    {
        Cursor.visible = false;
        ChangeState(PlayerStates.IDLE);

    }

    void Update()
    {
        localPositionCollider = this.transform.localPosition;
        Vector2 lookInput = _LookAction.ReadValue<Vector2>();
        _LookRotation.x += lookInput.x * _LookVelocity * Time.deltaTime;
        _LookRotation.y += (_InvertY ? 1 : -1) * lookInput.y * _LookVelocity * Time.deltaTime;

        _LookRotation.y = Mathf.Clamp(_LookRotation.y, minAngle, maxAngle);
        transform.rotation = Quaternion.Euler(0, _LookRotation.x, 0);
        camara.transform.localRotation = Quaternion.Euler(_LookRotation.y, 0, 0);
        UpdateState();
    }

    private void ChangeState(PlayerStates newstate)
    {
        ExitState(actualState);
        IniState(newstate);
    }

    private void IniState(PlayerStates initState)
    {
        actualState = initState;
        stateTime = 0f;

        switch (actualState)
        {
            case PlayerStates.IDLE:
                //rb.linearVelocity = Vector2.zero;
                //rb.angularVelocity = Vector3.zero;
                break;
            case PlayerStates.MOVE:
                moving = true;
                StartCoroutine(EmetreSOMove());
                break;
            default:
                break;
        }
    }

    private void UpdateState()
    {
        //_Moviment s l'action de l'InputAction especfic de Player
        Vector2 movementInput = _MoveAction.ReadValue<Vector2>();

        stateTime += Time.deltaTime;

        switch (actualState)
        {
            case PlayerStates.IDLE:
                if (!characterController.isGrounded || movementInput != Vector2.zero || salto)
                    ChangeState(PlayerStates.MOVE);
                break;
            case PlayerStates.MOVE:
                if (movementInput == Vector2.zero && characterController.isGrounded && !salto)
                {
                    ChangeState(PlayerStates.IDLE);

                    break;
                }
                if (salto)
                {
                    vSpeed = jumpSpeed;
                    salto = false;
                }
                Vector3 vel = (transform.right * movementInput.x +
                    transform.forward * movementInput.y).normalized * _Velocity;

                vSpeed -= gravity * Time.deltaTime;
                vel.y = vSpeed;

                characterController.Move(vel * Time.deltaTime);

                break;
        }
    }

    private void ExitState(PlayerStates exitState)
    {
        switch (exitState)
        {
            case PlayerStates.MOVE:
                //Comentar por si hacemos que haya mini estados.
                //rb.linearVelocity = Vector2.zero;
                //rb.angularVelocity = Vector3.zero;
                moving=false;
                break;
            default:
                break;
        }
    }

    IEnumerator EmetreSOMove()
    {
        while (moving)
        {
            Collider[] colliderHits = Physics.OverlapSphere(this.transform.position, 30);
            Debug.Log("Mi posicion: " + this.transform.position);
            foreach (Collider collider in colliderHits)
            {
                if (collider.gameObject.TryGetComponent<Enemy>(out Enemy en))
                {
                    en.Escuchar(this.transform.position, 1);
                }
            }
            Debug.Log("Corrutina sonido");
            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator EmetreSORun()
    {
        Collider[] colliderHits = Physics.OverlapSphere(this.transform.position, 7);
        if (GetComponent<Collider>().gameObject.TryGetComponent<Enemy>(out Enemy en))
        {
            en.Escuchar(this.transform.position, 7);
        }
        yield return new WaitForSeconds(1);
    }

}
