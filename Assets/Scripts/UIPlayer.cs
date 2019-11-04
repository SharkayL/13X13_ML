using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlayer : MonoBehaviour
{
    public PlayerState player;
    public Item item;
    Collider2D collider;
    public bool hover = false;

    // Start is called before the first frame update
    void Start()
    {
        collider = this.GetComponent<Collider2D>();
    }

    public bool IsDropped() {
        if (collider.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            return true;
        }
        return false;
    }
}
