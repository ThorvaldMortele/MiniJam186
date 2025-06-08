using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplash : MonoBehaviour
{
    public GameObject SplashVFX;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            var vfx = Instantiate(SplashVFX, collision.transform.position, Quaternion.identity);
            Destroy(vfx, 1.5f);
        }
    }
}
