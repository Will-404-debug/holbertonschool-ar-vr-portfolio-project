using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;                 // InputActionReference (new Input System)
using UnityEngine.XR;                         // InputDevices, CommonUsages
using UnityEngine.XR.Interaction.Toolkit;     // XRGrabInteractable, IXRSelectInteractor
using UMol.API;                               // APIPython

// Disambiguate: XR InputDevice vs Input System InputDevice
using XRInputDevice = UnityEngine.XR.InputDevice;
using XRCommonUsages  = UnityEngine.XR.CommonUsages;

namespace UMol {
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class TwoHandGrabScaler : MonoBehaviour {
    [Header("APIPython global scale")]
    public float sizeScale = 0.10f;
    public Vector2 scaleLimits = new Vector2(0.005f, 0.5f);
    public float sendEpsilon = 0.0005f;

    [Header("Rotation (when NOT scaling)")]
    [Range(0f, 1f)] public float rotationSmoothing = 0.18f;
    [Range(0f, 1f)] public float twistWeight = 0.35f;

    [Header("Trigger (Action‑based, assign in Inspector)")]
    public InputActionReference leftActivate;
    public InputActionReference rightActivate;
    [Range(0f, 1f)] public float triggerThreshold = 0.5f;

    [Header("Pivot")]
    public bool recenterPivotOnTwoHand = true;

    [Header("Plumbing")]
    [Tooltip("If OFF, bypass APIPython and directly scale LoadedMolecules for debugging.")]
    public bool useAPIPython = true;

    [Header("Debug")]
    public bool debugScale = false;
    public bool debugInput = false;

    // XR selection
    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor first, second;
    Transform firstAttach, secondAttach;

    // two‑hand baseline (for rotation)
    bool twoHandActive;
    Quaternion startWorldRot;
    Quaternion startHandsFrameW;
    Vector3 fixedWorldPos;

    // scaling state
    bool scalingActive;
    bool sentOnce = false;
    float startDistForScale;
    float scaleStart;
    float lastSent;

    // UnityMol refs
    Transform loadedMols;
    UnityMolStructureManager sm;

    bool warnedMissingActions;

    void Awake() {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.attachEaseInTime = 0f;
        grab.selectMode = UnityEngine.XR.Interaction.Toolkit.Interactables.InteractableSelectMode.Multiple;
        grab.trackPosition = true;
        grab.trackRotation = true;

        grab.selectEntered.AddListener(OnSelectEntered);
        grab.selectExited.AddListener(OnSelectExited);

        sm = UnityMolMain.getStructureManager();
        var repRoot = UnityMolMain.getRepresentationParent();
        if (repRoot != null) loadedMols = repRoot.transform;
    }

    void OnDestroy() {
        grab.selectEntered.RemoveListener(OnSelectEntered);
        grab.selectExited.RemoveListener(OnSelectExited);
    }

    void OnEnable() {
        if (leftActivate  != null && leftActivate.action  != null)  leftActivate.action.Enable();
        if (rightActivate != null && rightActivate.action != null) rightActivate.action.Enable();
    }

    void OnDisable() {
        if (leftActivate  != null && leftActivate.action  != null)  leftActivate.action.Disable();
        if (rightActivate != null && rightActivate.action != null) rightActivate.action.Disable();
    }

    void OnSelectEntered(SelectEnterEventArgs args) {
        if (first == null) {
            first = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor;
            firstAttach = first?.GetAttachTransform(grab);
            twoHandActive = false;
        } else if (second == null && args.interactorObject != first) {
            second = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor;
            secondAttach = second?.GetAttachTransform(grab);

            grab.trackPosition = false;
            grab.trackRotation = false;

            BeginTwoHand(PoseFrom(firstAttach), PoseFrom(secondAttach));
            twoHandActive = true;
        }
    }

    void OnSelectExited(SelectExitEventArgs args) {
        var who = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor;

        if (who == second) {
            second = null; secondAttach = null;
            EndTwoHand();
        } else if (who == first) {
            first = second; firstAttach = secondAttach;
            second = null; secondAttach = null;
            EndTwoHand();
        }
    }

    void EndTwoHand() {
        if (debugInput && scalingActive) Debug.Log("[TwoHandGrabScaler] Scaling ended (lost second hand).");
        twoHandActive = false;
        scalingActive = false;
        grab.trackPosition = true;
        grab.trackRotation = true;
    }

    void Update() {
        if (!twoHandActive || firstAttach == null || secondAttach == null) return;

        bool triggerDown = ReadTriggerEither();

        if (triggerDown && !scalingActive) {
            BeginScaling();
            if (debugInput) Debug.Log("[TwoHandGrabScaler] Scaling started (trigger pressed).");
        } else if (!triggerDown && scalingActive) {
            scalingActive = false;
            if (debugInput) Debug.Log("[TwoHandGrabScaler] Scaling stopped (trigger released).");
        }

        var pA = PoseFrom(firstAttach);
        var pB = PoseFrom(secondAttach);

        if (scalingActive)
            UpdateScale(pA, pB);
        else
            UpdateRotate(pA, pB);
    }

    // -------- rotation (no scaling) --------
    void BeginTwoHand(Pose leftW, Pose rightW) {
        if (recenterPivotOnTwoHand) RecenterPivotToBoundsCenter();

        startWorldRot = transform.rotation;
        startHandsFrameW = BuildHandsFrameWorld(leftW, rightW);
        fixedWorldPos = transform.position;
    }

    void UpdateRotate(Pose leftW, Pose rightW) {
        Quaternion currentHandsW = BuildHandsFrameWorld(leftW, rightW);
        Quaternion desiredWorldRot = currentHandsW * Quaternion.Inverse(startHandsFrameW) * startWorldRot;

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredWorldRot, rotationSmoothing);
        transform.position = fixedWorldPos;
    }

