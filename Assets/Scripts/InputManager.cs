using System.Collections;
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
    public void PlaceTower(Transform parent)
    {
        Debug.Log("Place Tower");
        GameObject prefab = Resources.Load($"Prefabs/TowersAndMobs/Tower{(parent.tag == "Tower" ? "Ranged" : "Blockade")}") as GameObject;
        TowerBase tower = Instantiate(prefab, parent).GetComponent<TowerBase>();
        int towerIndex = Random.Range(0, 3);
        tower.Initialise(Resources.Load($"Cards/Towers/Tower{(parent.tag == "Tower" ? towerIndex.ToString() : "Block0")}") as TowerCard);
    }
    public void SetMob(Transform parent)
    {
        Debug.Log("Set Mob");
        GameObject prefab = Resources.Load("Prefabs/TowersAndMobs/Mob") as GameObject;
        Mob mob = Instantiate(prefab, parent.position, Quaternion.identity).GetComponent<Mob>();
        int mobIndex = Random.Range(0, 3);
        mob.Initialise(Resources.Load("Cards/Mobs/Mob" + mobIndex.ToString()) as MobCard, parent.gameObject);

    }
    #endregion
    #region Input Actions
    public void Zoom(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _inputs.y = Mathf.Clamp(context.ReadValue<float>(), -1f, 1f);

        }
        if (context.canceled)
        {
            _inputs.y = 0f;
        }
    }
    public void ClickDrag(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _oldPosition = transform.position;
            if (context.control.path == "/Mouse/leftButton")
            {
                Ray _locationRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Plane _screenLocator = new Plane(Vector3.up, Vector3.zero);
                float _screenPoint;
                if (_screenLocator.Raycast(_locationRay, out _screenPoint))
                {
                    _dragStartPos = _locationRay.GetPoint(_screenPoint);
                }
            }
        }
        if (context.performed)
        {
            if (context.control.path == "/Mouse/leftButton")
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
        }
        if (context.canceled)
        {
            if (context.control.path == "/Mouse/leftButton" && _oldPosition == transform.position)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out _hitInfo))
                {
                    if (_hitInfo.transform.tag == "Start")
                    {
                        SetMob(_hitInfo.transform);
                    }
                    else if (_hitInfo.transform.tag == "Tower" || _hitInfo.transform.tag == "Path")
                    {
                        PlaceTower(_hitInfo.transform);
                    }
                }
            }
        }
    }
    #endregion
}
