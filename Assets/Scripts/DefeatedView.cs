using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatedView : MonoBehaviour
{
    public void OnDefeatedShown()
    {
        Time.timeScale = 0;
    }
}
