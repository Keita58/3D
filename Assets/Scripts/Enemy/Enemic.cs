using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemic : MonoBehaviour, IDamageable
{
    private enum EnemyStates { PATRULLA, INVESTIGAR, PERSEGUIR, ATACAR, NOQUEJAT }
    [SerializeField] private EnemyStates _CurrentState;
    [SerializeField] private float _StateTime;
    [SerializeField] private bool _Detectat;
    [SerializeField] private bool _Cami;
    [SerializeField] private bool _AtacarBoolean;
    [SerializeField] private LayerMask _LayerJugador;
    [SerializeField] private LayerMask _LayerMask;
    [SerializeField] private GameObject _Jugador;

    private NavMeshAgent _NavMeshAgent;
    private Collider[] _Atacar;
    private System.Random _Random;
    private Animator _Animacio;
    private InputSystem_Actions _InputActions;
    private Vector3 _PuntSo; //Punt d'on prove el so, tant jugador com objecte
    private bool _InvestigarSo;
    private bool _ActivatEspera;
    private bool _Perseguir;

    private void Awake()
    {
        _InputActions = new InputSystem_Actions();
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
        _ActivatEspera = false;
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
                _Cami = false;
                StartCoroutine(Patrullar());
                break;
            case EnemyStates.INVESTIGAR:
                _Animacio.Play("Run");
                _InvestigarSo = true;
                StartCoroutine(Investigar());
                break;
            case EnemyStates.PERSEGUIR:
                _Animacio.Play("Run");
                break;
            case EnemyStates.ATACAR:
                break;
            case EnemyStates.NOQUEJAT:
                _Animacio.Play("Idle");
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
            case EnemyStates.INVESTIGAR:
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
                _InvestigarSo = false;
                _ActivatEspera = false;
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

                _NavMeshAgent.SetDestination(new Vector3(coord.x, transform.position.y, coord.z));
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

        if (jugador != null)
        {
            float angleVisio = Vector3.Angle(transform.forward, jugador.transform.position);

            if (angleVisio <= 120f)
            {
                //Raycast amb les layers de paret i player i si tenim la paret no seguim, sinó seguim el jugador
                
                if (Physics.Raycast(transform.position, _Jugador.transform.position, out RaycastHit info,_LayerMask))
                {
                    if(info.transform.gameObject.layer == _LayerJugador)
                    {
                        Debug.Log("Detecto alguna cosa aprop!");
                        StopAllCoroutines();
                        _Perseguir = true;
                        _NavMeshAgent.SetDestination(_Jugador.transform.position);
                        if (_CurrentState != EnemyStates.PERSEGUIR)
                            ChangeState(EnemyStates.PERSEGUIR);
                    }

                }
            }
            else if (_Perseguir)
            {
                StartCoroutine(AcabarPerseguir());
                _NavMeshAgent.SetDestination(_Jugador.transform.position);
            }
        }
        else
        {
            Debug.Log("No detecto res a la meva mirada!");
            if (_CurrentState == EnemyStates.PERSEGUIR)
                ChangeState(EnemyStates.INVESTIGAR);
        }

    }

    IEnumerator Investigar()
    {
        while (_InvestigarSo)
        {
            if (transform.position == _NavMeshAgent.destination)
            {
                if (!_ActivatEspera)
                {
                    _ActivatEspera = true;
                    StartCoroutine(EsperarCanvi()); //Temps d'espera per canviar a patrulla (te posat un chage a patrulla)
                }
                _Animacio.Play("Idle");
                yield return new WaitForSeconds(2.5f);
                _Animacio.Play("Run");
                RandomPoint(_PuntSo, 5f, out Vector3 hit);
                _NavMeshAgent.destination = hit;
            }
            else
                yield return new WaitForSeconds(1f);
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

            //Comprovem que el punt que hem agafat esta dins del NavMesh
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

            if (nivellSo >= 1)
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
        // Aquí posaríem el mal a l'enemic
        ChangeState(EnemyStates.NOQUEJAT);
        StartCoroutine(SurtNoqueig());
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 10f);
    }

    IEnumerator EsperarCanvi()
    {
        yield return new WaitForSeconds(10);
        _Animacio.Play("Idle");
        ChangeState(EnemyStates.PATRULLA);
    }

    IEnumerator AcabarPerseguir()
    {
        yield return new WaitForSeconds(2);
        _Perseguir = false;
    }

    IEnumerator SurtNoqueig()
    {
        yield return new WaitForSeconds(3);
        ChangeState(EnemyStates.PATRULLA);
    }
}