using Unity.VisualScripting;
using UnityEngine;

public class ObjectsScript : MonoBehaviour
{
    [SerializeField]
    Sound mySound;
    [SerializeField]
    PlayerComponent pc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<MeshFilter>().mesh = mySound.mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mySound.mesh;    
        this.transform.localScale = this.transform.localScale/2;
        this.AddComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        Lanzar();
    }
    void Lanzar()
    {
        this.GetComponent<Rigidbody>().AddForce(pc.transform.forward.x*100, 554, pc.transform.forward.z* 100);

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name != "Player") {
            Collider[] colliderHits = Physics.OverlapSphere(this.transform.position, 30);
            foreach (Collider collider in colliderHits)
            {
                Debug.Log("Enemic: "+collider.gameObject.name);
                if (collider.gameObject.TryGetComponent<Enemy>(out Enemy en))
                {
                    en.Escuchar(this.transform.position, mySound.intesitatSo);
                }
            }
        }
    }
}
