using UnityEngine;
using System.Collections;

public class Artefacto : MonoBehaviour
{
    public GameObject objetoADestruir1;
    public GameObject objetoADestruir2;
    public Rigidbody objetoConRigidbody;
    public GameObject[] objetosAEncender;
    public Animator playerAnim;
    public GameObject player;
    public Inventario inventario;
    public string tagPlayer = "Player";

    private bool girando = true;
    private bool jugadorCerca = false;

    private GameObject objetoCercano;

    void Update()
    {
        if (girando)
        {
            transform.Rotate(Vector3.up * 100 * Time.deltaTime);
        }

        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            girando = false;

            if (objetoADestruir1) Destroy(objetoADestruir1);
            if (objetoADestruir2) Destroy(objetoADestruir2);

            if (objetoConRigidbody != null)
            {
                objetoConRigidbody.useGravity = true;
            }

            // Activar objetos
            foreach (GameObject obj in objetosAEncender)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

            Debug.Log("Robaste el artefacto. ahora debes escapar");

            StartCoroutine(RecolectarConAnimacion());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            jugadorCerca = true;
            objetoCercano = this.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            jugadorCerca = false;
            objetoCercano = null;
        }
    }

    IEnumerator RecolectarConAnimacion()
    {
        playerAnim.SetTrigger("take");
        playerAnim.SetBool("takeYes", true);

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

            
            objetoCercano = null;
        }

        playerAnim.SetBool("takeYes", false);
    }
}
