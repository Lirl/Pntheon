using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void DiskReleaseHandler();

public class Disk : Photon.PunBehaviour {
    private Vector3 originalPosition;
    private Rigidbody _rigidbody;
    internal bool startedMoving = false;
    private bool isMouseDown = false;
    private bool zoomOut = false;
    public Rigidbody Rigidbody;
    public SpringJoint SpringJoint;
    public float releaseTime = 0.15f;
    public float cameraAdjuster;
    public float endTurn;
    public float HeightOffSet;

    public int isDamaged = 0;

    public List<DiskReleaseHandler> OnDiskRelease = new List<DiskReleaseHandler>();

    public LineRenderer line;
    public SpringJoint SJ;
    public MeshRenderer mesh;

    private GameObject hook;

    public int Alliance;

    public double Health;
    public double TotalHealth;
    public float Range = 30.0f;

    public int Attack = 1;
    public int Id = -1;
    public enum ClassType { Rock, Paper, Scissors };
    public ClassType classType;

    public bool Enable = false; // when disabled, block any mouse interaction with this game object

    private static int _idCounter = 0;
    private float enlarge = 0;
    private float _normalScaleX;
    private float _normalScaleZ;
    private float _enlargeScaleX;
    private float _enlargeScaleZ;
    private float _shrinkScaleX;
    private float _shrinkScaleZ;

    public bool _released = false;
    private bool inField = false;
    private bool outOfBounds = false;

    public bool _forcedRelease = false;
    public int Code;


    public static int GenerateId() {
        _idCounter++;
        return _idCounter;
    }

    private void Awake() {
        line = GetComponent<LineRenderer>();
        SJ = GetComponent<SpringJoint>();
        line.enabled = false;
    }

