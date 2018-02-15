using UnityEngine;
using System.Collections;

public class TimedDestroy : MonoBehaviour
{
    public float DestroyTime = 0f;
    public Component DestroyComp = null;

    // Update is called once per frame
    void Update()
    {
        DestroyTime -= Time.deltaTime;

        if (DestroyTime <= 0f)
        {
            if (DestroyComp != null)
            {
                Destroy(DestroyComp);
                Destroy(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
