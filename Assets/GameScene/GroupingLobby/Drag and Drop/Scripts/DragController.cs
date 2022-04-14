// Source: https://github.com/dipen-apptrait/Vertical-drag-drop-listview-unity
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform currentTransform;
    private GameObject mainContent;
    private Vector3 currentPosition;
    public bool isVertical = true;
    public bool isHorizontal = true;

    private int totalChild;

    public void OnPointerDown(PointerEventData eventData)
    {
        currentPosition = currentTransform.position;
        mainContent = currentTransform.parent.gameObject;
        totalChild = mainContent.transform.childCount;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isVertical && isHorizontal) {
            currentTransform.position =
                        new Vector3(eventData.position.x, eventData.position.y, currentTransform.position.z);
        } else if (isHorizontal)
        {
            currentTransform.position =
                        new Vector3(eventData.position.x, currentTransform.position.y, currentTransform.position.z);
        }
        else if (isVertical)
        {
            currentTransform.position =
                        new Vector3(currentTransform.position.x, eventData.position.y, currentTransform.position.z);
        } else
        { //if neither vert or horiz, then no drag
            return;
        }


        for (int i = 0; i < totalChild; i++)
        {
            if (i != currentTransform.GetSiblingIndex())
            {
                Transform otherTransform = mainContent.transform.GetChild(i);
                int distance = (int) Vector3.Distance(currentTransform.position,
                    otherTransform.position);
                if (distance <= 10)
                {
                    if (isVertical && isHorizontal)
                    {
                        Vector3 otherTransformOldPosition = otherTransform.position;
                        otherTransform.position = new Vector3(currentPosition.x, currentPosition.y,
                            otherTransform.position.z);
                        currentTransform.position = new Vector3(otherTransformOldPosition.x, otherTransformOldPosition.y,
                            currentTransform.position.z);
                        currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                        currentPosition = currentTransform.position;
                    }
                    else if (isHorizontal)
                    {
                        Vector3 otherTransformOldPosition = otherTransform.position;
                        otherTransform.position = new Vector3(currentPosition.x, otherTransform.position.y,
                            otherTransform.position.z);
                        currentTransform.position = new Vector3(otherTransformOldPosition.x, currentTransform.position.y,
                            currentTransform.position.z);
                        currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                        currentPosition = currentTransform.position;
                    }
                    else //isVertical
                    {
                        Vector3 otherTransformOldPosition = otherTransform.position;
                        otherTransform.position = new Vector3(otherTransform.position.x, currentPosition.y,
                            otherTransform.position.z);
                        currentTransform.position = new Vector3(currentTransform.position.x, otherTransformOldPosition.y,
                            currentTransform.position.z);
                        currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                        currentPosition = currentTransform.position;
                    }
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentTransform.position = currentPosition;
    }
}