using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ScrollOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public bool Dragged = false;
    public void OnBeginDrag(PointerEventData data)
    {
        Debug.Log("START DRAG");
        Dragged = true;
    }
    
    public void OnEndDrag(PointerEventData data)
    {
        Debug.Log("END DRAG");
        Dragged = false;
    }

    public void OnDrag(PointerEventData data)
    {
        Dragged = true;
    }
}
