using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Hero : MonoBehaviour
{
    static public Hero S { get; private set; }  // Singleton property

    [Header("Inscribed")]
    // These fields control the movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;

    [Header("Dynamic")]
    [Range(0, 4)]
    private float _shieldLevel = 1;

    [Tooltip("This field holds a reference to the last triggering GameObject")]
    private GameObject lastTriggerGo = null;

    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();                                // a
    // Create a WeaponFireDelegate event named fireEvent.
    public event WeaponFireDelegate fireEvent;

    void Awake()
    {
        if (S == null)
        {
            S = this; // Set the Singleton only if it’s null
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }

        fireEvent += TempFire;                                                // b
    }

    void Update()
    {
        // Pull in information from the Input class
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        // Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(vAxis * pitchMult, hAxis * rollMult, 0);

        // Use the fireEvent to fire Weapons when the Spacebar is pressed.
        if (Input.GetAxis("Jump") == 1 && fireEvent != null)                 // d
        {
            Debug.Log("Hero position when firing: " + transform.position);
            fireEvent();                                                      // e
        }
    }

    void TempFire()                                                           // f
    {
        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();

        // Get ProjectileHero component
        ProjectileHero proj = projGO.GetComponent<ProjectileHero>();          // h
        proj.type = eWeaponType.blaster;

        float tSpeed = Main.GET_WEAPON_DEFINITION(proj.type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        Debug.Log("Shield trigger hit by: " + go.gameObject.name);

        // Make sure it’s not the same triggering go as last time
        if (go == lastTriggerGo) return;
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            // If the shield was triggered by an enemy
            shieldLevel--;        // Decrease the level of the shield by 1
            Destroy(go);          // … and Destroy the enemy
        }
        else
        {
            Debug.LogWarning("Shield trigger hit by non-Enemy: " + go.name);
        }
    }

    public float shieldLevel
    {
        get { return (_shieldLevel); }
        private set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject);  // Destroy the Hero
                Main.HERO_DIED();
            }
        }
    }
}

// using UnityEngine;

// using System.Collections;
// using System.Collections.Generic;
// using System;

// public class Hero : MonoBehaviour
// {
//     static public Hero S { get; private set; }  // Singleton property    // a

//     [Header("Inscribed")]
//     // These fields control the movement of the ship
//     public float speed = 30;
//     public float rollMult = -45;
//     public float pitchMult = 30;
//     public GameObject projectilePrefab;
//     public float projectileSpeed = 40;

//     [Header("Dynamic")]
//     [Range(0, 4)]                                          // b
//     private float _shieldLevel = 1;
//     private GameObject lastTriggerGo = null;

//     void Awake()
//     {
//         if (S == null)
//         {
//             S = this; // Set the Singleton only if it�s null                  // c
//         }
//         else
//         {
//             Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
//         }
//     }

//     void Update()
//     {
//         // Pull in information from the Input class
//         float hAxis = Input.GetAxis("Horizontal");                            // d
//         float vAxis = Input.GetAxis("Vertical");                              // d

//         // Change transform.position based on the axes
//         Vector3 pos = transform.position;
//         pos.x += hAxis * speed * Time.deltaTime;
//         pos.y += vAxis * speed * Time.deltaTime;
//         transform.position = pos;

//         // Rotate the ship to make it feel more dynamic                       // e
//         transform.rotation = Quaternion.Euler(vAxis * pitchMult, hAxis * rollMult, 0);

//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             Debug.Log("Hero position when firing: " + transform.position);
//             TempFire();
//         }
//     }

//     void TempFire()
//     {                                                        // b
//         GameObject projGO = Instantiate<GameObject>(projectilePrefab);
//         projGO.transform.position = transform.position;
//         Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
//         rigidB.velocity = Vector3.up * projectileSpeed;
//     }

//     void OnTriggerEnter(Collider other)
//     {
//         Transform rootT = other.gameObject.transform.root;
//         GameObject go = rootT.gameObject;
//         Debug.Log("Shield trigger hit by: " + go.gameObject.name);          // b
//         print("Shield trigger hit by: " + go.gameObject.name);

//         // Make sure it’s not the same triggering go as last time
//         if (go == lastTriggerGo) return;                                    // c
//         lastTriggerGo = go;                                                   // d

//         Enemy enemy = go.GetComponent<Enemy>();                               // e
//         if (enemy != null)
//         {  // If the shield was triggered by an enemy
//             shieldLevel--;        // Decrease the level of the shield by 1
//             Destroy(go);          // … and Destroy the enemy                  // f
//         }
//         else
//         {
//             Debug.LogWarning("Shield trigger hit by non-Enemy: " + go.name);    // g
//         }
//     }

//     public float shieldLevel
//     {
//         get { return (_shieldLevel); }                                      // b
//         private set
//         {                                                         // c
//             _shieldLevel = Mathf.Min(value, 4);                             // d
//                                                                             // If the shield is going to be set to less than zero…
//             if (value < 0)
//             {                                                  // e
//                 Destroy(this.gameObject);  // Destroy the Hero
//                 Main.HERO_DIED();
//             }
//         }
//     }

//     // void Start() {�}  // Please delete the unused Start() method
// }