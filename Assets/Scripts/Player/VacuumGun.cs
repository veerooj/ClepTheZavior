
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class VacuumGun : MonoBehaviour
{
    [Header("References")]
    public Transform m_raycastSpot;
    public Transform m_suctionPoint;
    public AudioSource m_audioSource;

    [Header("Vacuum Settings")]
    public float m_suctionRadius = 8f;
    public float m_suctionForce = 30f;
    public float m_suctionMaxSpeed = 15f;
    public float m_collectDistance = 0.5f;
    public float m_suctionAngle = 45f;
    private Coroutine m_fireCoroutine;
    private bool doingSomething = false;
    [Header("Inventory")]
    public int m_maxCapacity = 10;

    [Header("Ejection")]
    public float m_ejectForce = 15f;

    [Header("Text")] 
    [SerializeField] private TextMeshProUGUI m_text;
    [Header("Detection")]
    public LayerMask m_suckableLayer;
    private bool m_isSucking;
    private List<Rigidbody> m_objectsBeingSucked = new List<Rigidbody>();
    private List<Rigidbody> m_storedObjects = new List<Rigidbody>();

    private void Awake()
    {
        if (m_audioSource == null)
            m_audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (m_raycastSpot == null && Camera.main != null)
            m_raycastSpot = Camera.main.transform;
        UIRefresh();
    }

    private void Update()
    {
        if (m_isSucking)
            Vacuum();
    }

    private void FixedUpdate()
    {
        if (m_isSucking)
            AttractObjects();
    }
    
    private void Vacuum()
    {
        if (m_suctionPoint == null || m_raycastSpot == null)
            return;

        int availableCapacity = m_maxCapacity - m_storedObjects.Count /*- m_objectsBeingSucked.Count*/;

        if (availableCapacity <= 0)
            return;
        Collider[] objects = Physics.OverlapSphere(m_suctionPoint.position, m_suctionRadius, m_suckableLayer);

        foreach (Collider col in objects)
        {
            if (availableCapacity <= 0) break;

            Rigidbody rb = col.attachedRigidbody;

            if (rb == null) continue;

            if (m_objectsBeingSucked.Contains(rb) || m_storedObjects.Contains(rb)) continue;

            if (!rb.gameObject.activeInHierarchy) continue;

            Vector3 direction = rb.position - m_raycastSpot.position;

            float angle = Vector3.Angle(m_raycastSpot.forward, direction);

            if (angle > m_suctionAngle / 2f)
                continue;
            
            m_objectsBeingSucked.Add(rb);

            availableCapacity--;
        }
    }
    private void AttractObjects()
    {
        if (m_suctionPoint == null || m_storedObjects.Count > m_maxCapacity)
            return;
        

        for (int i = m_objectsBeingSucked.Count - 1; i >= 0; i--)
        {
            Rigidbody rb = m_objectsBeingSucked[i];

            if (rb == null)
            {
                m_objectsBeingSucked.RemoveAt(i);
                continue;
            }

            Vector3 direction = m_suctionPoint.position - rb.position;

            float distance = direction.magnitude;

            if (distance <= m_collectDistance)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.gameObject.SetActive(false);

                m_objectsBeingSucked.RemoveAt(i);
                m_storedObjects.Add(rb);
                UIRefresh();
                continue;
            }

            rb.AddForce(direction.normalized * m_suctionForce, ForceMode.Acceleration);

            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, m_suctionMaxSpeed);
        }

        if (m_storedObjects.Count > m_maxCapacity)
        {
            StartCoroutine(WaitBetweenLaunch(m_maxCapacity));
        }
    }

    private void UIRefresh()
    {
        m_text.text = m_storedObjects.Count+"/"+ m_maxCapacity;
        if (m_storedObjects.Count == m_maxCapacity)
        {
            m_text.color = Color.crimson;
            m_text.text = "Full!";
        }

        if (m_storedObjects.Count > m_maxCapacity)
        {
            m_text.color = Color.red;
            m_text.text = "XOX";
        }
        else if (m_storedObjects.Count < m_maxCapacity && m_text.color != Color.lawnGreen)
        {
            m_text.color = Color.lawnGreen;
        }
    }
    
    private void Eject()
    {
        if (m_storedObjects.Count <= 0)
            return;

        if (m_suctionPoint == null ||
            m_raycastSpot == null)
            return;
        int index = m_storedObjects.Count - 1;
        Rigidbody rb = m_storedObjects[index];
        if (rb == null)
        {
            m_storedObjects.RemoveAt(index);
            return;
        }
        rb.gameObject.SetActive(true);
        rb.position = m_suctionPoint.position;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(m_raycastSpot.forward * m_ejectForce, ForceMode.Impulse);

        m_storedObjects.RemoveAt(index);
        UIRefresh();
    }
    
    private void OnSuckPerformed()
    {
        if (doingSomething) return;
        m_isSucking = true;
        doingSomething = true;
    }

    private void OnSuckCancelled()
    {
        m_isSucking = false;
        for (int i = m_objectsBeingSucked.Count - 1; i >= 0; i--)
        {
            m_objectsBeingSucked.RemoveAt(i);
        }
        doingSomething = false;

    }

    private void OnFirePerformed()
    {
        if (doingSomething) return;
        m_fireCoroutine = StartCoroutine(WaitBetweenLaunch(0));
        doingSomething = true;
    }

    private void OnFireCancelled()
    {
        doingSomething = false;
        if (m_fireCoroutine != null)
        {
            StopCoroutine(m_fireCoroutine);
            m_fireCoroutine = null;
        }
        doingSomething = false;
    }

    private void OnEnable()
    {
        InputManager.Instance.SuckPerformed += OnSuckPerformed;
        InputManager.Instance.SuckCancelled += OnSuckCancelled;
        InputManager.Instance.FirePerformed += OnFirePerformed;
        InputManager.Instance.FireCancelled += OnFireCancelled;
    }

    private void OnDisable()
    {
        InputManager.Instance.SuckPerformed -= OnSuckPerformed;
        InputManager.Instance.SuckCancelled -= OnSuckCancelled;
        InputManager.Instance.FirePerformed -= OnFirePerformed;
        InputManager.Instance.FireCancelled -= OnFireCancelled;
    }
    public int GetCurrentCapacity()
    {
        return m_storedObjects.Count + m_objectsBeingSucked.Count;
    }
    public int GetStoredObjects()
    {
        return m_storedObjects.Count;
    }

    public bool IsInventoryFull()
    {
        return GetCurrentCapacity() >= m_maxCapacity;
    }
    
    private void OnDrawGizmosSelected()
    {
            if (m_raycastSpot == null)
                return;

            Gizmos.color = Color.cyan;

            Vector3 origin = m_raycastSpot.position;
            Vector3 forward = m_raycastSpot.forward;

            float halfAngle = m_suctionAngle / 2f;

            Vector3 left = Quaternion.Euler(0, -halfAngle, 0)
                           * forward;

            Vector3 right = Quaternion.Euler(0, halfAngle, 0)
                            * forward;

            Gizmos.DrawRay(origin, left * m_suctionRadius);
            Gizmos.DrawRay(origin, right * m_suctionRadius);
            Gizmos.DrawWireSphere(origin, m_suctionRadius);
        
    }

    private IEnumerator WaitBetweenLaunch(int desiredInt)
    {
        if (desiredInt != 0)
        {
            yield return new WaitForSeconds(1);
        }
        while (m_storedObjects.Count > desiredInt)
        {
            Eject();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
