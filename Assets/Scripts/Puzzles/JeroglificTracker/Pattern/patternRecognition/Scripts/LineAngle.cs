using PDollarGestureRecognizer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class LineAngle : MonoBehaviour
{
    public bool showGizmos;

    public bool keepAngleOnFlip;

    public Animator myAnimator;

    public Transform PlayerTransform;

    public string animatorVariableName;

    public float angle;

    public Vector3 AngleOrigin;

    public void TakeAngleOrigin(Vector2 Origin)
    {
        AngleOrigin = new(Origin.x, Origin.y, PlayerTransform.position.z);
    }

    //private Vector3 _mouseDir;
    private Vector3 _mouseWorldPos;
    private Vector3 _mouseScreenPos;

    public UnityEvent<float> MouseAngleEvent;


    public void SendAngleInformation(Result Result)
    {
        if (Result.GestureID == 1)
        {
            MouseAngleEvent?.Invoke(angle);
            myAnimator.SetFloat(animatorVariableName, angle);
        }
    }

    public void Update()
    {
        if (PlayerTransform == null) return;
        _mouseScreenPos = Input.mousePosition;
        // Obtener la posición del mouse en el mundo ajustando el plano Z
        _mouseWorldPos = GetMouseWorldPosition();
        _mouseWorldPos.z = PlayerTransform.position.z; // Igualar el Z del objeto

        if (!keepAngleOnFlip)
        {

            Vector2 directionToMouse = (_mouseWorldPos - transform.position).normalized;

            if (PlayerTransform.right.x < 0)
            {
                // Calcular el ángulo entre transform.right y la dirección hacia el mouse
                angle = -Vector2.SignedAngle(PlayerTransform.right, directionToMouse);
            }
            else
            {

                // Calcular el ángulo entre transform.right y la dirección hacia el mouse
                angle = Vector2.SignedAngle(PlayerTransform.right, directionToMouse);
            }
        }
        else
        {
            // Calcular el vector desde el objeto hacia el mouse
            Vector2 directionToMouse = (_mouseWorldPos - AngleOrigin).normalized;

            // Calcular el ángulo con respecto a transform.right
            angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

            // Ajustar el ángulo para que sea relativo a la rotación del objeto
            angle -= PlayerTransform.eulerAngles.z;
        }
        // MouseAngleEvent?.Invoke(angle);
        if (myAnimator == null) return;
        // myAnimator.SetFloat(animatorVariableName, angle);

    }
    Vector3 GetMouseWorldPosition()
    {
        // Crear un rayo desde la posición del mouse en pantalla
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Definir un plano en la profundidad deseada (en este caso, Z = planeZ)
        Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, 0));

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    public void OnDrawGizmos()
    {
        if (!showGizmos) return;

        if (!Application.isPlaying) return;

        // Dibujar la línea desde el objeto hacia el mouse
        Gizmos.color = Color.green;

        var _mouseDir = (_mouseWorldPos - AngleOrigin).normalized;

        Gizmos.DrawRay(AngleOrigin, _mouseDir * 5);



        // Dibujar la línea de referencia del transform.right
        Gizmos.color = Color.red;
        Gizmos.DrawLine(AngleOrigin, AngleOrigin + PlayerTransform.right * 5);

        // Dibujar un círculo en la posición del mouse
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_mouseWorldPos, 0.1f);

        // Dibujar el ángulo como texto en la posición del mouse
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 16;
        Handles.Label(_mouseWorldPos, $"Ángulo: {angle:F2}°", style);
    }


}
