using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyStick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image background, middle;
    private Vector2 posInput;
    static public float jHz { private set; get; }
    static public float jVt { private set; get; }
    private Vector2 iVec;
    private void Awake()
    {
        background = GetComponent<Image>();
        middle = transform.GetChild(0).GetComponent<Image>();
        iVec = background.rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background.rectTransform, eventData.position, eventData.pressEventCamera, out posInput))
        {
            float sizeX = background.rectTransform.sizeDelta.x / 2.5f;
            float jx = posInput.x / sizeX;
            float sizeY = background.rectTransform.sizeDelta.y / 2.5f;
            float jy = posInput.y / sizeY;
            Vector2 jVector = new Vector2(jx, jy);
            if (jVector.magnitude > 1f) jVector = jVector.normalized; 
            middle.rectTransform.anchoredPosition = new Vector2(jVector.x * sizeX, jVector.y * sizeY);
            if (Mathf.Abs(jVector.x) > 0.375f || Mathf.Abs(jVector.y) > 0.375f)
            {
                jHz = jVector.x;
                jVt = jVector.y;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        background.rectTransform.position = eventData.position;
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        background.rectTransform.anchoredPosition = iVec;
        middle.rectTransform.anchoredPosition = Vector2.zero;
        jHz = 0;
        jVt = 0;
    }

}