    // -------- scaling (APIPython global) --------
    void BeginScaling() {
        scalingActive = true;

        startDistForScale = Mathf.Max(Vector3.Distance(firstAttach.position, secondAttach.position), 1e-4f);

        if (loadedMols == null) {
            var repRoot = UnityMolMain.getRepresentationParent();
            if (repRoot != null) loadedMols = repRoot.transform;
        }
        scaleStart = (loadedMols != null) ? loadedMols.localScale.x : 0.1f;
        
        // force the very first UpdateScale() to send
        lastSent = float.NaN;
        sentOnce = false;
        
        if (debugScale)
            Debug.Log($"[TwoHandGrabScaler] BeginScaling"
                    + $" startDist:{startDistForScale:F3}"
                    + $" scaleStart:{scaleStart:F4}"
                    + $" sizeScale:{sizeScale:F3}");
    }

    void UpdateScale(Pose leftW, Pose rightW) {
        transform.position = fixedWorldPos;
        transform.rotation = startWorldRot; // freeze rotation while scaling

        float distNow = Mathf.Max(Vector3.Distance(leftW.position, rightW.position), 1e-4f);
        float diff    = sizeScale * (distNow - startDistForScale);
        float scaleVal = Mathf.Clamp(scaleStart + diff, scaleLimits.x, scaleLimits.y);
        
        // send at least once, then throttle by epsilon
        bool mustSend = !sentOnce || Mathf.Abs(scaleVal - lastSent) >= Mathf.Max(1e-6f, sendEpsilon);
        if (!mustSend) return;
        
        if (useAPIPython) {
            // VIU-style reparent -> API -> restore
            if (sm != null && loadedMols != null) {
                List<Transform> savedParents = new List<Transform>(sm.structureToGameObject.Count);
                foreach (GameObject gos in sm.structureToGameObject.Values) {
                    savedParents.Add(gos.transform.parent);
                    gos.transform.parent = loadedMols;
                }
                
                API.APIPython.changeGeneralScale(scaleVal);
                
                int id = 0;
                foreach (GameObject gos in sm.structureToGameObject.Values)
                    gos.transform.parent = savedParents[id++];
            } else {
                API.APIPython.changeGeneralScale(scaleVal);
            }
        } else {
            // DEBUG PATH: bypass APIPython and scale directly
            if (loadedMols != null)
                loadedMols.localScale = new Vector3(scaleVal, scaleVal, scaleVal);
        }
        
        lastSent = scaleVal;
        sentOnce = true;
        
        if (debugScale)
            Debug.Log($"[TwoHandGrabScaler] distNow:{distNow:F3} diff:{diff:F4} -> scaleVal:{scaleVal:F4}");
    }

