
using UnityEngine;

public class SkillWeapon : MonoBehaviour
{
    public ParticleSystem[] ParticleSystems;
    public Transform targetTransform;
    public LayerMask targetLayer;
    public float FireRate = 0.15f;

    private bool _isButtonHold;
    private float _time;

    private void LateUpdate()
    {
        var ray = new Ray(transform.position, targetTransform.position);
        // Éú³É LayerMask
        LayerMask layerMask = 1 << targetLayer;
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            return;

        var lookDelta = hit.point - transform.position;
        var targetRot = Quaternion.LookRotation(lookDelta);
        transform.rotation = targetRot;

        if (Input.GetKeyDown(KeyCode.Z))
            _isButtonHold = true;
        else if (Input.GetKeyUp(KeyCode.Z))
            _isButtonHold = false;

        _time += Time.deltaTime;

        if (!_isButtonHold)
            return;

        if (_time < FireRate)
            return;

        foreach (var ps in ParticleSystems)
            ps.Emit(1);

        _time = 0;
    }
}
