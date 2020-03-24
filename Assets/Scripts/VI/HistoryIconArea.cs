using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryIconArea : MonoBehaviour
{
    private void Update()
    {
        if (transform.childCount > 13) {
            Destroy(transform.GetChild(0).gameObject);
        }
    }
}
