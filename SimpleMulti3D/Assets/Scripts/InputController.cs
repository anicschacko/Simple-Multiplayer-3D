using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public float rotatingSpeed;
    public float releaseSpeed;

    public static Action<bool, float> UpdateYRotation;

    private RectTransform rectT;
    private Vector2 centerPoint;

    private float inputAngle = 0f;
    private float prevAngle = 0f;
    private bool isPointerDown = false;

    private void Awake()
    {
        rectT = this.gameObject.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!isPointerDown && !Mathf.Approximately(0f, inputAngle))
        {
            float deltaAngle = releaseSpeed * Time.deltaTime;
            if (Mathf.Abs(deltaAngle) > Mathf.Abs(inputAngle))
                inputAngle = 0f;
            else if (inputAngle > 0f)
                inputAngle -= deltaAngle;
            else
                inputAngle += deltaAngle;
        }
        UpdateYRotation?.Invoke(isPointerDown, inputAngle * Time.deltaTime);
        rectT.localEulerAngles = Vector3.back * inputAngle;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pointerPos = ((PointerEventData)eventData).position;
        float wheelNewAngle = Vector2.Angle(Vector2.up, pointerPos - centerPoint);

        if (Vector2.Distance(pointerPos, centerPoint) > 20f) //Checking if the pointer is too close to center
        {
            var speed = rotatingSpeed * Time.deltaTime;
            if (pointerPos.x > centerPoint.x)
                inputAngle += (wheelNewAngle - prevAngle) * speed;
            else
                inputAngle -= (wheelNewAngle - prevAngle) * speed;
        }

        prevAngle = wheelNewAngle;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var pointerPos = eventData.position;

        isPointerDown = true;
        centerPoint =
            RectTransformUtility.WorldToScreenPoint((eventData).pressEventCamera, rectT.position);
        prevAngle = Vector2.Angle(Vector2.up, pointerPos - centerPoint);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Performing one last drag event
        OnDrag(eventData);
        isPointerDown = false;
    }
}