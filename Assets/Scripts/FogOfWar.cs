using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    Material _material;
    private float _current;
    private float _target;
    private Vector2 _offset;
    [SerializeField] private float _dissolveSpd, _moveSpd;

    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponentInChildren<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        _offset += new Vector2(_moveSpd * Time.deltaTime, _moveSpd * Time.deltaTime);
        _material.mainTextureOffset = _offset;

        if (_current == _target)
            return;

        _current = Mathf.MoveTowards(_current, _target, _dissolveSpd * Time.deltaTime);
        _material.SetFloat("_DissolveAmount", _current);
    }

    public void SetTargetDissolve(float target)
    {
        _target = target;
    }
}
