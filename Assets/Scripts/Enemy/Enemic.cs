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
    [SerializeField] private Collider _DetectarCollider;
    [SerializeField] private LayerMask _LayerJugador;
    [SerializeField] private GameObject _Jugador;

    private NavMeshAgent _NavMeshAgent;
    private Collider[] _Atacar;
    private System.Random _Random;
    private Animator _Animacio;
    private InputSystem_Actions _InputActions;
    private InputAction _MoveAction;
    private Vector3 _PuntSo; //Punt d'on prové el so, tant jugador com objecte
    private bool _InvestigarSo;

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
        _InvestigarSo = false;
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
                _InvestigarSo = true;
                StartCoroutine(Investigar());
                StartCoroutine(EsperarCanvi(10)); //Temps d'espera per canviar a patrulla (té posat un chage a patrulla)
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
                if (Physics.Raycast(transform.position, transform.forward, 10f, _LayerJugador))
                {
                    Debug.Log("Detecto alguna cosa aprop!");
                    _NavMeshAgent.destination = _Jugador.transform.position;
                    ChangeState(EnemyStates.PERSEGUIR);
                    StopAllCoroutines();
                }
                break;
            case EnemyStates.PERSEGUIR:
                if (Physics.Raycast(transform.position, transform.forward, 10f, _LayerJugador))
                {
                    Debug.Log("Detecto alguna cosa aprop!");
                    _NavMeshAgent.destination = _Jugador.transform.position;
                }
                else
                {
                    Debug.Log("No detecto res!");
                    _NavMeshAgent.destination = transform.position;
                    ChangeState(EnemyStates.INVESTIGAR);
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
                _InvestigarSo = false;
                break;
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
            yield return new WaitForSeconds(1);
        }
    }


    IEnumerator Investigar()
    {
        while(_InvestigarSo)
        {
            yield return new WaitForSeconds(1f);
            RandomPoint(_PuntSo, 2.5f, out _);
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
        _PuntSo = pos;
        RaycastHit[] hits = Physics.RaycastAll(this.transform.position, _PuntSo - this.transform.position, Vector3.Distance(_PuntSo, this.transform.position));
        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.TryGetComponent<IAtenuacio>(out IAtenuacio a))
            {
                nivellSo = a.atenuarSo(nivellSo);
            }
        }

        if (nivellSo == 1)
        {
            if(_CurrentState == EnemyStates.INVESTIGAR)
                _NavMeshAgent.SetDestination(_PuntSo);
            else if(_CurrentState == EnemyStates.PATRULLA)
            {
                RandomPoint(_PuntSo, 5f, out _);
                ChangeState(EnemyStates.INVESTIGAR);
            }
        }
    }

    public void RebreMal(float damage)
    {
        throw new NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position, 10f);
    }

    IEnumerator EsperarCanvi(int n)
    {
        yield return new WaitForSeconds(n);
        ChangeState(EnemyStates.PATRULLA);
    }
}
