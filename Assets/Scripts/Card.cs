using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public int Code;
    public string Name;
    public CanvasGroup CanvasGroup;
    public bool _initialized;

    private void Start() {
        Init();
    }

    private void Awake() {
        Init();
    }

    private void Init() {
        if(_initialized) {
            return;
        }

        canvas = GameObject.Find("Canvas");
        layout = GetComponent<LayoutElement>();
    }

    public static GameObject cardBeingDragged = null;
    Vector3 startPosition;
    Transform hand;
    private int siblingIndex;
    GameObject canvas;
    LayoutElement layout;
    float alpha = 1.0f;
    GameObject effect;

    public static GameObject CreateCard(int code, Transform parent) {
        var ui = Resources.Load("Cards/Card" + code);
        var ins = Instantiate(ui, new Vector3(), Quaternion.Euler(new Vector3(90f, (!Board.Instance.isHost) ? 180f : 0f, 0f))) as GameObject;

        ins.transform.localScale = new Vector3(.25f, .25f, .25f);

        if (parent) {
            ins.transform.parent = parent;
            ins.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        }

        var card = ins.GetComponent<Card>();
        card.CanvasGroup = ins.GetComponent<CanvasGroup>();
        //ins.GetComponent<Button>().onClick.AddListener(Board.Instance.CreateSelectedDisk);

        return ins;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        startPosition = transform.position;
        hand = transform.parent;
        siblingIndex = transform.GetSiblingIndex();

        if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
            effect = PhotonNetwork.Instantiate("SummonSphere", Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity, 0);
        } else {
            var prefab = Resources.Load("SummonSphere");
            effect = (GameObject) Instantiate(prefab, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
        }

        transform.parent = GameObject.Find("Canvas").transform;
        layout.enabled = false;

        Board.Instance.CardBeginDrag(this);
    }

    public void OnDrag(PointerEventData eventData) {
        //transform.position = Input.mousePosition;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.GetComponentInParent<Canvas>().worldCamera, out pos);
        transform.position = canvas.transform.TransformPoint(pos);
        
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        effect.transform.position = new Vector3(mousePos.x, 10, mousePos.z);

        Debug.Log(Math.Abs(startPosition.z - transform.position.z));
        if (Math.Abs(startPosition.z - transform.position.z) > 10.0f) {
            alpha = 0.0f;
        } else {
            alpha = 1f;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        var cube = GetValideCube();
        var board = Board.Instance;
        if (cube) { // if cube defined, we played a valid play
            board.SetHookPosition(board.isHost ? 1 : 0, cube.transform.position);
            board.CreateSelectedDisk(this, cube);
            DestroyEffect();
            //GetComponent<Image>().enabled = false;
            Destroy(this);
        } else {
            transform.position = startPosition;
            transform.parent = hand;
            transform.SetSiblingIndex(siblingIndex);
            layout.enabled = true;

            alpha = 1f;
            DestroyEffect();
        }

        Board.Instance.CardEndDrag(this);
    }

    private void DestroyEffect() {
        if (effect) {
            if (PhotonNetwork.connected && PhotonNetwork.inRoom) {
                PhotonNetwork.Destroy(effect);
            } else {
                Destroy(effect);
            }
            effect = null;
        }
    }

    private Cube GetValideCube() {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50.0f, LayerMask.GetMask("Board"))) {

            var cube = hit.collider.gameObject.GetComponent<Cube>();
            if (cube && (Board.Instance.isCubeValidPlay(cube, this))) {
                return cube;
            }
        }

        return null;
    }

    public void DestroyCard() {
        transform.parent = null;
        Destroy(this);
    }

    private void FixedUpdate() {
        if (alpha > CanvasGroup.alpha) {
            CanvasGroup.alpha += 0.1f;
        }

        if (alpha < CanvasGroup.alpha) {
            CanvasGroup.alpha -= 0.1f;
        }
    }
}