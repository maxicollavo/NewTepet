using UnityEditor;
using UnityEngine;

public static class VerCascada
{
    [MenuItem("Tepet/Vasijas/zz Ver cascada")]
    static void Ver()
    {
        var sb = new System.Text.StringBuilder();
        foreach (var nombre in new[] { "CascadaArena", "SandCascade" })
        {
            var go = GameObject.Find(nombre);
            if (!go) { sb.AppendLine(nombre + ": no esta"); continue; }
            foreach (var t in go.GetComponentsInChildren<Transform>(true))
            {
                string comps = "";
                foreach (var c in t.GetComponents<Component>()) comps += c.GetType().Name + " ";
                sb.AppendLine($"{t.name} pos={t.position} rot={t.eulerAngles} esc={t.lossyScale} activo={t.gameObject.activeInHierarchy} layer={LayerMask.LayerToName(t.gameObject.layer)} [{comps}]");
                var r = t.GetComponent<Renderer>(); if (r) sb.AppendLine($"   bounds {r.bounds.center} {r.bounds.size} mat={(r.sharedMaterial ? r.sharedMaterial.name : "-")}");
                var col = t.GetComponent<Collider>(); if (col) sb.AppendLine($"   collider {col.GetType().Name} trigger={col.isTrigger} {col.bounds.center} {col.bounds.size}");
                var ps = t.GetComponent<ParticleSystem>();
                if (ps)
                {
                    var m = ps.main; var sh = ps.shape; var co = ps.collision; var em = ps.emission;
                    sb.AppendLine($"   PS loop={m.loop} life={m.startLifetime.constant} speed={m.startSpeed.constant} size={m.startSize.constant} grav={m.gravityModifier.constant} sim={m.simulationSpace} max={m.maxParticles} rate={em.rateOverTime.constant}");
                    sb.AppendLine($"   shape={sh.shapeType} rad={sh.radius} angle={sh.angle} pos={sh.position} rot={sh.rotation} scale={sh.scale}");
                    sb.AppendLine($"   collision={co.enabled} type={co.type} mode={co.mode} dampen={co.dampen.constant} bounce={co.bounce.constant} loss={co.lifetimeLoss.constant} layers={co.collidesWith.value}");
                    var rr = t.GetComponent<ParticleSystemRenderer>(); if (rr) sb.AppendLine($"   render={rr.renderMode} mat={(rr.sharedMaterial ? rr.sharedMaterial.name : "-")}");
                }
            }
        }
        var vfx = Object.FindObjectsByType<UnityEngine.VFX.VisualEffect>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var v in vfx)
        {
            if (v.visualEffectAsset == null) continue;
            sb.AppendLine($"VFX {v.name} asset={AssetDatabase.GetAssetPath(v.visualEffectAsset)}");
            var props = new System.Collections.Generic.List<UnityEngine.VFX.VFXExposedProperty>();
            v.visualEffectAsset.GetExposedProperties(props);
            foreach (var p in props) sb.AppendLine($"   expuesta {p.name} ({p.type?.Name})");
        }
        // piso debajo de la cascada
        var sc = GameObject.Find("SandCascade");
        if (sc && Physics.Raycast(sc.transform.position + Vector3.up * 0.5f, Vector3.down, out var hit, 50f))
            sb.AppendLine($"piso debajo de SandCascade: {hit.point.y} ({hit.collider.name})");
        var cam = Camera.main; if (cam) sb.AppendLine("camara layer " + LayerMask.LayerToName(cam.gameObject.layer));
        foreach (var h in Object.FindObjectsByType<HandVases>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            sb.AppendLine($"mano {h.name} layer={LayerMask.LayerToName(h.gameObject.layer)} pos={h.transform.position}");
        System.IO.File.WriteAllText("ver_cascada.txt", sb.ToString());
        Debug.Log("[Cascada] listo ver_cascada.txt");
    }
}
