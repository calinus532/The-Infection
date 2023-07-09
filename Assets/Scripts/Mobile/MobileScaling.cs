using UnityEngine;
using UnityEngine.UI;

public class MobileScaling : MonoBehaviour
{
    private CanvasScaler scaler;
    [SerializeField] private float scale;
    private void Awake()
    {
        if (!Application.isMobilePlatform) return;
        scaler = GetComponent<CanvasScaler>();
        scaler.referenceResolution = new Vector2(scale, scaler.referenceResolution.y);
    }
    private void Update()
    {
        if (!Application.isMobilePlatform) return;
        scaler = GetComponent<CanvasScaler>();
        scaler.referenceResolution = new Vector2(scale, scaler.referenceResolution.y);
    }
}
