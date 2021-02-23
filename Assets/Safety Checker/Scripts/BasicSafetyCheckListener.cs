using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSafetyCheckListener : SafetyChecker.SafetyCheckListener
{
    protected override void fOnEnterDangerRange() => fSetSafetyRangeValueInMaterial(1.0f);

    protected override void fOnEnterWarningRange() => fSetSafetyRangeValueInMaterial(0.5f);

    protected override void fOnEnterSafetyRange() => fSetSafetyRangeValueInMaterial(0.0f);

    private void fSetSafetyRangeValueInMaterial(float value)
    {
        foreach (Material material in fGetAllMaterials())
            material.SetFloat("_SafetyRange", value);
    }

    private Material[] fGetAllMaterials()
    {
        List<Material> materials = new List<Material>();
        foreach (MeshRenderer meshRenderer in transform.GetComponentsInChildren<MeshRenderer>())
            materials.AddRange(meshRenderer.materials);

        return (materials.ToArray());
    }
}
