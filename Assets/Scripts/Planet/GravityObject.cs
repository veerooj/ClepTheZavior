using System;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }
    public bool rotate;
    public bool shouldrotate;
    private DinoBasics dinoBasics;

    [Header("TEMPORAL")] [SerializeField] private float timertoStand;
    private float timer;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        dinoBasics = GetComponent<DinoBasics>();
    }

    private void Start()
    {
        if (PlanetGravity.instance != null)
            PlanetGravity.instance.Register(this);
    }

    private void OnEnable()
    {
        if (shouldrotate)
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
        if (timer < timertoStand && !rotate && shouldrotate)
        {
            timer += Time.deltaTime;
            return;
        }
        if (rotate && shouldrotate) return;
        timer = 0;
        rotate = true;
        OnStand();
    }

    public void OnStand()
    {
        if (dinoBasics != null)
        {
            dinoBasics.OnStanding();
        }
    }
}