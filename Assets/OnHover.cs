using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnHoverClick();
        throw new NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEnter();
        throw new NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit();
        throw new NotImplementedException();
    }

    void OnHoverClick()
    {
        image.color = Color.blue;
    }

    void OnHoverEnter()
    {
        image.color = Color.gray;
    }

    void OnHoverExit()
    {
        image.color = Color.white;
    }
}
