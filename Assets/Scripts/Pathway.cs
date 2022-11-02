using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathway : MonoBehaviour
{
    [SerializeField] private bool _refresh;
    private Transform[] _points;
    private LineRenderer _line;

    private void OnValidate()
    {
        Start();
    }

    private void Start()
    {
        //Get the child objects and draw the line based on their positions
        _points = GetComponentsInChildren<Transform>();
        _line = GetComponent<LineRenderer>();
        _line.positionCount = _points.Length - 1;
        for (int i = 0; i < _points.Length; i++)
        {
            if (i > 0)
            {
                _line.SetPosition(i - 1, _points[i].position);
            }
        }
    }

}
