using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("General")]
    public bool playPhase;
    public List<GameObject> WorkerList = new List<GameObject>();

    [Header("Resources")]
    public int _cryptoI;
    public int _NFTI;
    [SerializeField] TextMeshProUGUI _cryptoTMP;
    [SerializeField] TextMeshProUGUI _NFTTMP;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _cryptoTMP.text = "Crypto Count: " + _cryptoI;
        _NFTTMP.text = "NFT Count: " + _NFTI;
    }
}
