using UnityEngine;
using UnityEngine.EventSystems;

namespace Skyloft.UI
{
    /// <summary>
    /// Mobile virtual joystick that processes touch and mouse pointer drag events.
    /// Returns a clamped, analog Vector2 input for movement.
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("UI References")]
        [SerializeField] private RectTransform containerRect;
        [SerializeField] private RectTransform handleRect;

        [Header("Joystick Settings")]
        [Tooltip("Max radius in pixels that the handle can travel from center.")]
        [SerializeField] private float handleRange = 80f;

        [Tooltip("Dead zone threshold below which input is ignored (0..0.5).")]
        [Range(0f, 0.5f)]
        [SerializeField] private float deadZone = 0.05f;

        private Canvas canvas;
        private Camera targetCamera;

        public Vector2 MoveInput { get; private set; }

        private void Awake()
        {
            if (containerRect == null)
            {
                containerRect = GetComponent<RectTransform>();
            }

            canvas = GetComponentInParent<Canvas>();
            UpdateTargetCamera();
        }

        private void Start()
        {
            UpdateTargetCamera();
        }

        private void UpdateTargetCamera()
        {
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                targetCamera = canvas.worldCamera;
            }
            else
            {
                targetCamera = null;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (containerRect == null || handleRect == null)
            {
                return;
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    containerRect,
                    eventData.position,
                    targetCamera,
                    out Vector2 localPoint))
            {
                float radius = handleRange > 0f ? handleRange : (containerRect.rect.width * 0.5f);
                Vector2 clamped = Vector2.ClampMagnitude(localPoint, radius);
                handleRect.anchoredPosition = clamped;

                Vector2 rawInput = clamped / radius;
                float magnitude = rawInput.magnitude;

                if (magnitude < deadZone)
                {
                    MoveInput = Vector2.zero;
                }
                else
                {
                    // Smoothly remap from [deadZone..1] to [0..1] to prevent sudden input jumps
                    float normalizedMag = (magnitude - deadZone) / (1f - deadZone);
                    MoveInput = rawInput.normalized * normalizedMag;
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            MoveInput = Vector2.zero;
            if (handleRect != null)
            {
                handleRect.anchoredPosition = Vector2.zero;
            }
        }

        private void OnValidate()
        {
            handleRange = Mathf.Max(10f, handleRange);
            deadZone = Mathf.Clamp(deadZone, 0f, 0.5f);
        }
    }
}
