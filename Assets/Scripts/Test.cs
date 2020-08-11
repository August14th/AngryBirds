using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : GameBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            NewGameObject<GameBehaviour>("Loading", gameObject);
        }
    }
}