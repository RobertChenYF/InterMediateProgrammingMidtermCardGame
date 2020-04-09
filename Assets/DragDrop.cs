
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{

    private Vector3 offset;
    //public float ScaleUp;

    void OnMouseDown()
    {

        offset = gameObject.transform.position -
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
        //gameObject.transform.localScale = new Vector3(transform.localScale.x * ScaleUp, transform.localScale.y * ScaleUp, 1);

    }

    void OnMouseDrag()
    {
        Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
        transform.position = Camera.main.ScreenToWorldPoint(newPosition) + offset;

    }

    void OnMouseUp()
    {
        //gameObject.transform.localScale = new Vector3(transform.localScale.x / ScaleUp, transform.localScale.y / ScaleUp, 1);


    }
}