using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnStart : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, .25f);
    }
}
