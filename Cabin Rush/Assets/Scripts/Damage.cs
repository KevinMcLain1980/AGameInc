using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum DamageType
    {
       moving, stationary,DOT,Homing,
    }
    [SerializeField] DamageType damageType;
    [SerializeField] Rigidbody rb;
    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    bool isDamaging;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(damageType == DamageType.moving || damageType == DamageType.Homing)
        {
            Destroy(gameObject, destroyTime);
            if(damageType == DamageType.moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(damageType == DamageType.Homing)
        {
            rb.linearVelocity = (GameManager.instance.Player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger) { return; }

        IDamage dmg = other.GetComponent<IDamage>();

        
        if(dmg != null && damageType != DamageType.DOT)
        {
          dmg.takeDamage(damageAmount);

        }
        if(damageType == DamageType.moving || damageType == DamageType.Homing)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) { return; }
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && damageType == DamageType.DOT)
        {
           StartCoroutine(damageOther(dmg));
        }
    }
    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
       
    }
}
