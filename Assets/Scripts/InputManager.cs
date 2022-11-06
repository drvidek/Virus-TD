using System.Collections.Generic; //For use of lists
using UnityEngine; //Connect to Unity Engine
using UnityEngine.InputSystem; //We are using new input system

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
    //Vector3 to store the start position of your touch for dragging
    private Vector3 _dragStartPos;
    //Vector3 to store the position your touch has moved to for dragging
    private Vector3 _dragCurrentPos;
    //Vector3 to store the position you want the camera to move to when dragging
    private Vector3 _newPosition;
    //Vector3 to store the starting position of camera when starting to drag
    Vector3 _oldPosition;
    //Float to store the distance between your 2 touch fingers when starting Zoom
    private float _touchStartDist = 0f;
    //Float to store the distance between your 2 touch fingers during zoom
    private float _touchCurrentDist = 0f;
    [Header("References")]
    [Tooltip("Add the PlayerManger instance from the scene")]
    [SerializeField] private PlayerManager _playerManager;
    [Tooltip("Add UI Manager from scene to this reference")]
    [SerializeField] private UIManager _uiManager;
    [Header("Movement")]
    [Tooltip("Set the movement speed we will use for the cameras movement")]
    [SerializeField] private float _moveSpeed = 2.5f;
    #endregion
    #region Startup & Update
    private void Start()
    {
        //If class references are null find them in scene
        if (_playerManager == null)
        {
            _playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        }
        if (_uiManager == null)
        {
            _uiManager = GameObject.Find("EventSystem").GetComponent<UIManager>();
        }
        //At start the new position to move to is the current position
        _newPosition = transform.position;
    }
    private void Update()
    {
        //Vector3 to store the new location to move to information from input actions. _newPosition comes from Click/Drag and _inputs.y comes from Zoom
        Vector3 _moveDir = new Vector3(_newPosition.x, transform.position.y + _inputs.y * 2, _newPosition.z);
        //Use Lerp to smoothly move to new location according to our move speed
        transform.position = Vector3.Lerp(transform.position, _moveDir, _moveSpeed * Time.deltaTime);
        //Adjust current location to clamp it inside acceptable movement area
        float yPos = Mathf.Clamp(transform.position.y, 2f, 10.4f);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -5.85f + (yPos / 2), 5.85f - (yPos / 2)), yPos, Mathf.Clamp(transform.position.z, -10.4f + yPos, 10.4f - yPos));
    }
    #endregion
    #region Input Actions
    public void Zoom(InputAction.CallbackContext context)
    {
        //Seperate out input from Mouse scroll wheel so we don't access Touch features without any touches
        #region Mouse
        if (context.control.path == "/Mouse/scroll/up")
        {
            //If we have started input make _inputs.y equal value passed through from mouse scroll clamped between -1 and 1. Mouse scroll registers one input as 120 by default for some reason
            if (context.started)
            {
                _inputs.y = Mathf.Clamp(context.ReadValue<float>(), -1f, 1f);
            }
        }
        #endregion
        //Else use our Touchscreen inputs
        #region TouchScreen
        //Only run function if we have 2 touches. This allows us to get data from second touches without errors if a touch was cancelled before function finishes
        else if (Input.touchCount > 1)
        {
            //If touch input has started store the current distance between the touches
            if (context.started)
            {
                _touchStartDist = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                Debug.Log(_touchStartDist);
            }
            //As touch input continues
            if (context.performed)
            {
                //Store current distance between the 2 touches
                _touchCurrentDist = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                Debug.Log(_touchCurrentDist);
                //If current distance is less than original distance apply 1 to our _inputs.y to zoom out and make our start distance the current distance
                if (_touchCurrentDist < _touchStartDist)
                {
                    Debug.Log("Zooming Out");
                    _inputs.y = 1f;
                    _touchStartDist = _touchCurrentDist;
                }
                //Else if current distance is more than original distance apply -1 to _inputs.y to zoom in and make our start distance our current distance
                else if (_touchCurrentDist > _touchStartDist)
                {
                    Debug.Log("Zooming In");
                    _inputs.y = -1f;
                    _touchStartDist = _touchCurrentDist;
                }
                //Else our touches haven't moved, apply nothing to _inputs.y
                else
                {
                    _inputs.y = 0f;
                }
            }
        }
        #endregion
        //If either input is cancelled _inputs.y = 0 to stop zooming
        if (context.canceled)
        {
            _inputs.y = 0f;
        }
    }
    public void ClickDrag(InputAction.CallbackContext context)
    {
        //Seperate out mouse button input to avoid errors when trying to access touch data without touches. This also allows us to use less if statements
        #region Mouse
        if (context.control.path == "/Mouse/leftButton")
        {
            //This if check throws a warning in Unity Console but doesn't seem to stop any function. Will try to find a better way when I have more time
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                //If we have started recieving inputs
                if (context.started)
                {
                    //Set our old position to our current position
                    _oldPosition = transform.position;
                    //Define a Ray to use to find current position of mouse click
                    Ray _locationRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                    //A plane to use as our area to determine location from
                    Plane _screenLocator = new Plane(Vector3.up, Vector3.zero);
                    //A float to store the location information recieved from our Ray
                    float _screenPoint;
                    //Perform the Raycast and store the location hit as our _dragStartPos
                    if (_screenLocator.Raycast(_locationRay, out _screenPoint))
                    {
                        _dragStartPos = _locationRay.GetPoint(_screenPoint);
                    }
                }
                //As input continues to be recieved
                if (context.performed)
                {
                    //Define a Ray to use to find the current position of mouse as it is dragged
                    Ray _locationRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                    //A plane to use as our area to determine location from
                    Plane _screenLocator = new Plane(Vector3.up, Vector3.zero);
                    //A float to store the location information recieved from our Ray
                    float _screenPoint;
                    //Perform the Raycast, store the location hit as our _dragStartPos and apply the difference between start and current position to our current camera position
                    if (_screenLocator.Raycast(_locationRay, out _screenPoint))
                    {
                        _dragCurrentPos = _locationRay.GetPoint(_screenPoint);
                        _newPosition = transform.position + _dragStartPos - _dragCurrentPos;
                    }
                }
                //If input from mouse has stopped
                if (context.canceled)
                {
                    CheckBuild(Mouse.current.position.ReadValue());
                    _newPosition = transform.position;
                }
            }
        }
        #endregion
        //Else use our Touchscreen inputs
        #region Touchscreen
        //Run the function if we have less than 2 touches
        if (Input.touchCount == 1)
        {
            //If we have started receiving inputs
            if (context.started)
            {
                //Old position equals the current camera transform position
                _oldPosition = transform.position;
                //Define a Ray at touch position to determine position in scene
                Ray _locationRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                //Plane to use for Ray to determine location
                Plane _screenLocator = new Plane(Vector3.up, Vector3.zero);
                //Float to store location data gathered from Ray
                float _screenPoint;
                //Cast the ray and store the location received as our drag start position
                if (_screenLocator.Raycast(_locationRay, out _screenPoint))
                {
                    _dragStartPos = _locationRay.GetPoint(_screenPoint);
                }

            }
            //If input is continuing
            if (context.performed)
            {
                //Define a Ray at touch position to get current position of touch in scene
                Ray _locationRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                //Plane to use for Ray to determine location
                Plane _screenLocator = new Plane(Vector3.up, Vector3.zero);
                //Float to store location data received from Ray
                float _screenPoint;
                //Cast the ray, store the location recieved as current drag position and apply the difference between the start and current drag position to our camera position
                if (_screenLocator.Raycast(_locationRay, out _screenPoint))
                {
                    _dragCurrentPos = _locationRay.GetPoint(_screenPoint);
                    _newPosition = transform.position + _dragStartPos - _dragCurrentPos;
                }

            }
            //If input has stopped
            if (context.canceled)
            {
                CheckBuild(Input.GetTouch(0).position);
            }
        }
        #endregion
    }
    #endregion
    #region Call Build Functions
    private void CheckBuild(Vector3 pos)
    {
        //If old position is equal to cameras current position we have not moved and we are obviously selecting something
        if (_oldPosition == transform.position)
        {
            //Cast a ray from touch location and store data on object with collider that is hit 
            if (Physics.Raycast(Camera.main.ScreenPointToRay(pos), out _hitInfo) && !_uiManager.purchasePanel.activeInHierarchy)
            {
                //If we select the path tiles and we have enough resources place a blockade tower and adjust resources to take away cost
                if (_hitInfo.transform.tag == "Path" && _playerManager.ResourceCount[0] >= _playerManager.blockTowerCost)
                {
                    _hitInfo.transform.GetComponent<BuildTower>().PlaceTowerFromPlayerInput(NetworkManager.GetPlayerIDNormalised(),0);
                    _playerManager.AdjustResources(0, -_playerManager.blockTowerCost);
                }
                //Else if we have selected start or tower tiles update the buttons with either tower or mob cards and open purchase panel
                else if (_hitInfo.transform.tag == "Start" || _hitInfo.transform.tag == "Tower")
                {
                    _uiManager.UpdateDisplay(_hitInfo);
                    _uiManager.purchasePanel.SetActive(true);
                }
            }
        }
    }
    #endregion
}