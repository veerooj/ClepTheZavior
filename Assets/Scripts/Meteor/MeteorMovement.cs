using UnityEngine;
using UnityEngine.Events;

public class MeteorMovement : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector3 finalPosition;
    
    [Header("Time of Movement")]
    [SerializeField] private float duration = 5f;
    [SerializeField] private bool startOnEnable = true;
    
    [SerializeField] private Vector3 rotationSpeed = new Vector3(30f, 45f, 15f);
    
    public UnityEvent OnMeteorArrived;

    private float _timer;
    private bool _isMoving;
    
    private void OnEnable()
    {
        if (startOnEnable)
        {
            StartMeteor();
        }
    }
    
    public void StartMeteor()
    {
        _timer = 0f;
        transform.position = initialPosition;
        _isMoving = true;
    }
    
    private void Update()
    {
        if (!_isMoving) return;
        
        transform.Rotate(rotationSpeed * Time.deltaTime);

        _timer += Time.deltaTime;
        float progress = Mathf.Clamp01(_timer / duration);
        
        transform.position = Vector3.Lerp(initialPosition, finalPosition, progress);

        if (progress >= 1f)
        {
            _isMoving = false;
            OnMeteorArrived?.Invoke();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(initialPosition, 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(finalPosition, 0.5f);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(initialPosition, finalPosition);
    }
}
