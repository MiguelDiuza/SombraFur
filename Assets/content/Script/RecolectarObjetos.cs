using UnityEngine;

public class RecolectarObjetos : MonoBehaviour
{
    private GameObject objetoCercano;
    private Inventario inventario;

    void Start()
    {
        inventario = GetComponent<Inventario>();
    }

    void Update()
    {
        if (objetoCercano != null && Input.GetKeyDown(KeyCode.E))
        {
            if (objetoCercano.CompareTag("BalaItem"))
            {
                inventario.AgregarBalas(3);
            }
            else if (objetoCercano.CompareTag("Lata"))
            {
                inventario.AgregarLatas(1);
            }

            Destroy(objetoCercano);
            objetoCercano = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BalaItem") || other.CompareTag("Lata"))
        {
            objetoCercano = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == objetoCercano)
        {
            objetoCercano = null;
        }
    }
}
