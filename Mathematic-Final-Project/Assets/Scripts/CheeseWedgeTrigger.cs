using UnityEngine;

public class CheeseWedgeTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Range(0.1f, 20f)] public float Radius = 5.0f;
    [Range(1.0f, 20.0f)] public float Height = 1.0f;
    [Range(-1f, 1f)] public float ThresholdAngle = 0.8f;

    [Header("References")]
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _lookAt;

    private bool _isTriggered;
    private float _thresholdDeg;
    private Vector3 _targetOffset;

    private void Awake()
    {
        _thresholdDeg = Mathf.Acos(ThresholdAngle) * Mathf.Rad2Deg;
        _targetOffset = Vector3.up * Height * 0.5f;
    }

    private void Update()
    {
        if (!_target || !_lookAt) return;

        Vector3 toTarget = _target.position - transform.position;
        Vector3 lookDir = _lookAt.position - transform.position;

        _isTriggered = IsWithinTriggerArea(toTarget, lookDir);
    }

    private bool IsWithinTriggerArea(Vector3 toTarget, Vector3 lookDir)
    {
        // 1. Fast Distance Check
        if (toTarget.sqrMagnitude > Radius * Radius) return false;

        // 2. Height Check in Local Space
        Vector3 localTarget = transform.InverseTransformPoint(_target.position);
        if (Mathf.Abs(localTarget.y) > Height * 0.5f) return false;

        // 3. Angle Check
        float angle = Vector3.Angle(lookDir.normalized, toTarget.normalized);
        return angle <= _thresholdDeg;
    }

    private void OnDrawGizmosSelected()
    {
        if (!_target || !_lookAt) return;

        // Visualization code (optimized version of original)
        // ... [use Handles.matrix for proper rotations] ...
    }
}
