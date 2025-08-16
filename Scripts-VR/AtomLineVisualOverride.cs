using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

namespace UMol {
    public class AtomLineVisualOverride : MonoBehaviour {

    [Header("References")]
    public AtomRayInteractor atomRayInteractor;
    public XRInteractorLineVisual lineVisual;
    public float fallbackLength = 5f;

    [Header("Colors")]
    public Gradient defaultColor;
    public Gradient activeColor;

    void Start() {
        if (lineVisual != null) {
            lineVisual.invalidColorGradient = defaultColor;
            lineVisual.validColorGradient = activeColor;
            lineVisual.overrideInteractorLineLength = true;
        }
    }
    void LateUpdate() {
        if (atomRayInteractor == null || lineVisual == null)
            return;

        Vector3 start = atomRayInteractor.attachTransform.position;
        Vector3 end;

        if (atomRayInteractor.hasSelection && atomRayInteractor.SelectionHitPosition.HasValue) {
            end = atomRayInteractor.SelectionHitPosition.Value;
        } else if (atomRayInteractor.CurrentHitPosition.HasValue) {
            end = atomRayInteractor.CurrentHitPosition.Value;
        } else {
            end = start + atomRayInteractor.attachTransform.forward * fallbackLength;
        }

        lineVisual.lineLength = Vector3.Distance(start, end);
        
        if (atomRayInteractor.hasSelection) {
            lineVisual.validColorGradient = activeColor;
        } else {
            lineVisual.invalidColorGradient = defaultColor;
        }
    }
}
}
