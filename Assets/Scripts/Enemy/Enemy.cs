using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    Vector3 posToMove;
    NavMeshAgent agent;
    [SerializeField] RaycastHit[] hits;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //move();
    }
    public void Escuchar(Vector3 pos, int nivellSo)
    {

        hits = Physics.RaycastAll(this.transform.position, pos-this.transform.position, Vector3.Distance(pos, this.transform.position));
        Debug.Log("Antes: " + nivellSo);
        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.collider.gameObject.name);
            if(hit.collider.TryGetComponent<IAtenuacio>(out IAtenuacio a))
            {
                nivellSo = a.atenuarSo(nivellSo);
            }
        }
        Debug.Log("Despues: " + nivellSo);
        if (nivellSo == 1)
        {
            agent.SetDestination(pos);

        } 
    }
    public void move()
    {
        this.GetComponent<Rigidbody>().linearVelocity = posToMove;
    }
}
