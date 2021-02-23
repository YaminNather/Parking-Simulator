using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DummyCar
{
    public class SafetyCheckListener : SafetyChecker.SafetyCheckListener
    {
        protected override void fOnEnterDangerRange()
        {
            MeshRenderer[] meshRenderers = transform.parent.GetComponentsInChildren<MeshRenderer>();
            List<Material> materials = new List<Material>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
                materials.AddRange(meshRenderer.materials);

            foreach (Material material in materials)
            {
                Color curColor = material.GetColor("_BaseColor");
                material.SetColor("_BaseColor", Color.Lerp(curColor, Color.red, 0.5f));
            }
        }
    }
}
