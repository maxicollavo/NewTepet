using UnityEngine;

public class CambiarMaterial : MonoBehaviour
{
    public Material nuevoMaterial; // El material que quieres poner
    public int indiceMaterial = 0; // Índice del material a reemplazar (0 a 3 si tienes 4)



    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material[] materiales = renderer.materials;

            if (indiceMaterial >= 0 && indiceMaterial < materiales.Length)
            {
                materiales[indiceMaterial] = nuevoMaterial;
                renderer.materials = materiales;
            }
            else
            {
                Debug.LogWarning("Índice fuera de rango.");
            }
        }
    }
}