using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

public class Enemic : MonoBehaviour
{
    private enum EnemyStates { PATRULLA, INVESTIGAR, PERSEGUIR, ATACAR, NOQUEJAT }
    [SerializeField] private EnemyStates _CurrentState;
    [SerializeField] private float _StateTime;
    [SerializeField] private bool _Detectat;
    [SerializeField] private bool _Cami;
    [SerializeField] private bool _AtacarBoolean;
    [SerializeField] private Collider[] _DetectarCollider;
    [SerializeField] private LayerMask _LayerJugador;
    [SerializeField] private GameObject _Jugador;
    [SerializeField] private GameObject[] _PuntsMapa;
    [SerializeField] private GameObject _Camera; //Treure quan ho tingui Jugador
    [SerializeField] private bool _InvertY = true;

    [Tooltip("Velocitat de mouse en graus per segon.")]
    [Range(10f, 360f)]
    [SerializeField] private float _LookVelocity = 180; //Treure quan ho tingui Jugador

    private NavMeshAgent _NavMeshAgent;
    private Collider[] _Atacar;
    private System.Random _Random;
    //private Animator _Animacio;
    private InputSystem_Actions _InputActions;
    private InputAction _LookAction; //Treure quan ho tingui Jugador
    private InputAction _MoveAction;
    private Vector2 _LookRotation; //Treure quan ho tingui Jugador
    [SerializeField] RaycastHit[] hits;

    private void Awake()
    {
        _InputActions = new InputSystem_Actions();
        _MoveAction = _InputActions.Player.Move;
        _LookAction = _InputActions.Player.Look;
     //   _Animacio = GetComponent<Animator>();
        _NavMeshAgent = GetComponent<NavMeshAgent>();

        _InputActions.Player.Enable();
    }

    private void Start()
    {
        _Random = new System.Random();
        InitState(EnemyStates.PATRULLA);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ChangeState(EnemyStates newState)
    {
        if (newState == _CurrentState)
            return;

        ExitState(_CurrentState);
        InitState(newState);
    }

    private void InitState(EnemyStates initState)
    {
        _CurrentState = initState;
        _StateTime = 0f;

        switch (_CurrentState)
        {
            case EnemyStates.PATRULLA:
             //   _Animacio.Play("Run");
                _Detectat = false;
                StartCoroutine(Patrullar());
                break;
            case EnemyStates.INVESTIGAR:
               // _Animacio.Play("Run");
                break;
            case EnemyStates.PERSEGUIR:
                //_Animacio.Play("Run");
                break;
            case EnemyStates.ATACAR:
                break;
            case EnemyStates.NOQUEJAT:
                _NavMeshAgent.destination = transform.position;
                break;
            default:
                break;
        }
    }

    private void UpdateState(EnemyStates updateState)
    {
        _StateTime += Time.deltaTime;

        switch (updateState)
        {
            case EnemyStates.INVESTIGAR:
                _DetectarCollider = Physics.OverlapSphere(transform.position, 10f, _LayerJugador);
                
                if (_DetectarCollider.Length > 0)
                {
                    Debug.Log("Detecto alguna cosa aprop!");
                    _NavMeshAgent.destination = _Jugador.transform.position;
                    ChangeState(EnemyStates.PERSEGUIR);
                }
                
                break;
            case EnemyStates.PERSEGUIR:
                _DetectarCollider = Physics.OverlapSphere(transform.position, 10f, _LayerJugador);
                _Atacar = Physics.OverlapSphere(transform.position, 5f, _LayerJugador);

                if (_DetectarCollider.Length > 0)
                {
                    Debug.Log("Detecto alguna cosa aprop!");
                    _NavMeshAgent.destination = _Jugador.transform.position;
                }
                else
                {
                    ChangeState(EnemyStates.PATRULLA);
                    Debug.Log("No detecto res!");
                    _NavMeshAgent.destination = transform.position;
                }
                break;
            case EnemyStates.ATACAR:
                break;
            case EnemyStates.NOQUEJAT:
                break;
            default:
                break;
        }
    }

    private void ExitState(EnemyStates exitState)
    {
        switch (exitState)
        {
            case EnemyStates.PATRULLA:
                _Detectat = true;
                break;
            case EnemyStates.INVESTIGAR:
            case EnemyStates.PERSEGUIR:
                break;
            case EnemyStates.ATACAR:
                _AtacarBoolean = false;
                break;
            case EnemyStates.NOQUEJAT:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        UpdateState(_CurrentState);

        MovimentCamera(); //Treure quan ho tingui Jugador
    }

    IEnumerator Patrullar()
    {
        Vector3 coord = Vector3.zero;
        while (!_Detectat)
        {
            if (!_Cami)
            {
           //     _Animacio.Play("Run");
                coord = _PuntsMapa[_Random.Next(0, _PuntsMapa.Length - 1)].transform.position;
                Debug.Log(coord);
                _NavMeshAgent.destination = new Vector3(coord.x, transform.position.y, coord.z);
                _Cami = true;
            }

            if (transform.position == new Vector3(coord.x, transform.position.y, coord.z))
            { 
             //   _Animacio.Play("Idle");
                _Cami = false;
            }

            _DetectarCollider = Physics.OverlapSphere(transform.position, 10f, _LayerJugador);

            if (_DetectarCollider.Length > 0)
            {
                ChangeState(EnemyStates.INVESTIGAR);
                _Detectat = true;
                _Cami = false;
            }

            yield return new WaitForSeconds(1);
        }
    }

    public void Escuchar(Vector3 pos, int nivellSo)
    {
            hits = Physics.RaycastAll(this.transform.position, pos - this.transform.position, Vector3.Distance(pos, this.transform.position));
            Debug.DrawLine(this.transform.position, pos - this.transform.position, Color.red, 10f);
            foreach (RaycastHit hit in hits)
            {
                Debug.Log(hit.collider.gameObject.name);
                if (hit.collider.TryGetComponent<IAtenuacio>(out IAtenuacio a))
                {
                    nivellSo = a.atenuarSo(nivellSo);
                }
            }
            if (nivellSo >= 2)
            {
            _NavMeshAgent.SetDestination(pos);
            }
            else if (nivellSo >= 1)
            {
                print("a");
                Vector3 r = new Vector3((float)UnityEngine.Random.Range(pos.x - 10, pos.x + 10), this.transform.position.y, UnityEngine.Random.Range(pos.z - 10, pos.z + 10));
            _NavMeshAgent.SetDestination(r);
            }
    }

    //Script de la càmera pel jugador
    public void MovimentCamera() //Treure quan ho tingui Jugador
    {
        Vector2 lookInput = _LookAction.ReadValue<Vector2>();

        _LookRotation.x += lookInput.x * _LookVelocity * Time.deltaTime;
        _LookRotation.y += (_InvertY ? 1 : -1) * lookInput.y * _LookVelocity * Time.deltaTime;

        _LookRotation.y = Mathf.Clamp(_LookRotation.y, -35, 35);
        _Camera.transform.localRotation = Quaternion.Euler(_LookRotation.y, _LookRotation.x, 0);
    }

}
