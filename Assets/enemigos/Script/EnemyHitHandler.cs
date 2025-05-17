using UnityEngine;

public class EnemyHitHandler : MonoBehaviour
{
    public GameObject bloodImpactEffect;
    public GameObject bloodSprayEffect;
    public Transform modelTransform; // ¡Arrastra aquí el Transform del modelo en el Inspector!

    public void HandleHit(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (bloodImpactEffect != null && modelTransform != null)
        {
            GameObject impact = Instantiate(bloodImpactEffect, hitPoint, Quaternion.LookRotation(hitNormal));
            impact.transform.SetParent(modelTransform); // ¡Ahora es hijo del modelo!
            Destroy(impact, 2f);
        }
        else if (bloodImpactEffect != null && modelTransform == null)
        {
            Debug.LogWarning("¡Advertencia! 'modelTransform' no está asignado en el Inspector. El efecto de impacto podría no seguir al modelo.");
            GameObject impact = Instantiate(bloodImpactEffect, hitPoint, Quaternion.LookRotation(hitNormal));
            impact.transform.SetParent(transform); // Usar el transform del enemigo como respaldo
            Destroy(impact, 2f);
        }

        if (bloodSprayEffect != null && modelTransform != null)
        {
            GameObject spray = Instantiate(bloodSprayEffect, hitPoint, Quaternion.LookRotation(hitNormal));
            spray.transform.SetParent(modelTransform); // ¡También el chorrito es hijo del modelo!
            Destroy(spray, 3f);
        }
        else if (bloodSprayEffect != null && modelTransform == null)
        {
            Debug.LogWarning("¡Advertencia! 'modelTransform' no está asignado en el Inspector. El efecto de chorro podría no seguir al modelo.");
            GameObject spray = Instantiate(bloodSprayEffect, hitPoint, Quaternion.LookRotation(hitNormal));
            spray.transform.SetParent(transform); // Usar el transform del enemigo como respaldo
            Destroy(spray, 3f);
        }
    }
}