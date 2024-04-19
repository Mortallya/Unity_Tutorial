using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintActivator : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSprinting sprinting = other.GetComponent<PlayerSprinting>();
            if (sprinting != null)
            {
                sprinting.enabled = true;
                Destroy(gameObject);
            }
        }
    }
}
