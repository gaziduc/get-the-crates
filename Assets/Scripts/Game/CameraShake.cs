using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    private Camera camera;
    private float shakeAmount = 0f;
    private Vector3 initialCamPos;
    
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        initialCamPos = camera.transform.position;
    }

    void Update()
    {
        if (shakeAmount == 0f && Vector3.Distance(camera.transform.position, initialCamPos) >= 0.0001f)
        {
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, initialCamPos, Time.deltaTime * 0.5f);
        }
    }

    public void Shake(float amount, float length)
    {
        shakeAmount = amount;
        InvokeRepeating("BeginShake", 0f, 0.005f);
        Invoke("StopShake", length);
    }

    void BeginShake()
    {
        if (shakeAmount > 0)
        {
            Vector3 camPos = camera.transform.position;
            
            float shakeAmountX = Random.value * shakeAmount * 2 - shakeAmount;
            float shakeAmountY = Random.value * shakeAmount * 2 - shakeAmount;

            camPos.x += shakeAmountX;
            camPos.y += shakeAmountY;

            camera.transform.position = camPos;
        }
    }

    void StopShake()
    {
        CancelInvoke("BeginShake");
        shakeAmount = 0f;
    }
}
