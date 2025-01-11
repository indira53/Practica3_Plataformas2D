using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    public bool moveX = true;
    public bool moveY = false;
    public float speed = 2f;
    public float xLimit;
    public float yLimit;
    public float pauseDuration = 1f;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private bool _movingToTarget = true;
    private float _pauseTimer = 0f;  

    // Start is called before the first frame update
    void Start()
    {
        //Guardamos la posición inicial
        _startPosition = transform.position;

        //Calculamos la posición objetivo inicial
        _targetPosition = new Vector3
            (
                moveX ? _startPosition.x + xLimit : _startPosition.x,
                moveY ? _startPosition.y + yLimit : _startPosition.y,
                _startPosition.z
            );
    }

    // Update is called once per frame
    void Update()
    {
        if (_pauseTimer > 0f)
        {
            _pauseTimer -= Time.deltaTime;
            return;
        }

        //Movimiento de la Plataforma
        MovePlatform();
    }

    private void MovePlatform()
    {
        //Calculamos la dirección del movimiento
        Vector3 target = _movingToTarget ? _targetPosition : _startPosition;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        //Verificamos si ha llegado al destino
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            //Cambiamos la dirección
            _movingToTarget = !_movingToTarget;
            _pauseTimer = pauseDuration;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            //Dibujamos las posiciones límites en el editor
            Vector3 startPos = transform.position;
            Vector3 targetPos = new Vector3
            (
                moveX ? startPos.x + xLimit : startPos.x,
                moveY ? startPos.y + yLimit : startPos.y,
                startPos.z
            );

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPos, targetPos);   
        }
    }
}
