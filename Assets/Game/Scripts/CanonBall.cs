using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBall : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionVFX;
    [SerializeField]
    private int damage = 10;
    private float damageMultiplier = 1;
    private void OnTriggerEnter(Collider other)
    {
        GameObject.Instantiate(explosionVFX,transform.position, Quaternion.identity);
        if (other.gameObject.TryGetComponent(out IDamageable damageableObject))
        {
            damageableObject.GetDamage(Mathf.RoundToInt(damage*damageMultiplier));
        }
        Destroy(gameObject);
    }

    public void SetDamageMultiplier(float damageMultiplier)
    {
        this.damageMultiplier = damageMultiplier;
    }
}
