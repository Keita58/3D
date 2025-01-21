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
        Debug.DrawLine(this.transform.position, pos-this.transform.position, Color.red, 10f);
        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.collider.gameObject.name);
            if(hit.collider.TryGetComponent<IAtenuacio>(out IAtenuacio a))
            {
                nivellSo = a.atenuarSo(nivellSo);
            }
        }
        if(nivellSo >= 2)
        {
            agent.SetDestination(pos);
        }
        else if (nivellSo >= 1)
        {
            print("a");
            Vector3 r = new Vector3((float)UnityEngine.Random.Range(pos.x - 10, pos.x + 10), this.transform.position.y, UnityEngine.Random.Range(pos.z - 10, pos.z + 10));
            agent.SetDestination(r);
        } 
    }
    public void move()
    {
        this.GetComponent<Rigidbody>().linearVelocity = posToMove;
    }
}
