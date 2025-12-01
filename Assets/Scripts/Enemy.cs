using System.Collections;          // Required for some Arrays manipulation
using System.Collections.Generic;  // Required for Lists and Dictionaries
using UnityEngine;                 // Required for Unity

public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 10f;   // The movement speed is 10m/s
    public float fireRate = 0.3f;  // Seconds/shot (Unused)
    public float health = 10;    // Damage needed to destroy this enemy
    public int score = 100;   // Points earned for destroying this
    public float powerUpDropChance = 1f;

    protected bool calledShipDestroyed = false;
    protected BoundsCheck bndCheck;  // Reference to the BoundsCheck component
    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }

    // This is a Property: A method that acts like a field
    public Vector3 pos
    {                                                       // a
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }

    void Update()
    {
        Move();
        if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offDown))
        {
            Destroy(gameObject);
        }
        // Check whether this Enemy has gone off the bottom of the screen
        //if (!bndCheck.isOnScreen)
        //{                                         // d
        //    if (pos.y < bndCheck.camHeight - bndCheck.radius)
        //    {
        //        // We�re off the bottom, so destroy this GameObject
        //        Destroy(gameObject);
        //    }
        //}
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;

        // Check for collisions with ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {                                                    // b
                                                             // Only damage this Enemy if it’s on screen
            if (bndCheck.isOnScreen)
            {
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if (health <= 0)
                {
                    // Tell Main that this ship was destroyed                 // b
                    if (!calledShipDestroyed)
                    {
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED(this);
                    }                               
                    Destroy(this.gameObject);
                }
            }
            // Destroy the ProjectileHero regardless
            Destroy(otherGO);                                               // e
        }
        else
        {
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);      // f
        }
    }

    // void OnCollisionEnter(Collision coll)
    // {
    //     GameObject otherGO = coll.gameObject;                                  // a
    //     if (otherGO.GetComponent<ProjectileHero>() != null)
    //     {                // b
    //         Destroy(otherGO);      // Destroy the Projectile
    //         Destroy(gameObject);   // Destroy this Enemy GameObject 
    //     }
    //     else
    //     {
    //         Debug.Log("Enemy hit by non-ProjectileHero: " + otherGO.name);  // c
    //     }
    // }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    // void Start() {�}  // Please delete the unused Start() method
}