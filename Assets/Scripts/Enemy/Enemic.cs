using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    [SerializeField] private EnemyStates _BeforeState;
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
    private Vector3 _PuntSo; //Punt d'on prove el so, tant jugador com objecte
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
        _BeforeState = EnemyStates.PATRULLA;
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
        _BeforeState = _CurrentState;
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
                StartCoroutine(EsperarCanvi(10)); //Temps d'espera per canviar a patrulla (t� posat un chage a patrulla)
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
            case EnemyStates.PATRULLA:
                DetectarJugador();
                break;
            case EnemyStates.INVESTIGAR:
                DetectarJugador();
                break;
            case EnemyStates.PERSEGUIR:
                DetectarJugador();
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
                StopAllCoroutines();
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

    private void DetectarJugador()
    {
        Collider jugador = Physics.OverlapSphere(transform.position, 10f, _LayerJugador).FirstOrDefault();

        if(jugador != null)
        {
            float angleVisio = Vector3.Angle(transform.forward, jugador.transform.position);

            if(angleVisio <= 80f)
            {
                RaycastHit[] a = Physics.RaycastAll(transform.forward, jugador.transform.position, 10f);

                if(a != null)
                {
                    RaycastHit j = a.Where(x => x.collider.gameObject.layer.Equals(_LayerJugador)).FirstOrDefault();
                
                    if(!j.Equals(null))
                    {

                        bool paret = false;

                        foreach (RaycastHit r in a) 
                        {
                            if (r.distance < j.distance)
                            {
                                paret = true;
                                Debug.Log("Tinc una paret al davant!");
                                break;
                            }
                        }

                        if(!paret)
                        {
                            Debug.Log("Detecto alguna cosa aprop!");
                            StopAllCoroutines();
                            _NavMeshAgent.destination = _Jugador.transform.position;
                            if(_CurrentState != EnemyStates.PERSEGUIR)
                                ChangeState(EnemyStates.PERSEGUIR);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("No detecto res!");
                if(_CurrentState == EnemyStates.PERSEGUIR)
                    ChangeState(EnemyStates.INVESTIGAR);
            }
        }
    }

    IEnumerator Investigar()
    {
        while(_InvestigarSo)
        {
            _Animacio.Play("Idle");
            yield return new WaitForSeconds(5f);
            RandomPoint(_PuntSo, 5f, out Vector3 hit);
            _NavMeshAgent.destination = hit;
        }
    }

    //Busca punt aleatori dins del NavMesh
    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            //Agafa un punt aleatori dins de l'esfera amb el radi que passem per par�metre
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;

            //Comprovem que el punt que hem agafat est� dins del NavMesh
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
            if (hit.collider.TryGetComponent<IAtenuacio>(out IAtenuacio a))
            {
                nivellSo = a.atenuarSo(nivellSo);
            }

            /*
            if (nivellSo >= 2)
            {
                _NavMeshAgent.SetDestination(pos);
            }
            else if (nivellSo >= 1)
            {
                print("a");
                Vector3 r = new Vector3((float)UnityEngine.Random.Range(pos.x - 10, pos.x + 10), this.transform.position.y, UnityEngine.Random.Range(pos.z - 10, pos.z + 10));
                _NavMeshAgent.SetDestination(r);
            }*/

            if (nivellSo == 1)
            {
                if (_CurrentState == EnemyStates.INVESTIGAR)
                    _NavMeshAgent.SetDestination(_PuntSo);
                else if (_CurrentState == EnemyStates.PATRULLA)
                {
                    RandomPoint(_PuntSo, 5f, out _);
                    ChangeState(EnemyStates.INVESTIGAR);
                }
            }
        }        
    }

    public void RebreMal(float damage)
    {
        throw new NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 10f);
    }

    IEnumerator EsperarCanvi(int n)
    {
        yield return new WaitForSeconds(n);
        ChangeState(EnemyStates.PATRULLA);
    }
}
