﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloodlust : Photon.PunBehaviour {

    // Use this for initialization
    private Disk self;
    private void Awake() {
        Init();
    }
    void Start() {
        Init();
    }

    public void Init() {
        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            photonView.RPC("PunInit", PhotonTargets.All);
        } else {
            PunInit();
        }
    }

    [PunRPC]
    public void PunInit() {
        Debug.LogError("Bloodlust : PunInit");
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
