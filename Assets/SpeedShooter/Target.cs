using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float lifespan = 5.0f; // time before the target disappears automatically

    private void Start()
    {
        Destroy(gameObject, lifespan); // Automatically destroy the target after its lifespan
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet")) // Assuming you have a Bullet tag for your bullets
        {
            // Here, you can implement what happens when a target is hit
            // For example, increase score, play sound, etc.
            Destroy(gameObject); // Destroy the target when hit
        }
    }
}