    public void Init(int alliance) {
        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            photonView.RPC("PunInit", PhotonTargets.All, alliance);
        } else {
            PunInit(alliance);
        }
    }

    [PunRPC]
    public void PunInit(int alliance) {
        if (alliance == 1) {
            hook = GameObject.Find("HostHook");
        } else {
            hook = GameObject.Find("ClientHook");
        }

        GetComponent<SpringJoint>().connectedAnchor = hook.transform.position;
        GetComponent<SpringJoint>().connectedBody = hook.GetComponent<Rigidbody>();

        _normalScaleX = gameObject.transform.localScale.x;
        _normalScaleZ = gameObject.transform.localScale.z;
        originalPosition = transform.position;

        _rigidbody = GetComponent<Rigidbody>();

        if (!Board.Instance.isYourTurn) {
            Destroy(GetComponent<SpringJoint>());
        }

        if (!Board.Instance.isHost && !Board.Instance.isTutorial) {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        Alliance = alliance;

        // Set Disk properties according to rank
        if (User.instance) {
            for (int i = 1; i < User.instance.cardLevels[Code]; i++) {
                Attack = (int)(Mathf.Floor(Attack * 1.11f));
                TotalHealth = (int)(TotalHealth * 1.11f);
                Health = TotalHealth;
            }
        }
        
        // Set disk color
        GetComponent<MeshRenderer>().material = Resources.Load("Materials/Color" + alliance, typeof(Material)) as Material;

        mesh = SJ.connectedBody.GetComponent<MeshRenderer>();
        line.SetPosition(0, SJ.connectedBody.position);

        // Health bar settings
        //Debug.Log("Init " + name + " with health " + Health + " of total " + TotalHealth);

        Id = GenerateId();
        HeightOffSet = (Id == 3) ? 0.1f : 0;
        Board.Instance.SaveDisk(Id, this);
    }

    public void Init(int alliance, bool enable) {
        Enable = enable;
        Init(alliance);
    }

    private void Update() {
        if (isMouseDown) {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hookPos = hook.transform.position;

            float speed = 1.0f;

            // Keep disk in fixed range
            if (Vector3.Distance(mousePos, hookPos) > Range) {
                transform.position = (mousePos - hookPos).normalized * Range + hookPos;
            } else {
                transform.position = mousePos;
            }

            // Clamp mouse position
            if (mousePos.z > -52 && mousePos.z < 52) {
                if (transform.position.x > 0) {
                    transform.position = new Vector3(Math.Min(transform.position.x, 28), transform.position.y, transform.position.z);
                }

                if (transform.position.x < 0) {
                    transform.position = new Vector3(Math.Max(transform.position.x, -28), transform.position.y, transform.position.z);
                }
            }

            if (line) {
                line.SetPosition(1, Rigidbody.position);
            }
        }

        if (_released) {
            transform.position = new Vector3(transform.position.x, 0.1f + HeightOffSet, transform.position.z);
        }

        if (enlarge > 0) {
            if (gameObject.transform.localScale.x < _enlargeScaleX && gameObject.transform.localScale.z < _enlargeScaleZ) {
                gameObject.transform.position += new Vector3(0, 0.05f, 0);
                gameObject.transform.localScale += new Vector3(0.1f, 0, 0.1f);
            } else {
                enlarge = 0;
            }
        } else if (enlarge < 0) {
            if (gameObject.transform.localScale.x > _shrinkScaleX && gameObject.transform.localScale.z > _shrinkScaleZ) {
                gameObject.transform.position -= new Vector3(0, 0.05f, 0);
                gameObject.transform.localScale -= new Vector3(0.1f, 0, 0.1f);
            } else {
                enlarge = 0;
            }
        }

        if (inField && !outOfBounds) {
            CheckIfOutOfBounds();
        }
        if ((outOfBounds && gameObject.transform.localScale.x > 0.1) || (Health <= 0 && gameObject.transform.localScale.x > 0.1)) {
            _rigidbody.isKinematic = false;
            gameObject.transform.localScale -= new Vector3(0.05f, 0, 0.05f);
        }
        if (gameObject.transform.localScale.x < 0.1) {
            DestroyDisk();
        }
    }

    private void CheckIfOutOfBounds() {
        if (Math.Abs(gameObject.transform.position.x) > 32 || Math.Abs(gameObject.transform.position.z) > 47) {
            outOfBounds = true;
            Board.Instance.OnDiskOutOfBound(this);
        }
    }

    private void OnMouseDown() {
        if (!Enable) {
            return;
        }

        _released = false;

        isMouseDown = true;
        Rigidbody.isKinematic = true;
        mesh.enabled = true;
        if (line) {
            line.enabled = true;
        }

        Board.Instance.OnDiskClick(this);
    }

    private void OnMouseUp() {
        if (!Enable) {
            return;
        }

        var pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        transform.position = originalPosition;

        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            photonView.RPC("Release", PhotonTargets.All, pos);
        } else {
            Release(pos);
        }
    }

    public void ReleaseOnTurnEnd() {
        Debug.Log("Disk is being relesaed from hook");
        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            Debug.Log("Releasing from within photon");
            photonView.RPC("PunReleaseOnTurnEnd", PhotonTargets.All);
        } else {
            PunReleaseOnTurnEnd();
        }
        Board.Instance._lastCreatedDisk = null;
    }

    [PunRPC]
    public void PunReleaseOnTurnEnd() {
        Debug.Log("PunReleaseOnTurnEnd");
        if (!_released) {
            Debug.Log("Disk is not released!!!!!!!!!!!!!!!!!!!!!!!");
            _forcedRelease = true;
            Release(Board.Instance.GetHook(this.Alliance).transform.position + new Vector3(0, 3f, 0));
        }
    }


    [PunRPC]
    public void Release(Vector3 pos) {
        Debug.Log("Release fired for player " + (Board.Instance.isHost ? 1 : 0) + " pos: " + transform.position.x + "," + transform.position.y + "," + transform.position.z);
        
        Enable = false;
        isMouseDown = false;

        _released = true;

        isMouseDown = false;
        Rigidbody.isKinematic = false;
        if (line) {
            Destroy(line);
        }

        transform.position = pos;
        StartCoroutine(UnHook());

        Invoke("RunOnDiskReleaseHandlers", 1);

        Board.Instance.OnDiskReleased(this);

        // GUY- Invulnerable disks are not damaged
        Invulnerable = true;

        if (!_forcedRelease) {
            Invoke("StopMoving", 6);
        }
        Invoke("SetInvulnerableFalse", 5);
    }

    public void SetInvulnerableFalse() {
        Invulnerable = false;
    }

    public void RunOnDiskReleaseHandlers() {
        foreach (var item in OnDiskRelease) {
            item.Invoke();
        }
    }

    public void StopMoving() {
        // !!!- No matter what, the disk must stop, this is why i set its velocity to 0.
        // this was when we started playing, and disk didnt have enough drag. This is not the case, and causes
        // cases such as: opponenet fires disk A, disk A still moving, you played a card and summoned B quick enough 
        // to hit previously released A. Disk A might stop spontaniously though it was hit and have increased velocity as a result
        //Rigidbody.velocity = Vector3.zero;
        

        // Anyway we should make it stiff afterward
        Rigidbody.mass = Rigidbody.mass * 1.6f;
    }

    IEnumerator UnHook() {
        mesh.enabled = false;
        yield return new WaitForSeconds(releaseTime);
        Invoke("SetInField", 1f);
        if (GetComponent<SpringJoint>()) {
            Destroy(GetComponent<SpringJoint>());
        }
        startedMoving = true;
        yield return new WaitForSeconds(endTurn);
    }

    public void SetInField() {
        inField = true;
    }


    private void OnCollisionEnter(Collision collision) {

        var disk = collision.gameObject.GetComponent<Disk>();

        if (!disk) {
            AudioManager.Instance.Play("Wall Hit");
        } else {
            AudioManager.Instance.Play("Disk Hit");
        }


        if (!Board.Instance.isYourTurn || !disk) {
            return;
        }

        // Alliance is current player alliance
        if (disk.Alliance != Alliance) {

            // Play hit effect
            EffectManager.Instance.PlayHitEffect(collision.contacts[0].point);
            disk.DealDamage(Attack);

            /*if (classType == ClassType.Rock) {
                if (disk.classType == ClassType.Paper) {
                    disk.DealDamage(Attack * 0.5);
                }
                else if (disk.classType == ClassType.Scissors) {
                    disk.DealDamage(Attack * 2);
                }
                else {
                    disk.DealDamage(Attack);
                }
            }
            else if (classType == ClassType.Paper) {
                if (disk.classType == ClassType.Paper) {
                    disk.DealDamage(Attack);
                }
                else if (disk.classType == ClassType.Scissors) {
                    disk.DealDamage(Attack * 0.5);
                }
                else {
                    disk.DealDamage(Attack * 2);
                }
            }
            else {
                if (disk.classType == ClassType.Scissors) {
                    disk.DealDamage(Attack);
                }
                else if (disk.classType == ClassType.Rock) {
                    disk.DealDamage(Attack * 0.5);
                }
                else {
                    disk.DealDamage(Attack * 2);
                }
            }*/
        }

        Board.Instance.OnDiskDamangeHandled(this, disk);
    }

    public void ForceSyncPosition() {
        if (Board.Instance.isYourTurn) {
            if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
                photonView.RPC("PunForceSyncPosition", PhotonTargets.All, transform.position);
            }
        }
    }

    [PunRPC]
    public void PunForceSyncPosition(Vector3 pos, PhotonMessageInfo info) {
        transform.position = pos;
    }

    private void DealDamage(double dmg) {
        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            photonView.RPC("PunDealDamage", PhotonTargets.All, dmg);
        } else {
            PunDealDamage(dmg);
        }
    }

    [PunRPC]
    private void PunDealDamage(double dmg) {
        Health = Health - dmg;

        if (Invulnerable) {
            return;
        }

        isDamaged++;
        Invoke("StopDamaged", 1.5f);
    }

    private void StopDamaged() {
        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            photonView.RPC("PunStopDamaged", PhotonTargets.All);
        } else {
            PunStopDamaged();
        }
    }

    [PunRPC]
    private void PunStopDamaged() {
        isDamaged--;
    }

    public void Stiff() {
        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            photonView.RPC("PunStiff", PhotonTargets.All);
        } else {
            PunStiff();
        }
    }

    [PunRPC]
    private void PunStiff() {
        if (_rigidbody) {
            _rigidbody.drag *= 1.2f;
        }
    }

    public void SetHealth(double health) {
        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            photonView.RPC("PunSetHealth", PhotonTargets.All, health);
        } else {
            PunSetHealth(health);
        }
    }

    [PunRPC]
    public void PunSetHealth(double health) {
        Health = health;
    }

    internal void DestroyDisk() {
        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            if (photonView.isMine) {
                PhotonNetwork.Destroy(photonView);
            }
        } else {
            // TODO: have a death effect
            Destroy(gameObject);
        }
    }

    /*
    ~Disk()  // destructor
    {
        // cleanup statements...
        DestroyDisk();
    }
    */


    internal void Enlarge(float ratio) {
        _enlargeScaleX = _normalScaleX * ratio;
        _enlargeScaleZ = _normalScaleZ * ratio;
        enlarge = 1;
    }


    internal void Shrink(float ratio) {
        _shrinkScaleX = gameObject.transform.localScale.x / ratio;
        _shrinkScaleZ = gameObject.transform.localScale.z / ratio;
        enlarge = -1;
    }


    public override string ToString() {
        var pos = transform.position;
        return pos.x + "," + pos.z + "=" + Id;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(Alliance);
        } else {
            Alliance = (int)stream.ReceiveNext();
        }
    }

    #region Buffs Handling

    public class Buff {
        public string Name;
        public List<GameObject> Effects = new List<GameObject>();
        public Buff(string name) {
            Name = name;
        }

        public Buff(string name, GameObject effect) {
            Name = name;
            Effects.Add(effect);
        }

        public Buff(string name, List<GameObject> effects) {
            Name = name;
            Effects.AddRange(effects);
        }
    }

    public List<Buff> Buffs = new List<Buff>();

    public bool Invulnerable { get; private set; }

    internal void AddBuff(string v) {
        Buffs.Add(new Buff(v));
    }

    internal void AddBuff(string v, GameObject effect) {
        Buffs.Add(new Buff(v, effect));
    }

    internal void AddBuff(string v, List<GameObject> effects) {
        Buffs.Add(new Buff(v, effects));
    }

    internal bool hasBuff(string v) {
        for (int i = 0; i < Buffs.Count; i++) {
            if (Buffs[i].Name == v) {
                return true;
            }
        }
        return false;
    }

    internal void RemoveBuff(string v) {

        if (!hasBuff(v)) {
            return;
        }

        for (int i = 0; i < Buffs.Count; i++) {
            if (Buffs[i].Name == v) {

                // Remove any effec associated with this 
                for (int j = 0; j < Buffs[i].Effects.Count; j++) {
                    var e = Buffs[i].Effects[j];
                    if (e) {
                        Destroy(e);
                    }
                }
            }
        }
    }

    #endregion

    void OnDestroy() {
        //Debug.Log("Disk " + Id + " destroyed");
    }
}
