using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.UI;

public class UIRayInteractorToggle : MonoBehaviour {
    [Header("Left Hand")]
    public XRRayInteractor uiRayInteractorLeft;
    public XRRayInteractor atomRayInteractorLeft;
    public GraphicRaycaster leftRaycaster;

    [Header("Right Hand")]
    public XRRayInteractor uiRayInteractorRight;
    public XRRayInteractor atomRayInteractorRight;
    public GraphicRaycaster rightRaycaster;

    [Header("Event System")]
    public EventSystem eventSystem;

    private PointerEventData pointerEventDataLeft;
    private PointerEventData pointerEventDataRight;

    void Awake() {
        if (eventSystem == null)
            eventSystem = EventSystem.current;

        pointerEventDataLeft = new PointerEventData(eventSystem);
        pointerEventDataRight = new PointerEventData(eventSystem);
    }

    void Update() {
        UpdateInteractorState(uiRayInteractorLeft, atomRayInteractorLeft, leftRaycaster, pointerEventDataLeft);
        UpdateInteractorState(uiRayInteractorRight, atomRayInteractorRight, rightRaycaster, pointerEventDataRight);
    }

    void UpdateInteractorState(XRRayInteractor uiInteractor, XRRayInteractor atomInteractor, GraphicRaycaster raycaster, PointerEventData pointerData) {
        if (uiInteractor == null || atomInteractor == null || raycaster == null || eventSystem == null)
            return;

        if (uiInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit)) {
            pointerData.position = Camera.main.WorldToScreenPoint(hit.point);
            var results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            bool hoveringUI = results.Count > 0;
            uiInteractor.enabled = hoveringUI;
            atomInteractor.enabled = !hoveringUI;
        } else {
            uiInteractor.enabled = false;
            atomInteractor.enabled = true;
        }
    }
}
