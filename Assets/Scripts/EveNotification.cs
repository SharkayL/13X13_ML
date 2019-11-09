using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EveNotification : MonoBehaviour
{
    public MatureManager manager;
    float t = 0;
    float fadeOutStart = 2;
    float completionTime = 3;
    SpriteRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t > completionTime)
        {
            Destroy(this.gameObject);
        }
        else if(t > fadeOutStart)
        {
            float completion = 1-(t-fadeOutStart) / (completionTime-fadeOutStart);
            renderer.color = new Vector4(1, 1, 1, 1 * completion);
        }
    }
}
