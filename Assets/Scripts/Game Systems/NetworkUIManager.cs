using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _messages;

    private static NetworkUIManager _networkUIManagerInstance = null;
    public static NetworkUIManager NetworkUIManagerInstance
    {
        //Property Read is the instance, public by default
        get => _networkUIManagerInstance;
        //private means only this instance of the class can access set
        private set
        {
            //set the instance to the value if the instance is null
            if (_networkUIManagerInstance == null)
            {
                _networkUIManagerInstance = value;
            }
            //if it is not null, check if the value is stored as the static instance
            else if (_networkUIManagerInstance != value)
            {
                //if not, throw a warning and destroy that instance

                //$ is to identify the string as containing an interpolated value
                Debug.LogWarning($"{nameof(NetworkUIManager)} instance already exists, destroy duplicate!");
                Destroy(value.gameObject);
            }
        }
    }

    private void Awake()
    {
        _networkUIManagerInstance = this;
    }

    public void ShowMessageFailedConnect()
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }

    public void ShowMessageDisconnected()
    {
        transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
    }
}
