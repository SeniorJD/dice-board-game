using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonScript : MonoBehaviour {

    public Animator animator;
	public void ShowMenu()
    {
        animator.SetTrigger("Menu_Show");
    }
}
