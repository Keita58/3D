using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject camaraPrimera;
    [SerializeField] GameObject camaraTercera;
    [SerializeField] GameObject _Pivot;
    InputSystem_Actions _inputActions;
    [SerializeField] Transform puntoDisparo;
    [SerializeField] GameObject pistola;

    InputAction _MoveAction;
    InputAction _LookAction;
    InputAction _ScrollAction;
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
    float minDistanceCamera = 3f;
    float maxDistancecamera = 7f;

    [SerializeField] LayerMask layerMask;
    [SerializeField] LayerMask _CameraCollisionMask;
    [SerializeField] Collider[] colliders;
    [SerializeField] private float _CameraDistance = 5f;
    Vector3 camaraInitialPosition;
    bool salto = false;
    bool moving=false;

    Vector3 localScaleCollider;
    Vector3 localPositionCollider;
    bool agachado = false;
    bool primeraPersona = true;


    private void Awake()
    {
        Debug.Assert(camaraPrimera is not null, "Camera no assignada, espabila Hector!");
        _inputActions = new InputSystem_Actions();
        _MoveAction = _inputActions.Player.Move;
        _LookAction = _inputActions.Player.Look;
        _inputActions.Player.Attack.performed += Attack;
        _inputActions.Player.Jump.performed += Jump;
        _inputActions.Player.Crouch.performed += Crouch;
        _inputActions.Player.CambiarCamera.performed += CambiarCamara;
        _ScrollAction= _inputActions.Player.MouseWheel;
        localScaleCollider = this.transform.localScale;

        _inputActions.Player.Enable();
        //rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        camaraInitialPosition = camaraPrimera.transform.localPosition;
    }

    private void CambiarCamara(InputAction.CallbackContext context)
    {
        primeraPersona = !primeraPersona;
        if (primeraPersona)
        {
            camaraTercera.gameObject.SetActive(false);
            camaraPrimera.SetActive(true);
        }
        else
        {
            camaraPrimera.SetActive(false);
            camaraTercera.gameObject.SetActive(true);
        }
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        if (!agachado)
        {
            this.GetComponent<CapsuleCollider>().height = 1;
            float center =this.gameObject.GetComponent<CapsuleCollider>().center.y;
            center = 1f;
            this.characterController.height = 1f;
            float centerCharacterController =this.characterController.center.y;
            centerCharacterController = 1f;
            agachado = true;
            _Velocity /= 2;
        }
        else
        {
            this.GetComponent<CapsuleCollider>().height = 2;
            float center = this.gameObject.GetComponent<CapsuleCollider>().center.y;
            center = 0f;
            this.characterController.height = 2f;
            float centerCharacterController = this.characterController.center.y;
            centerCharacterController = 0f;
            agachado =false;
            _Velocity *= 2;
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

        UpdateState();
        if (primeraPersona)
        {
            MovimentCamera1aPersona();
        }
        else
        {
            MovimentCamera();
        }

        float zoom = _ScrollAction.ReadValue<float>();
        if (zoom > 0)
        {
            if (_CameraDistance>=minDistanceCamera) _CameraDistance--;
        }else if (zoom < 0)
        {
            if (_CameraDistance<=maxDistancecamera) _CameraDistance++;
        }

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

    public void MovimentCamera()
    {
        Vector2 lookInput = _LookAction.ReadValue<Vector2>();

        _LookRotation.x += lookInput.x * _LookVelocity * Time.deltaTime;
        _LookRotation.y += (_InvertY ? 1 : -1) * lookInput.y * _LookVelocity * Time.deltaTime;
        
        _LookRotation.y = Mathf.Clamp(_LookRotation.y, -35, 35);
        //camaraTercera.UpdateCamera(_LookRotation, _CameraDistance);
        camaraTercera.transform.position = _Pivot.transform.position;
        camaraTercera.transform.localRotation = Quaternion.Euler(_LookRotation.y, _LookRotation.x, 0);
        if (Physics.Raycast(camaraTercera.transform.position, -camaraTercera.transform.forward, out RaycastHit hit, _CameraDistance, _CameraCollisionMask))
        {
            camaraTercera.transform.position = hit.point + camaraTercera.transform.forward * 0.1f;
        }
        else
        {
            camaraTercera.transform.position -= camaraTercera.transform.forward * _CameraDistance;
        }
        //camaraTercera.transform.localRotation = Quaternion.Euler(_LookRotation.y, _LookRotation.x, 0);
        transform.forward = Vector3.ProjectOnPlane(camaraTercera.transform.forward, Vector3.up);
    }

    public void MovimentCamera1aPersona()
    {
        Vector2 lookInput = _LookAction.ReadValue<Vector2>();
        _LookRotation.x += lookInput.x * _LookVelocity * Time.deltaTime;
        _LookRotation.y += (_InvertY ? 1 : -1) * lookInput.y * _LookVelocity * Time.deltaTime;

        _LookRotation.y = Mathf.Clamp(_LookRotation.y, minAngle, maxAngle);
        transform.rotation = Quaternion.Euler(0, _LookRotation.x, 0);
        camaraPrimera.transform.localRotation = Quaternion.Euler(_LookRotation.y, 0, 0);
    }


}
