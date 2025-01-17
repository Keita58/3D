using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

public class Enemic : MonoBehaviour, IDamageable
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

    private NavMeshAgent _NavMeshAgent;
    private Collider[] _Atacar;
    private System.Random _Random;
    private Animator _Animacio;
    private InputSystem_Actions _InputActions;
    private InputAction _MoveAction;

    private void Awake()
    {
        _InputActions = new InputSystem_Actions();
        _MoveAction = _InputActions.Player.Move;
        _Animacio = GetComponent<Animator>();
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
                _Animacio.Play("Run");
                _Detectat = false;
                StartCoroutine(Patrullar());
                break;
            case EnemyStates.INVESTIGAR:
                _Animacio.Play("Run");
                break;
            case EnemyStates.PERSEGUIR:
                _Animacio.Play("Run");
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
                    Debug.Log("No detecto res!");
                    _NavMeshAgent.destination = transform.position;
                    ChangeState(EnemyStates.PATRULLA);
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
    }

    IEnumerator Patrullar()
    {
        Vector3 coord = Vector3.zero;
        float range = 30.0f;
        while (!_Detectat)
        {
            if (!_Cami)
            {
                _Animacio.Play("Run");
                if (RandomPoint(transform.position, range, out coord))
                {
                    Debug.DrawRay(coord, Vector3.up, UnityEngine.Color.black, 1.0f);
                }
                Debug.Log(coord);
                _NavMeshAgent.destination = new Vector3(coord.x, transform.position.y, coord.z);
                _Cami = true;
            }

            if (transform.position == new Vector3(coord.x, transform.position.y, coord.z))
            { 
                _Animacio.Play("Idle");
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

    //Busca punt aleatori dins del NavMesh
    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            //Agafa un punt aleatori dins de l'esfera amb el radi que passem per paràmetre
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;

            //Comprovem que el punt que hem agafat està dins del NavMesh
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    public void Escuchar(Vector3 pos, int nivellSo)
    {
        RaycastHit[] hits = Physics.RaycastAll(this.transform.position, pos - this.transform.position, Vector3.Distance(pos, this.transform.position));
        //Debug.Log("Antes: " + nivellSo);
        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.TryGetComponent<IAtenuacio>(out IAtenuacio a))
            {
                nivellSo = a.atenuarSo(nivellSo);
            }
        }
        //Debug.Log("Despues: " + nivellSo);
        if (nivellSo == 1)
        {
            if(_CurrentState == EnemyStates.INVESTIGAR)
                _NavMeshAgent.SetDestination(pos);
            else if(_CurrentState == EnemyStates.PATRULLA)
                ChangeState(EnemyStates.INVESTIGAR);
        }
    }

    public void RebreMal(float damage)
    {
        throw new NotImplementedException();
    }
}
