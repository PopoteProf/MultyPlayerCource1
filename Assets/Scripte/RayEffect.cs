using UnityEngine;

public class RayEffect : MonoBehaviour
{
    [SerializeField] private float _fadeTimer;
    [SerializeField] private GameObject _PrefabsPS;
    private LineRenderer _lineRenderer;
    private Material _mat;
    private float _timer;

    public void SetUpEffect(Vector3 Start, Vector3 End, Color emissionColor) {
        
        _lineRenderer = GetComponent<LineRenderer>();
        _mat = _lineRenderer.material;

        _mat.SetColor("_EmissionColor", emissionColor);
        _mat.EnableKeyword("_EMISSION");
        
        _lineRenderer.positionCount = 3;
        _lineRenderer.SetPositions(new []{Start, Vector3.Lerp(Start, End, 0.5f), End});

        GameObject ps = Instantiate(_PrefabsPS, End, Quaternion.identity);
        ps.transform.forward = Start - End;
    }

    private void Update() {
        _timer += Time.deltaTime;
        _lineRenderer.endWidth =Mathf.Lerp(1, 0f, _timer /_fadeTimer);
        _lineRenderer.startWidth =Mathf.Lerp(1, 0f, _timer / _fadeTimer);
        if (_timer >= _fadeTimer)
        {
            Destroy(gameObject);
        }
    } 
}