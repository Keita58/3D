using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject camaraPrimera;
    [SerializeField] GameObject camaraTercera;
    [SerializeField] GameObject _Pivot;
    InputSystem_Actions _inputActions;
    [SerializeField] Transform puntoDisparo;
    [SerializeField] GameObject pistola;
    [SerializeField] GameObject itemSlot;
    public List<Item> inventari { get; private set; }

    InputAction _MoveAction;
    InputAction _LookAction;
    InputAction _ScrollAction;
    InputAction _RunAction;
    //Rigidbody rb;

    [Tooltip("Velocitat de moviment del jugador.")]
    [SerializeField] private float _VelocityRun = 6f;
    [SerializeField] private float _VelocityMove = 3f;

    [Tooltip("Velocitat de mouse en graus per segon.")]
    [Range(10f, 360f)]
    [SerializeField] private float _LookVelocity = 100;

    [SerializeField] private bool _InvertY = false;
    private Vector2 _LookRotation = Vector2.zero;
    Animator animator;
    CharacterController characterController;

    [SerializeField] public float hp = 50.0f;

    float maxAngle = 45.0f;
    float minAngle = -30.0f;
    float vSpeed = 0;
    float gravity = 9.8f;
    float jumpSpeed = 4.0f;
    float minDistanceCamera = 3f;
    float maxDistancecamera = 7f;

    [SerializeField] LayerMask _LayerEnemic;
    [SerializeField] LayerMask _InteractLayerMask;
    [SerializeField] LayerMask _CameraCollisionMask;
    [SerializeField] Collider[] colliders;
    [SerializeField] private float _CameraDistance = 5f;
    [SerializeField] Material material;
    Vector3 camaraInitialPosition;
    bool salto = false;
    bool moving=false;

    Vector3 localScaleCollider;
    Vector3 localPositionCollider;
    bool agachado = false;
    bool primeraPersona = true;
    [SerializeField] bool tengoItem=false;
    [SerializeField] private GameObject interactuable;
    [SerializeField] private Material materialBase;
    bool inventariObert = false;
    bool corriendo = false;
    bool empezarCorrutinaCorrer = false;
    public int _Bales { get; private set; }
    GameObject objetoEquipado;

    public event Action onInteractuable;
    public event Action onNotInteractuable;
    public event Action OnDisparar;

    [SerializeField] private VisualEffect _Pols;

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
        _inputActions.Player.CogerItem.performed += CogerItem;
        _ScrollAction= _inputActions.Player.MouseWheel;
        _inputActions.Player.Inventari.performed += ObrirTancarInventari;
        _inputActions.Player.LanzarItem.performed += LanzarObjeto;
        _RunAction= _inputActions.Player.Run;
        _ScrollAction = _inputActions.Player.MouseWheel;
        localScaleCollider = this.transform.localScale;
        inventari = new List<Item>();
        _Bales = 0;


        _inputActions.Player.Enable();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        camaraInitialPosition = camaraPrimera.transform.localPosition;
    }

    private void ObrirTancarInventari(InputAction.CallbackContext context)
    {
        if (!inventariObert)
        {
            Cursor.visible = true;
            GameManager.instance.ObrirInventari(this.gameObject);
            inventariObert = true;
        }
        else
        {
            Cursor.visible = false;
            GameManager.instance.TancarInventari();
            inventariObert = false;
        }
    }

    public void EquiparItem(GameObject itemAEquipar)
    {
        if (!tengoItem)
        {
            GameObject equipo = Instantiate(itemAEquipar, itemSlot.transform);
            equipo.transform.position = itemSlot.transform.position;
            tengoItem = true;
            objetoEquipado = equipo;
        }
    }

    private void CogerItem(InputAction.CallbackContext context)
    {
        if (interactuable != null)
        {
     
            GameManager.instance.AfegirItem(interactuable.GetComponent<CogerItem>().item);
            Debug.Log("QUE COJO?"+interactuable.GetComponent<CogerItem>().item);
            interactuable.gameObject.SetActive(false);
            Debug.Log("Entro Coger item");
        }
        
    }

    public void RecarregaBales(int bales)
    {
        _Bales += bales;
        OnDisparar?.Invoke();
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
            this.characterController.height=1f;
            float centerCharacterController =this.characterController.center.y;
            centerCharacterController = 1f;
            agachado = true;
            _VelocityRun /= 2;
            moving = false;
            StartCoroutine(EmetreSOCrouch());
        }
        else
        {
            this.GetComponent<CapsuleCollider>().height = 2;
            float center = this.gameObject.GetComponent<CapsuleCollider>().center.y;
            center = 0f;
            this.characterController.height = 2f;
            float centerCharacterController = this.characterController.center.y;
            centerCharacterController = 0f;
            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
            characterController.enabled = false;
            characterController.enabled=true;
            agachado = false;
            moving = true;
            _VelocityRun *= 2;
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
        if(_Bales >= 1)
        {
            Debug.DrawRay(puntoDisparo.transform.position, puntoDisparo.transform.forward, Color.magenta, 5f);
            Debug.Log("TIRO DEBUGRAY");
            if(Physics.Raycast(puntoDisparo.transform.position, puntoDisparo.transform.forward, _LayerEnemic))
            {
                e.RebreMal(5);
            }
            _Bales--;
            OnDisparar?.Invoke();
        }
    }

    private void LanzarObjeto(InputAction.CallbackContext context)
    {
        if (objetoEquipado != null)
        {
            objetoEquipado.GetComponent<Rigidbody>().isKinematic = false;
            objetoEquipado.GetComponent<ObjectsScript>().camaraPrimera=camaraPrimera;
            //faltaria ponerle el sonido
            objetoEquipado.GetComponent<ObjectsScript>().Lanzar();
            objetoEquipado.transform.parent = null;
        }
    }

    enum PlayerStates { MOVE, RUN, HURT, RUNMOVE }
    [SerializeField] PlayerStates actualState;
    [SerializeField] float stateTime;


    void Start()
    {
        Cursor.visible = false;
        ChangeState(PlayerStates.MOVE);
        StartCoroutine(interactuarRaycast());

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

    public IEnumerator interactuarRaycast()
    {
        while (true)
        {
            Debug.DrawRay(camaraPrimera.transform.position, camaraPrimera.transform.forward, Color.magenta, 5f);
            //Lanzar Raycast interactuar con el mundo.
            
            if (Physics.Raycast(camaraPrimera.transform.position, camaraPrimera.transform.forward, out RaycastHit hit, 5f, _InteractLayerMask)
                && !hit.collider.gameObject.Equals(interactuable))
            {
                interactuable = hit.collider.gameObject;
                materialBase = interactuable.GetComponent<MeshRenderer>().materials[0];
                interactuable.GetComponent<MeshRenderer>().materials = new Material[]
                {
                    interactuable.GetComponent<MeshRenderer>().materials[0],
                    
                    material
                };
                onInteractuable?.Invoke();
            }
            else if (!Physics.Raycast(camaraPrimera.transform.position, camaraPrimera.transform.forward, out RaycastHit hit2, 10f, _InteractLayerMask))
            {
                if (interactuable != null)
                {
                    interactuable.GetComponent<MeshRenderer>().materials = new Material[] { interactuable.GetComponent<MeshRenderer>().materials[0] };
                    interactuable = null;
                }
                onNotInteractuable?.Invoke();
            }
            yield return new WaitForSeconds(0.5f);
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
            case PlayerStates.MOVE:
                moving = true;
                StartCoroutine(EmetreSOMove());
                break;
            case PlayerStates.RUN:
                corriendo = true;
                StartCoroutine(EmetreSORun());
                StartCoroutine(EmetrePols());
                break;
            default:
                break;
        }
    }

    private void UpdateState()
    {
        Vector2 movementInput = _MoveAction.ReadValue<Vector2>();

        stateTime += Time.deltaTime;
        switch (actualState)
        {
            
            case PlayerStates.MOVE:
                if (salto)
                {
                    vSpeed = jumpSpeed;
                    salto = false;
                }
                Vector3 vel = (transform.right * movementInput.x +
                    transform.forward * movementInput.y).normalized * _VelocityMove;

                if (vel == Vector3.zero)
                {
                    moving= false;
                }
                else
                {
                    if (!moving)
                    {
                        moving = true;
                        StartCoroutine(EmetreSOMove());
                        StartCoroutine(EmetrePols());
                    }
                }
               
                if (_RunAction.IsPressed() && characterController.isGrounded)
                {
                    ChangeState(PlayerStates.RUN);
                    Debug.Log("ENTRO");
                }
                vSpeed -= gravity * Time.deltaTime;
                vel.y = vSpeed;

                characterController.Move(vel * Time.deltaTime);

                break;
            case PlayerStates.RUN:
                if (salto)
                {
                    vSpeed = jumpSpeed;
                    salto = false;
                }
                Vector3 velRun = (transform.right * movementInput.x +
                    transform.forward * movementInput.y).normalized * _VelocityRun;

                if (velRun == Vector3.zero || !_RunAction.IsPressed())
                {
                    ChangeState(PlayerStates.MOVE);
                }
                vSpeed -= gravity * Time.deltaTime;
                velRun.y = vSpeed;

                characterController.Move(velRun * Time.deltaTime);
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
            case PlayerStates.RUN:
                corriendo=false;
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
            foreach (Collider collider in colliderHits)
            {
                if (collider.gameObject.TryGetComponent<Enemic>(out Enemic en))
                {
                    en.Escuchar(this.transform.position, 2);
                }
            }
            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator EmetreSORun()
    {
        while (corriendo)
        {
            Collider[] colliderHits = Physics.OverlapSphere(this.transform.position, 7);
            if (GetComponent<Collider>().gameObject.TryGetComponent<Enemic>(out Enemic en))
            {
                en.Escuchar(this.transform.position, 7);
            }
            yield return new WaitForSeconds(1);
        }
    }
    
    IEnumerator EmetreSOCrouch()
    {
        while (agachado)
        {
            Collider[] colliderHits = Physics.OverlapSphere(this.transform.position, 5);
            if (GetComponent<Collider>().gameObject.TryGetComponent<Enemic>(out Enemic en))
            {
                en.Escuchar(this.transform.position, 5);
            }
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator EmetrePols()
    {
        while(moving || corriendo)
        {
            _Pols.Play();
            if(corriendo)
                yield return new WaitForSeconds(0.45f);
            else
                yield return new WaitForSeconds(0.6f);
        }
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
    [SerializeField]
    Enemic e;
    [SerializeField]
    Light luzFalla;
    [SerializeField]
    AudioClip luzFallaAudio;
    [SerializeField]
    Light luzExplota;
    [SerializeField]
    AudioClip luzExplotaAudio;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entro");
        if (other.name == "TriggerActivarEnemic")
        {
            e.GetComponent<Enemic>().enabled = true;
            e.GetComponent<NavMeshAgent>().enabled = true;
            e.GetComponent<Animator>().enabled = true;
            Destroy(other.gameObject);
        }else if(other.name == "TriggerFalloLuz")
        {
            luzFalla.GetComponent<AudioSource>().clip = luzFallaAudio;
            luzFalla.GetComponent<AudioSource>().Play();
            luzFalla.GetComponent<Animator>().Play("FalloLuz");
            Destroy(other.gameObject);
        }
        else if (other.name == "TriggerLuzExplota")
        {
            luzFalla.GetComponent<AudioSource>().clip = luzExplotaAudio;
            luzFalla.GetComponent<AudioSource>().Play();
            Destroy(luzExplota.gameObject);
            Destroy(other.gameObject);
        }
    }
}
