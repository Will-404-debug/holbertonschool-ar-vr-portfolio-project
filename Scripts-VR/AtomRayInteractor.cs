using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

namespace UMol {
public class AtomRayInteractor : XRRayInteractor, ILineRenderable {
    private LayerMask notUILayer;
    private CustomRaycastBurst raycaster;
    private UnityMolStructureManager sm;

    private static readonly float[] angles = { 0f, 30f, 60f };
    private IXRInteractable currentValidTarget = null;

    [SerializeField]
    private UnityEvent onRaycasterUpdateRequest = new UnityEvent();
    
    public Vector3? SelectionHitPosition { get; private set; }
    public Vector3? CurrentHitPosition { get; private set; }

    protected override void Start() {
            base.Start();
            sm = UnityMolMain.getStructureManager();
            raycaster = UnityMolMain.getCustomRaycast();

            if (raycaster == null) {
                Debug.LogWarning($"[AtomRayInteractor] Raycaster is not assigned on {gameObject.name}");
            }

            raycastMask = LayerMask.GetMask();
            
            notUILayer = ~LayerMask.GetMask("UI", "Ignore Raycast");
        }

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);
        
        if (args.interactableObject.transform != null) {
            SelectionHitPosition = args.interactableObject.transform.position;
            Debug.Log($"[AtomRayInteractor] Grabbed atom at: {SelectionHitPosition.Value}");
        } else {
            SelectionHitPosition = null;
            Debug.LogWarning("[AtomRayInteractor] Could not determine grabbed object's position.");
        }
    }
    
    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);
        TriggerRaycastUpdate(); // Method that triggers raycaster update
        SelectionHitPosition = null;
    }

    public override void GetValidTargets(List<IXRInteractable> targets) {
        targets.Clear();

        // Only update target if not already selecting something
        if (hasSelection || !isActiveAndEnabled) {
            CurrentHitPosition = null;
            return;
        }

        currentValidTarget = null;
        UnityMolAtom atomHit = null;
        Vector3 hitPosition = Vector3.zero;
        bool isExtrAtom = false;

        foreach (float angle in angles) {
            Vector3 direction = Quaternion.AngleAxis(angle, transform.right) * transform.rotation * Vector3.forward;
            atomHit = raycaster.customRaycastAtomBurst(transform.position, direction, ref hitPosition, ref isExtrAtom, true);
            if (atomHit != null) {
                break;
            }
        }

        if (atomHit != null) {
            Vector3 toHit = (hitPosition - transform.position).normalized;
            float alignment = Vector3.Dot(transform.forward, toHit);

            if (alignment > 0.95f &&
                atomHit.residue != null &&
                atomHit.residue.chain != null &&
                atomHit.residue.chain.model != null &&
                atomHit.residue.chain.model.structure != null)
            {
                string structureName = atomHit.residue.chain.model.structure.name;
                if (sm.structureToGameObject.TryGetValue(structureName, out GameObject structure)) {
                    if (structure.CompareTag("Molecule") && structure.TryGetComponent(out IXRInteractable interactable)) {
                        currentValidTarget = interactable;
                        targets.Add(interactable);
                    }
                }
                CurrentHitPosition = hitPosition;
                return;
            }
        }


        CurrentHitPosition = null;
    }

    public override bool CanSelect(IXRSelectInteractable interactable) {
        // Allow ongoing selection to persist
        return hasSelection || (interactable.transform.CompareTag("Molecule") && interactable == currentValidTarget);
    }

    public override bool CanHover(IXRHoverInteractable interactable) {
        return hasSelection || (interactable.transform.CompareTag("Molecule") && interactable == currentValidTarget);
    }

    public void TriggerRaycastUpdate() {
        if (raycaster != null) {
            raycaster.needsUpdatePos = true;
        }
    }
}
}
