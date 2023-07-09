using UnityEngine;

public class MobileControls : MonoBehaviour
{
    private void Awake()
    {
        if (!Application.isMobilePlatform)
        {
            gameObject.SetActive(false);
        }
    }
}
