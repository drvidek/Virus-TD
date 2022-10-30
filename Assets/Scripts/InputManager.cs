using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Variables
    //A variable to store the layer mask of the object we are selecting so we can filter for objects we actually want to interact with
    private LayerMask _touchMask;
    //A list of objects we have touched
    private List<GameObject> _touches = new List<GameObject>();
    //Storage variable for object we have touched
    private RaycastHit _hitInfo;
    //Gameobject reference to the current game object we have touched so we can access its data
    private GameObject _selectedLocation;
    //Vector3 to store inputs from mouse, will use x and z for movement and y for zooming.
    private Vector3 _inputs;
    private Vector3 _dragStartPos;
    private Vector3 _dragCurrentPos;
    private Vector3 _newPosition;
    float _touchStartDist = 0f;
    float _touchCurrentDist = 0f;
    Vector3 _oldPosition;
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2.5f;
    #endregion
    #region Startup & Update
    private void Start()
    {
        _newPosition = transform.position;
    }
    private void Update()
    {
        Vector3 _moveDir = new Vector3(_newPosition.x, transform.position.y + _inputs.y * 2, _newPosition.z);
        transform.position = Vector3.Lerp(transform.position, _moveDir, _moveSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 2f, 10.4f), transform.position.z);
    }
    #endregion
    #region Build Phase
    public void PlaceTower()
    {
        Debug.Log("Place Tower");
    }
    public void SetMob()
    {
        Debug.Log("Set Mob");
    }
    #endregion
    #region Input Actions
    public void Zoom(InputAction.CallbackContext context)
    {
        #region Mouse
        if (context.control.path == "/Mouse/scroll/up")
        {
            if (context.started)
            {
                _inputs.y = Mathf.Clamp(context.ReadValue<float>(), -1f, 1f);
            }
        }
        #endregion
        #region TouchScreen
        else if (Input.touchCount == 2)
        {
            if (context.started)
            {
                _touchStartDist = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            }
            if (context.performed)
            {
                _touchCurrentDist = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                if (_touchCurrentDist < _touchStartDist)
                {
                    _inputs.y = 1f;
                }
                else if (_touchCurrentDist > _touchStartDist)
                {
                    _inputs.y = -1f;
                }
                else
                {
                    _inputs.y = 0f;
                }
            }
        }
        #endregion
        if (context.canceled)
        {
            _inputs.y = 0f;
        }
    }
    public void ClickDrag(InputAction.CallbackContext context)
    {
        #region Mouse
        if (context.control.path == "/Mouse/leftButton")
        {
            if (context.started)
            {
                _oldPosition = transform.position;

                Ray _locationRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Plane _screenLocator = new Plane(Vector3.up, Vector3.zero);
                float _screenPoint;
                if (_screenLocator.Raycast(_locationRay, out _screenPoint))
                {
                    _dragStartPos = _locationRay.GetPoint(_screenPoint);
                }
            }
            if (context.performed)
            {
                Ray _locationRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Plane _screenLocator = new Plane(Vector3.up, Vector3.zero);
                float _screenPoint;
                if (_screenLocator.Raycast(_locationRay, out _screenPoint))
                {
                    _dragCurrentPos = _locationRay.GetPoint(_screenPoint);
                    _newPosition = transform.position + _dragStartPos - _dragCurrentPos;
                }
            }

            if (context.canceled)
            {
                if (_oldPosition == transform.position)
                {

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out _hitInfo))
                    {
                        if (_hitInfo.transform.tag == "Start")
                        {
                            SetMob();
                        }
                        else if (_hitInfo.transform.tag == "Tower" || _hitInfo.transform.tag == "Path")
                        {
                            PlaceTower();
                        }
                    }

                }

            }
        }
        #endregion
        #region Touchscreen
        if (context.control.path == "/Touchscreen1/press" && Input.touchCount < 2)
        {
            if (context.started)
            {
                _oldPosition = transform.position;
                Ray _locationRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                Plane _screenLocator = new Plane(Vector3.up, Vector3.zero);
                float _screenPoint;
                if (_screenLocator.Raycast(_locationRay, out _screenPoint))
                {
                    _dragStartPos = _locationRay.GetPoint(_screenPoint);
                }

            }
            if (context.performed)
            {
                Ray _locationRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                Plane _screenLocator = new Plane(Vector3.up, Vector3.zero);
                float _screenPoint;
                if (_screenLocator.Raycast(_locationRay, out _screenPoint))
                {
                    _dragCurrentPos = _locationRay.GetPoint(_screenPoint);
                    _newPosition = transform.position + _dragStartPos - _dragCurrentPos;
                }

            }
            if (context.canceled)
            {
                if (_oldPosition == transform.position)
                {

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(0).position), out _hitInfo))
                    {
                        if (_hitInfo.transform.tag == "Start")
                        {
                            SetMob();
                        }
                        else if (_hitInfo.transform.tag == "Tower" || _hitInfo.transform.tag == "Path")
                        {
                            PlaceTower();
                        }
                    }

                }
            }
        }
        #endregion
    }
    #endregion
}