    // -------- helpers --------
    static Pose PoseFrom(Transform t) =>
        !t ? new Pose(Vector3.zero, Quaternion.identity) : new Pose(t.position, t.rotation);

    bool ReadTriggerEither() {
        // Input Actions (Value)
        float leftAct  = ReadAction(leftActivate, "Left");
        float rightAct = ReadAction(rightActivate, "Right");

        // Fallback to XR CommonUsages if needed
        if (leftAct <= 0f && rightAct <= 0f) {
            float leftDev  = ReadDeviceTrigger(isLeft: true,  0f);
            float rightDev = ReadDeviceTrigger(isLeft: false, 0f);
            if (debugInput)
                Debug.Log($"[TwoHandGrabScaler] Fallback XR trigger L:{leftDev:0.00} R:{rightDev:0.00} (thr {triggerThreshold:0.00})");
            return (leftDev >= triggerThreshold) || (rightDev >= triggerThreshold);
        }

        return (leftAct >= triggerThreshold) || (rightAct >= triggerThreshold);
    }

    float ReadAction(InputActionReference actionRef, string label) {
        if (actionRef == null || actionRef.action == null)
            return 0f;

        var a = actionRef.action;
        if (!a.enabled) a.Enable();

        float v = 0f;
        try { v = a.ReadValue<float>(); } catch { /* some bindings aren’t float; ignore */ }

        // Some devices/maps don’t deliver a float > 0, but the action phase is still pressed.
        if (v <= 0f && a.IsPressed()) v = 1f;

        if (debugInput)
            Debug.Log($"[TwoHandGrabScaler] Action {label} value:{v:0.00} phase:{a.phase}");

        return v;
    }

    float ReadDeviceTrigger(bool isLeft, float defaultVal = 0f) {
        var list = s_devicesXR ??= new List<XRInputDevice>(4);
        list.Clear();

        var desired = InputDeviceCharacteristics.HeldInHand |
                        InputDeviceCharacteristics.Controller |
                        (isLeft ? InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right);

        InputDevices.GetDevicesWithCharacteristics(desired, list);
        for (int i = 0; i < list.Count; i++) {
            if (list[i].TryGetFeatureValue(XRCommonUsages.trigger, out float v))
                return v;
        }
        return defaultVal;
    }
    static List<XRInputDevice> s_devicesXR;

    void WarnIfMissingActions() {
        if (!warnedMissingActions &&
            ((leftActivate == null || leftActivate.action == null) ||
                (rightActivate == null || rightActivate.action == null))) {
            warnedMissingActions = true;
            Debug.LogWarning("[TwoHandGrabScaler] Assign Left/Right Activate (Value) InputActionReferences " +
                                "to enable trigger‑to‑scale. Falling back to CommonUsages.trigger.");
        }
    }

    void RecenterPivotToBoundsCenter() {
        var rends = GetComponentsInChildren<Renderer>(includeInactive: true);
        if (rends.Length == 0) return;

        Bounds b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);

        Vector3 worldCenter = b.center;
        Vector3 delta = transform.position - worldCenter;

        transform.position = worldCenter;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).position += delta;
    }

    Quaternion BuildHandsFrameWorld(Pose left, Pose right) {
        Vector3 fwd = (right.position - left.position);
        if (fwd.sqrMagnitude < 1e-12f) fwd = Vector3.forward;
        fwd.Normalize();

        Vector3 worldUpProj = Vector3.ProjectOnPlane(Vector3.up, fwd);
        if (worldUpProj.sqrMagnitude < 1e-12f) worldUpProj = Vector3.up;

        Vector3 avgUp = (left.rotation * Vector3.up + right.rotation * Vector3.up) * 0.5f;
        avgUp = Vector3.ProjectOnPlane(avgUp, fwd);
        if (avgUp.sqrMagnitude < 1e-12f) avgUp = worldUpProj;

        Vector3 up = Vector3.Lerp(worldUpProj.normalized, avgUp.normalized, Mathf.Clamp01(twistWeight));
        Vector3 rightV = Vector3.Cross(up, fwd).normalized;
        up = Vector3.Cross(fwd, rightV).normalized;

        return Quaternion.LookRotation(fwd, up);
    }
}
}
