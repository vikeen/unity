using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
        [Header("Movement")]
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float sprintMultiplier = 1.5f;
        [SerializeField] float jumpHeight = 1.2f;
        [SerializeField] float gravity = -20f;

        [Header("Look")]
        [SerializeField] Transform cameraTransform;
        [SerializeField] float lookSensitivity = 0.15f;
        [SerializeField] float maxPitch = 85f;

        [Header("Harvest")]
        [SerializeField] PlayerInventory inventory;
        [SerializeField] float harvestReach = 5f;

        CharacterController _controller;
        Camera _camera;
        InventoryPanelUI _inventoryPanel;
        float _verticalVelocity;
        float _pitch;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (inventory == null)
                inventory = GetComponent<PlayerInventory>();

            if (cameraTransform == null)
            {
                Camera cam = GetComponentInChildren<Camera>();
                if (cam != null)
                {
                    cameraTransform = cam.transform;
                    _camera = cam;
                }
            }
            else
            {
                _camera = cameraTransform.GetComponent<Camera>();
            }

            _inventoryPanel = FindFirstObjectByType<InventoryPanelUI>();
        }

        void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void Update()
        {
            if (Keyboard.current == null || Mouse.current == null)
                return;

            if (_inventoryPanel != null && _inventoryPanel.IsOpen)
                return;

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    TryHarvestTree();
                }
            }

            HandleLook();
            HandleMove();
        }

        void TryHarvestTree()
        {
            if (_camera == null || inventory == null)
                return;

            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, harvestReach))
                return;

            if (hit.collider.GetComponentInParent<WoodTree>() == null)
                return;

            inventory.AddWood(1);
        }

        void HandleLook()
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            transform.Rotate(Vector3.up, delta.x * lookSensitivity);

            _pitch -= delta.y * lookSensitivity;
            _pitch = Mathf.Clamp(_pitch, -maxPitch, maxPitch);
            if (cameraTransform != null)
                cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        void HandleMove()
        {
            float h = 0f;
            float v = 0f;
            if (Keyboard.current.wKey.isPressed) v += 1f;
            if (Keyboard.current.sKey.isPressed) v -= 1f;
            if (Keyboard.current.dKey.isPressed) h += 1f;
            if (Keyboard.current.aKey.isPressed) h -= 1f;

            Vector3 horizontal = transform.right * h + transform.forward * v;
            if (horizontal.sqrMagnitude > 1f)
                horizontal.Normalize();

            float speed = moveSpeed;
            if (Keyboard.current.leftShiftKey.isPressed)
                speed *= sprintMultiplier;

            if (_controller.isGrounded)
            {
                if (_verticalVelocity < 0f)
                    _verticalVelocity = -2f;
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            _verticalVelocity += gravity * Time.deltaTime;

            Vector3 motion = horizontal * speed;
            motion.y = _verticalVelocity;
            _controller.Move(motion * Time.deltaTime);
        }
}
