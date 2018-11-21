using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuHidingScript : MonoBehaviour, IPointerClickHandler
{

    public Animator animator;
    public void HideMenu()
    {
        animator.SetTrigger("Menu_Hide");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HideMenu();
    }
}
