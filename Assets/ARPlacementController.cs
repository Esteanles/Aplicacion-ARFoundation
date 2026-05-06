using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Gestiona la interacción AR, el cambio de prefabs cada dos elementos colocados 
/// y la modificación de materiales personalizados.
/// </summary>
/// <remarks>
/// Requiere que el objeto tenga un <see cref="ARRaycastManager"/> en la escena.
/// </remarks>
public class ARPlacementController : MonoBehaviour
{
    
    /// <summary>Componente encargado de detectar las superficies.</summary>
    public ARRaycastManager raycastManager;
    
    /// <summary>Arreglo de modelos 3D que se agregaran a la escena.</summary>
    public GameObject[] placementPrefab;
    
    /// <summary>Componente UI para mostrar los mensajes al usuario.</summary>
    public TextMeshProUGUI message;

 
    /// <summary>Radio de posición aleatoria de spawn.</summary>
    /// <value>Define la dispersión en metros (m).</value>
    public float spawnRadius = 0.3f;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private List<GameObject> spawnObjectsInScene = new List<GameObject>();
    
    private int prefabIndex = 0;   
    private int counterSpawn = 0;  

    /// <summary>
    /// Captura el input del usuario y decide si cambiar color o posicionar un objeto.
    /// </summary>
    void Update()
    {
        Vector2 inputPosition = GetInputPosition(out bool pressed);
        if (!pressed) return;

        //  Cambio de material
        if (IntentarCambiarColor(inputPosition)) return;

        // Detección de superficies horizontales
        if (raycastManager.Raycast(inputPosition, hits, TrackableType.PlaneWithinPolygon | TrackableType.Planes))
        {
            if (counterSpawn < 6)
            {
                InstanciarModeloAR(hits[0].pose);
            }
            else 
            {
                ReiniciarEscena();
            }
        }
    }

    /// <summary>
    /// Coloca el modelo actual y gestiona el cambio de prefab cada dos objetos.
    /// </summary>
    /// <param name="hitPose">Posición detectada por el Raycast de AR.</param>
    private void InstanciarModeloAR(Pose hitPose)
    {
        Vector3 offset = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0, Random.Range(-spawnRadius, spawnRadius));
        GameObject obj = Instantiate(placementPrefab[prefabIndex], hitPose.position + offset, hitPose.rotation);
        
        spawnObjectsInScene.Add(obj);
        counterSpawn++;

        if (counterSpawn % 2 == 0 && prefabIndex < placementPrefab.Length - 1)
        {
            prefabIndex++;
        }

        SetMessage($"Objeto {counterSpawn}/6 colocado.");
    }

    /// <summary>
    /// Detecta si se tocó un objeto existente para activar su cambio de material.
    /// </summary>
    /// <param name="screenPos">Posición del toque o click en coordenadas de pantalla.</param>
    /// <returns><c>true</c> si se golpeó un objeto con el componente adecuado si no<c>false</c></returns>
    private bool IntentarCambiarColor(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
           
            InteraccionObjeto interact = hit.collider.GetComponentInParent<InteraccionObjeto>();
            if (interact != null)
            {
                interact.ToggleMaterial();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Obtiene la posición del click.
    /// </summary>
    /// <param name="pressed">Parámetro que indica si hubo una pulsación en este frame.</param>
    /// <returns>Un <see cref="Vector2"/> con las coordenadas en pantalla.</returns>
    private Vector2 GetInputPosition(out bool pressed)
    {
        pressed = false;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            pressed = true;
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            pressed = true;
            return Mouse.current.position.ReadValue();
        }
        return Vector2.zero;
    }

    /// <summary>
    /// Actualiza el contenido del texto.
    /// </summary>
    /// <param name="t">Cadena de texto con el mensaje a mostrar.</param>
    private void SetMessage(string t) { if (message != null) message.text = t; }

    /// <summary>
    /// Reinicia la escenas y  los contadores empiezan de nuevo.
    /// </summary>
    /// <remarks>
    /// Este método destruye todos los <see cref="GameObject"/> que se encuentran en <see cref="spawnObjectsInScene"/>.
    /// </remarks>
    private void ReiniciarEscena()
    {
        foreach (GameObject o in spawnObjectsInScene) if (o != null) Destroy(o);
        spawnObjectsInScene.Clear();
        counterSpawn = 0;
        prefabIndex = 0;
        SetMessage("Presiona para reiniciar.");
    }
}