using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyShield))]
public class Enemy_4 : Enemy
{        // Enemy_4 also extends the Enemy class

    private EnemyShield[] allShields;
    private EnemyShield thisShield;

    void Start()
    {
        allShields = GetComponentsInChildren<EnemyShield>();      // a
        thisShield = GetComponent<EnemyShield>();
    }

    public override void Move()
    {
        // You’ll add much more here shortly. For now, it’s easier to test if
        // Enemy_4 doesn’t move.
    }

    /// <summary>
    /// Enemy_4 Collisions are handled differently from other Enemy subclasses
    /// to enable protection by EnemyShields.
    /// </summary>
    /// <param name="coll"></param>
    void OnCollisionEnter(Collision coll)
    {                       // b

        GameObject otherGO = coll.gameObject;

        // Make sure this was hit by a ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {

            // Destroy the ProjectileHero regardless of bndCheck.isOnScreen
            Destroy(otherGO);                                     // c

            // Only damage this Enemy if it’s on screen
            if (bndCheck.isOnScreen)
            {

                // Find the GameObject of this Enemy_4 that was actually hit
                GameObject hitGO = coll.contacts[0].thisCollider.gameObject;   // d
                if (hitGO == otherGO)
                {                                        // e
                    hitGO = coll.contacts[0].otherCollider.gameObject;
                }

                // Get the damage amount from the Main WEAP_DICT
                float dmg = Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;

                // Find the EnemyShield that was hit (if there was one)
                bool shieldFound = false;
                foreach (EnemyShield es in allShields)
                {                       // f
                    if (es.gameObject == hitGO)
                    {
                        es.TakeDamage(dmg);
                        shieldFound = true;
                    }
                }

                if (!shieldFound) thisShield.TakeDamage(dmg);                  // g

                // If thisShield is still active, then it has not been destroyed
                if (thisShield.isActive) return;                               // h

                // This ship was destroyed so tell Main about it
                if (!calledShipDestroyed)
                {
                    Main.SHIP_DESTROYED(this);
                    calledShipDestroyed = true;
                }

                // Destroy this Enemy_4
                Destroy(gameObject);
            }

        }
        else
        {
            Debug.Log("Enemy_4 hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
