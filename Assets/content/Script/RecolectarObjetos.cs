using UnityEngine;
using System.Collections;

public class RecolectarObjetos : MonoBehaviour
{
    private GameObject objetoCercano;
    private Inventario inventario;
    Animator playerAnim;

    void Start()
    {
        inventario = GetComponent<Inventario>();
        playerAnim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (objetoCercano != null && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(RecolectarConAnimacion());
        }
    }

    IEnumerator RecolectarConAnimacion()
    {
        playerAnim.SetTrigger("take");
        playerAnim.SetBool("takeYes", true);

        // Espera el tiempo de la animación (ajústalo según la duración real)
        yield return new WaitForSeconds(0.8f);

        if (objetoCercano != null)
        {
            if (objetoCercano.CompareTag("BalaItem"))
            {
                inventario.AgregarBalas(3);
                Debug.Log("Tienes 3 balas nuevas en el inventario");
            }
            else if (objetoCercano.CompareTag("Lata"))
            {
                inventario.AgregarLatas(1);
                Debug.Log("Tienes 1 lata nueva en el inventario");
            }

            Destroy(objetoCercano);
            objetoCercano = null;
        }

        playerAnim.SetBool("takeYes", false);
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
