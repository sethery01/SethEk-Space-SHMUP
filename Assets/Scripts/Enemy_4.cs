using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyShield))]
public class Enemy_4 : Enemy
{
    [Header("Enemy_4 Inscribed Fields")]
    public float duration = 4;   // Duration of interpolation movement

    private EnemyShield[] allShields;
    private EnemyShield thisShield;

    // Movement interpolation points
    private Vector3 p0, p1;
    private float timeStart;

    void Start()
    {
        allShields = GetComponentsInChildren<EnemyShield>();
        thisShield = GetComponent<EnemyShield>();

        // Set initial movement points
        p0 = p1 = pos;
        InitMovement();
    }

    /// <summary>
    /// Sets new movement endpoints for Enemy_4
    /// </summary>
    void InitMovement()
    {
        p0 = p1;   // Old endpoint becomes new start

        // Assign a new random ***on-screen*** location
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;

        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        // Ensure the new point is in a different quadrant
        if (p0.x * p1.x > 0 && p0.y * p1.y > 0)
        {
            if (Mathf.Abs(p0.x) > Mathf.Abs(p0.y))
                p1.x *= -1;
            else
                p1.y *= -1;
        }

        timeStart = Time.time;
    }

    /// <summary>
    /// Movement for Enemy_4 uses interpolation, not inherited Enemy.Move()
    /// </summary>
    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        // Recalculate movement path when finished
        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        // Apply easing
        u = u - 0.15f * Mathf.Sin(u * 2 * Mathf.PI);

        // Interpolate between p0 and p1
        pos = (1 - u) * p0 + u * p1;
    }

    /// <summary>
    /// Collision handling for shields & damage distribution
    /// </summary>
    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;

        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {
            Destroy(otherGO);

            if (bndCheck.isOnScreen)
            {
                // Identify the hit object
                GameObject hitGO = coll.contacts[0].thisCollider.gameObject;
                if (hitGO == otherGO)
                    hitGO = coll.contacts[0].otherCollider.gameObject;

                // Damage amount
                float dmg = Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;

                // Check if a shield was hit
                bool shieldFound = false;
                foreach (EnemyShield es in allShields)
                {
                    if (es.gameObject == hitGO)
                    {
                        es.TakeDamage(dmg);
                        shieldFound = true;
                    }
                }

                // If no shield was hit, damage the main shield
                if (!shieldFound)
                    thisShield.TakeDamage(dmg);

                // If the shield is still active, enemy survives
                if (thisShield.isActive) return;

                // Otherwise notify Main and destroy
                if (!calledShipDestroyed)
                {
                    Main.SHIP_DESTROYED(this);
                    calledShipDestroyed = true;
                }

                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("Enemy_4 hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
