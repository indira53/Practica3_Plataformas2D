using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCamera : MonoBehaviour
{
    public Transform playerTransform;
    private float velocidadCamara = 15f;
    public bool suavizado = true;
    private Vector3 nuevaPosicion;
    public float offset;

    // Update is called once per frame
    void Update()
    {
        nuevaPosicion = this.transform.position;
        nuevaPosicion.x = playerTransform.transform.position.x;
        nuevaPosicion.y = playerTransform.transform.position.y + offset;
        if (suavizado)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, nuevaPosicion, velocidadCamara * Time.deltaTime);
        }
    }
}
