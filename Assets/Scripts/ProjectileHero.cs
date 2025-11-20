using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class ProjectileHero : MonoBehaviour
{
    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Dynamic")]
    public Rigidbody rigid;
    [SerializeField]                                                         // a
    private eWeaponType _type;                                               // b

    // This public property masks the private field _type
    public eWeaponType type
    {                                                // c
        get { return (_type); }
        set { SetType(value); }
    }

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();                                     // d
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offUp))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sets the _type private field and colors this projectile to match the 
    ///   WeaponDefinition.
    /// </summary>
    /// <param name="eType">The eWeaponType to use.</param>
    public void SetType(eWeaponType eType)
    {                                 // e
        Debug.Log("Projectile rend: " + rend);
        Debug.Log("Renderer on projectile prefab: " + GetComponent<Renderer>());
        _type = eType;
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(_type);
        rend.material.color = def.projectileColor;
    }

    /// <summary>
    /// Allows Weapon to easily set the velocity of this ProjectileHero
    /// </summary>
    public Vector3 vel
    {
        get { return rigid.velocity; }
        set { rigid.velocity = value; }
    }
}


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [RequireComponent(typeof(BoundsCheck))]
// public class ProjectileHero : MonoBehaviour
// {
//     private BoundsCheck bndCheck;

//     void Awake()
//     {
//         bndCheck = GetComponent<BoundsCheck>();
//     }

//     void Update()
//     {
//         if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offUp))
//         {          // a
//             Destroy(gameObject);
//         }
//     }

//     // void Start() {…}  // Please delete the unused Start() method
// }

