using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]
public class PlayerComponent : MonoBehaviour
{

    //D'aquesta forma teniu l'input action a un json i podeu accedir
    //a totes les seves variables com a camps. Tot i aix�, no podreu
    //fer modificacions als bindings i d'altres.
    private InputSystem_Actions _inputActions;

    private InputAction _MoveAction;
    private InputAction _LookAction;
    private Rigidbody _RigidBody;

    [SerializeField] private GameObject _Camera;

    [Tooltip("Velocitat de moviment del jugador.")]
    [Range(0.1f, 20f)]
    [SerializeField] private float _Velocity = 3;

    [Tooltip("Velocitat de mouse en graus per segon.")]
    [Range(10f, 360f)]
    [SerializeField] private float _LookVelocity = 180;

    [SerializeField] private bool _InvertY = true;
    private Vector2 _LookRotation = Vector2.zero;

    //[SerializeField] private LayerMask _UseLayerMask;

    public event Action<Vector3> OnSetDestination;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Assert(_Camera is not null, $"Camera del player no assignada.");

        _inputActions = new InputSystem_Actions();

        _MoveAction = _inputActions.Player.Move;
        _LookAction = _inputActions.Player.Look;

        _inputActions.Player.Enable();

        _RigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //C�mera
        Vector2 lookInput = _LookAction.ReadValue<Vector2>();

        _LookRotation.x += lookInput.x * _LookVelocity * Time.deltaTime;
        _LookRotation.y += (_InvertY ? 1 : -1) * lookInput.y * _LookVelocity * Time.deltaTime;

        //_Camera.transform.rotation = Quaternion.Euler(_LookRotation.y, _LookRotation.x, 0);
        transform.rotation = Quaternion.Euler(0, _LookRotation.x, 0);
        _Camera.transform.localRotation = Quaternion.Euler(_LookRotation.y, 0, 0);


        Move();

        //_RigidBody.linearVelocity = 
        //    (_Camera.transform.right * movementInput.x + 
        //    Vector3.ProjectOnPlane(_Camera.transform.forward, Vector3.up).normalized * movementInput.y)
        //    .normalized * _Velocity;

        // Aquest moviment no interessa perqu� no t� en compte la rotaci� de l'objecte
        //_RigidBody.linearVelocity = (new Vector3(movementInput.x, 0, movementInput.y)).normalized * 3;
    }
    bool a = false;
    public void Move()
    {
        //Moviment
        Vector2 movementInput = _MoveAction.ReadValue<Vector2>();
        //print(movementInput);
        if ((movementInput.x != 0 || movementInput.y != 0))
        {
            if (!a)
            {
                a = true;
                StartCoroutine(EmetreSOMove());
            }
        }
        else
        {
            a = false;
        }
        _RigidBody.linearVelocity =
            (transform.right * movementInput.x +
            transform.forward * movementInput.y)
            .normalized * _Velocity;
    }
    IEnumerator EmetreSOMove()
    {
        while (a)
        {
            Collider[] colliderHits = Physics.OverlapSphere(this.transform.position, 30);
            foreach (Collider collider in colliderHits)
            {
                if (collider.gameObject.TryGetComponent<Enemic>(out Enemic en))
                {
                    en.Escuchar(this.transform.position, 2);
                }
            }
            yield return new WaitForSeconds(1);
        }
    }
    void EmetreSORun()
    {
        Collider[] colliderHits = Physics.OverlapSphere(this.transform.position, 7);
        if (GetComponent<Collider>().gameObject.TryGetComponent<Enemic>(out Enemic en))
        {
            en.Escuchar(this.transform.position, 7);
        }
        new WaitForSeconds(1);
    }

}
