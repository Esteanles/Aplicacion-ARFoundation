using UnityEngine;

/// <summary>
/// Controla el cambio de apariencia visual de un objeto mediante el cambiode materiales.
/// </summary>
/// <remarks>
/// Este script afecta tanto al objeto actual como a todos sus hijos que posean un componente <see cref="Renderer"/>.
/// </remarks>
public class InteraccionObjeto : MonoBehaviour
{
    /// <summary>Material inicial del objeto.</summary>
    public Material originalMat;

    /// <summary>Material secundario.</summary>
    public Material altMat;

    /// <summary>Estado interno para identificar qué material se está mostrando actualmente.</summary>
    private bool usingOriginal = true;

    /// <summary>
    /// Alterna entre el material original y el alternativo en toda la jerarquía del objeto.
    /// </summary>
    /// <remarks>
    /// Este método utiliza <c>GetComponentsInChildren</c>, por lo que es ideal para prefabs con varios elementos.
    /// Se recomienda no llamarlo con excesiva frecuencia (como en un Update) por el costo de búsqueda de componentes.
    /// </remarks>
    public void ToggleMaterial()
    {
    
        usingOriginal = !usingOriginal;
        
        
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in childRenderers)
        {
            
            rend.material = usingOriginal ? originalMat : altMat;
        }
    }
}



