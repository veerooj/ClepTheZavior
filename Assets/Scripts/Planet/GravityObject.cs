using System;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }
    public bool rotate;

    [Header("TEMPORAL")] [SerializeField] private float timertoStand;
    private float timer;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (PlanetGravity.instance != null)
            PlanetGravity.instance.Register(this);
    }

    private void OnEnable()
    {
        rotate = false;
        if (PlanetGravity.instance != null)
            PlanetGravity.instance.Register(this);
    }

    private void OnDisable()
    {
        if (PlanetGravity.instance != null)
            PlanetGravity.instance.Unregister(this);
    }

    private void Update()
    {
        if (timer < timertoStand && !rotate)
        {
            timer += Time.deltaTime;
            return;
        }
        timer = 0;
        rotate = true;
    }
}