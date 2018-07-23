using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloodlust : Photon.PunBehaviour {

    // Use this for initialization
    private Disk self;
    private void Awake() {
        Invoke("Init", 0.5f);
    }
    void Start() {
        Invoke("Init", 0.5f);
    }

    public void Init() {
        if (false && PhotonNetwork.connected && PhotonNetwork.inRoom) {
            if (photonView.isMine) {
                photonView.RPC("PunBloodlustInit", PhotonTargets.All);
            }
        } else {
            PunBloodlustInit();
        }
    }

    [PunRPC]
    public void PunBloodlustInit() {
        Debug.LogError("Bloodlust : PunInit " + PhotonNetwork.isMasterClient);
        try {
            self = GetComponent<Disk>();

            EffectManager.Instance.PlayEffect("BloodlustCaster", transform.position, gameObject);

            self.OnDiskRelease.Add(delegate () {
                var disks = Board.Instance.DisksList.FindAll(disk => disk && disk.gameObject && disk.Alliance == self.Alliance);
                for (int i = 0; i < disks.Count; i++) {
                    disks[i].Enlarge(2);
                    if (disks[i] != self) {
                        var ins = EffectManager.Instance.PlayEffect("Bloodlust", disks[i].transform.position, disks[i].gameObject);
                        disks[i].AddBuff("Bloodlust", ins);
                    }
                }
            });
        } catch(Exception e) {
            Debug.LogError("Bloodlust : error " + e.Message);
        }
    }


    private void OnDestroy() {

        self.RemoveBuff("BloodlustCaster");
        var disks = Board.Instance.DisksList.FindAll(disk => disk && disk.gameObject && disk.Alliance == self.Alliance && disk.hasBuff("Bloodlust"));

        // Only disks of the same alliance that has bloodlust buff
        for (int i = 0; i < disks.Count; i++) {
            disks[i].Shrink(2);
            disks[i].RemoveBuff("Bloodlust");
        }
    }

}
