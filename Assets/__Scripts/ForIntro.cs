using UnityEngine;

public class PathFollowing : MonoBehaviour
{
    public Transform[] waypoints; // Массив точек пути
    public float speed = 5f; // Скорость движения

    private int currentWaypoint = 0;

    void Update()
    {
        // Если объект достиг конечной точки, перемещаемся к следующей
        if (currentWaypoint < waypoints.Length)
        {
            Vector3 targetPosition = waypoints[currentWaypoint].position;
            // Двигаем объект к целевой позиции
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            // Если объект достиг целевой позиции, переходим к следующей точке
            if (transform.position == targetPosition)
            {
                currentWaypoint++;
            }
        }
    }
}

