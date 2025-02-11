using Unity.VisualScripting;
using UnityEngine;

public class ObjectsScript : MonoBehaviour
{
    [SerializeField]
    Sound mySound;
    [SerializeField] public GameObject camaraPrimera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //this.GetComponent<MeshFilter>().mesh = mySound.mesh;
        //this.GetComponent<MeshCollider>().sharedMesh = mySound.mesh;    
        this.transform.localScale = this.transform.localScale/2;
        this.AddComponent<CapsuleCollider>();
    }
    bool lanzado = false;
    public void Lanzar()
    {
        if (!lanzado)
        {
            float u = 5 / this.GetComponent<Rigidbody>().mass;
            float t = 2 * u / Physics.gravity.magnitude;
            Vector3 AB = (camaraPrimera.transform.forward + camaraPrimera.transform.forward * 4) - camaraPrimera.transform.forward;
            Vector3 h = AB / t;
            Vector3 H = h * this.GetComponent<Rigidbody>().mass;
            Vector3 F = H + 8 * Vector3.up;
            this.GetComponent<Rigidbody>().AddForce(F, ForceMode.Impulse);
            lanzado = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name != "Player" && lanzado) {
            lanzado=false;
            Collider[] colliderHits = Physics.OverlapSphere(this.transform.position, 30);
            foreach (Collider collider in colliderHits)
            {
                Debug.Log("Enemic: "+collider.gameObject.name);
                if (collider.gameObject.TryGetComponent<Enemic>(out Enemic en))
                {
                    en.Escuchar(this.transform.position, mySound.intesitatSo);
                }
            }
        }
    }

}
