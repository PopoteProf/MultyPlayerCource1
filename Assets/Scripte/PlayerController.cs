
using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private bool _IsPlaying;
    
    [SerializeField] private Transform _aimingPart;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private LayerMask _AimingMask;
    [SerializeField] private Transform _aimPoint;
    [SerializeField] private RayEffect _prefabsRayEffect;
    [SerializeField] private Image _imgReload;
    [SerializeField] private float _reloadTime = 4;
    [SerializeField] private GameObject _prefabsPSDeath;
    

    private CharacterController _cc;
    private Camera _camera;
    private float _timer;

    private bool CanFire
    {
        get { return _timer >= _reloadTime; }
    }

    public float timeToBoom = 5f;
    private bool OnBoomZone = false;
    private float _boomZoneTimer;
    

    void Start() {
        if (IsOwner)
        {
            _IsPlaying = true;
            NetWorkManagerPlayerData.Instance.VirtualCamera.Follow = transform;
            NetWorkManagerPlayerData.Instance.VirtualCamera.LookAt = transform;
        }
        
        _cc = GetComponent<CharacterController>();
        _camera = Camera.main;
    }

    void Update() {
        if (!_IsPlaying) return;
        ManageMovement();
        ManageAim();
        ManagerFire();
        ManageReload();
        if (OnBoomZone)
        {
            _boomZoneTimer += Time.deltaTime;
            if (_boomZoneTimer >= timeToBoom)
            {
                //Send BOOM to server
                Debug.Log("BOOM client side");
                BoomServerRpc();
                _boomZoneTimer = 0;
            }
        }
    }

    private void ManageReload() {
        if (_timer > _reloadTime) return; 
        _timer += Time.deltaTime;
        _imgReload.fillAmount = _timer /_reloadTime;

        if (_timer >= _reloadTime) {
            _imgReload.enabled = false;
            _timer = _reloadTime;
        }
    }

    private void ManagerFire()
    {
        if (Input.GetButtonDown("Fire1")&& CanFire)
        {
            RaycastHit hit;
            if (Physics.Raycast(_aimPoint.position, _aimPoint.forward, out hit)) {
                FireServerRpc(_aimPoint.position, hit.point);
                //RayEffect ray = Instantiate(_prefabsRayEffect, transform.position, quaternion.identity);
                //ray.SetUpEffect(_aimPoint.position, hit.point);
                _timer = 0;
                _imgReload.enabled = true;
                _imgReload.fillAmount = 0;

                if (hit.collider.transform.GetComponent<PlayerController>())
                {
                    OnPlayerKillServerRpc(OwnerClientId,
                        hit.collider.transform.GetComponent<PlayerController>().OwnerClientId);
                    hit.collider.transform.GetComponent<PlayerController>().Death();
                }
            }
        }
    }

    private void ManageAim() {
        RaycastHit hit;
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit, _AimingMask)) {
            Vector3 aimingVec = hit.point - _aimingPart.position;
            aimingVec.y = 0;
            _aimingPart.forward =aimingVec;
        }
    }

    private void ManageMovement() {
        Vector3 moveVec = new Vector3(-Input.GetAxisRaw("Horizontal"),0, -Input.GetAxisRaw("Vertical"));
        _cc.Move(moveVec.normalized * (_moveSpeed*Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("BoomZone"))
        {
            Debug.Log("Entering boom zone");
            OnBoomZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("BoomZone"))
        {
            Debug.Log("exiting boom zone");
            OnBoomZone = false;
            _boomZoneTimer = 0;
        }
    }

    public void Death() {
        DeathServerRpc();
        //Instantiate(_prefabsPSDeath, transform.position, Quaternion.identity);
        //Destroy(gameObject);
    }
    [ServerRpc]
    private void FireServerRpc(Vector3 start, Vector3 end) {
        FireClientRpc(start, end);
    }
    
    [ClientRpc]
    private void FireClientRpc(Vector3 start, Vector3 end) {
        RayEffect ray = Instantiate(_prefabsRayEffect, transform.position, quaternion.identity);
        ray.SetUpEffect(start, end);
    }
    
    [ServerRpc]
    private void BoomServerRpc() {
        //NetWorkManagerPlayerData.;
        NetWorkManagerPlayerData.Instance.Boom(this.OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeathServerRpc() {
        DeathClientRpc();
        NetworkObject.Despawn();
    }

    [ClientRpc]
    private void DeathClientRpc()
    {
        Instantiate(_prefabsPSDeath, transform.position, Quaternion.identity);
        //Destroy(gameObject);
    }

    [ServerRpc]
    private void OnPlayerKillServerRpc(ulong killerId, ulong deadId) {
        NetWorkManagerPlayerData.Instance.OnPlayerKill(killerId,deadId);
    }
}