using UnityEngine;

public class PathFollowing : MonoBehaviour
{
    public Transform[] waypoints; // ������ ����� ����
    public float speed = 5f; // �������� ��������

    private int currentWaypoint = 0;

    void Update()
    {
        // ���� ������ ������ �������� �����, ������������ � ���������
        if (currentWaypoint < waypoints.Length)
        {
            Vector3 targetPosition = waypoints[currentWaypoint].position;
            // ������� ������ � ������� �������
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            // ���� ������ ������ ������� �������, ��������� � ��������� �����
            if (transform.position == targetPosition)
            {
                currentWaypoint++;
            }
        }
    }
}

