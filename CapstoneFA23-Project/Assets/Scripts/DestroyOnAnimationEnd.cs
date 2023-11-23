using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAnimationEnd : MonoBehaviour
{
    public void DestroyParent()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void DeactivateSelf()
    {
        gameObject.SetActive(false);
    }

    public void DeactivateParent()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
