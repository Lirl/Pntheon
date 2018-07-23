using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {
    MeshRenderer mesh;
    public Material[] materials = new Material[9];
    public int Alliance;
    public int X;
    public int Y;
    public int Score = 1;
    public bool IsGlowing;
    private Color defaultColor;
    private int materialCode;

    void Awake() {
        mesh = GetComponent<MeshRenderer>();
    }

    void Start() {
        mesh = GetComponent<MeshRenderer>();
    }

    public void Init(int alliance, int x, int y) {
        this.X = x;
        this.Y = y;
        SetAlliance(alliance);
    }

    internal void Init(int alliance, int x, int y, int score) {
        this.Score = score;
        Init(alliance, x, y);

        // TODO : change material or color or add some golden effect
    }

    public void SetAlliance(int alliance) {        
        Alliance = alliance;
        if (alliance == -1) {
            alliance = 0; // default material
        } else {
            if (alliance == 0) {
                alliance = alliance + 2;
            } else {
                alliance = alliance + 3;
            }
        }

        // 0 - normal grey
        // 1 - dark grey
        // 2 - red normal
        // 3 - red dark
        // 4 - blue normal
        // 5 - blue dark
        // 6 - Glowing
        // 7 - Golden Normal
        // 8 - Golden Dark

        if (Score == 2 && alliance == 0) {
            Debug.Log("Score of cube is 2, material selected is " + materials[7 + (X % 2)]);
            int code = 7 + ((X + Y) % 2);
            mesh.material = materials[code];
            materialCode = code;
        }
        else {
            materialCode = alliance + ((X + Y) % 2 == 0 ? 1 : 0);
            mesh.material = materials[materialCode];    
        }
        defaultColor = mesh.material.color;
    }

    public override string ToString() {
        return X + "," + Y + "=" + Alliance;
    }


    private void OnTriggerEnter(Collider other) {
        // check if this is not your turn.
        // collider is handled only in the player who has the turn
        /*if(!Board.Instance.isYourTurn && !Board.Instance.isTutorial) {
            return;
        }*/

        /*if(PhotonNetwork.inRoom && !Board.Instance.isHost) {
            return;
        }*/


        // Each player handles the collide of his disks
        if (PhotonNetwork.inRoom && !PhotonNetwork.isMasterClient) {
            return;
        }

        var disk = other.gameObject.GetComponent<Disk>();

        if (disk && disk.isDamaged <= 0) {
            Board.Instance.HandleSetTileAlliance(disk.Alliance, X, Y);
        }
    }

    private void Update() {
        if(IsGlowing) {
            var pingpong = Mathf.PingPong(Time.time, 0.3f) / 0.3f;
            mesh.material.Lerp(materials[materialCode], materials[6], pingpong);
            //mesh.material.color = Color.Lerp(defaultColor,Color.white, pingpong);
        } else {
            mesh.material = materials[materialCode];
            //mesh.material.color = defaultColor;
        }
    }

    public void toggleGlow() {
        IsGlowing = !IsGlowing;
    }


}
