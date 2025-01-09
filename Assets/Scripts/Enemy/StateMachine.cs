using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

public class StateMachine : MonoBehaviour
{
    private enum EnemyStates { PATROL, DETECT, ATTACK, IDLE }
    [SerializeField] private EnemyStates _CurrentState;
    [SerializeField] private float _StateTime;
    [SerializeField] private bool _Detectat;
    [SerializeField] private bool _Cami;
    [SerializeField] private bool _AtacarBoolean;
    [SerializeField] private Collider[] _DetectarCollider;
    [SerializeField] private LayerMask _LayerJugador;
    [SerializeField] private GameObject _Jugador;

    private NavMeshAgent _NavMeshAgent;
    private Collider[] _Atacar;

    private void Awake()
    {
        
    }

    private void ChangeState(EnemyStates newState)
    {
        //tornar al mateix estat o no
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
            case EnemyStates.PATROL:
                //_Animacio.Play("Run");
                _Detectat = false;
                StartCoroutine(Patrullar());
                break;
            case EnemyStates.DETECT:
                //_Animacio.Play("Run");
                break;
            case EnemyStates.ATTACK:
                break;
            case EnemyStates.IDLE:
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
            case EnemyStates.PATROL:
                break;
            case EnemyStates.DETECT:
                _DetectarCollider = Physics.OverlapSphere(transform.position, 10f, _LayerJugador);
                _Atacar = Physics.OverlapSphere(transform.position, 5f, _LayerJugador);

                if (_Atacar.Length > 0)
                {
                    ChangeState(EnemyStates.ATTACK);
                }
                else
                {
                    Debug.Log("Detecto!");
                    if (_DetectarCollider.Length > 0)
                    {
                        Debug.Log("Detecto alguna cosa aprop!");
                        _NavMeshAgent.destination = _Jugador.transform.position;
                    }
                    else
                    {
                        Debug.Log("No detecto res!");
                        _NavMeshAgent.destination = transform.position;
                    }
                }
                break;
            case EnemyStates.ATTACK:
                break;
            case EnemyStates.IDLE:
                break;
            default:
                break;
        }
    }

    private void ExitState(EnemyStates exitState)
    {
        switch (exitState)
        {
            case EnemyStates.PATROL:
            case EnemyStates.DETECT:
            case EnemyStates.ATTACK:
                _AtacarBoolean = false;
                break;
            case EnemyStates.IDLE:
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
        KeyValuePair<float, float> coordXZ = new KeyValuePair<float, float>(0, 0);
        while (!_Detectat)
        {
            if (!_Cami)
            {
                //_Animacio.Play("Run");
                //coordXZ = (KeyValuePair<float, float>)_PuntsMapa[_Random.Next(0, _PuntsMapa.Count - 1)];
                _NavMeshAgent.destination = new Vector3(coordXZ.Key, transform.position.y, coordXZ.Value);
                _Cami = true;
            }

            if (transform.position == new Vector3(coordXZ.Key, transform.position.y, coordXZ.Value))
            {
                //_Animacio.Play("Idle");
                _Cami = false;
            }

            _DetectarCollider = Physics.OverlapSphere(transform.position, 10f, _LayerJugador);

            if (_DetectarCollider.Length > 0 && _DetectarCollider[0].transform.tag.Equals("Player"))
            {
                ChangeState(EnemyStates.DETECT);
                _Detectat = true;
                _Cami = false;
            }

            yield return new WaitForSeconds(1);
        }
    }
}
