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
        HoldInteractProgressUI _holdProgressUi;
        WoodTree _holdTree;
        float _holdElapsed;
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
            _holdProgressUi = FindFirstObjectByType<HoldInteractProgressUI>();
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

            if (Mouse.current.leftButton.wasPressedThisFrame
                && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            HandleTreeHoldHarvest();

            HandleLook();
            HandleMove();
        }

        void HandleTreeHoldHarvest()
        {
            ResolveCamera();
            if (_camera == null || inventory == null)
            {
                ResetTreeHold();
                return;
            }

            if (Cursor.lockState != CursorLockMode.Locked || Cursor.visible)
            {
                ResetTreeHold();
                return;
            }

            bool holding = Mouse.current.leftButton.isPressed;
            bool hasTree = InteractionQueries.TryGetWoodTree(_camera, harvestReach, out WoodTree tree);

            if (!holding || !hasTree)
            {
                ResetTreeHold();
                return;
            }

            if (_holdTree != tree)
            {
                _holdTree = tree;
                _holdElapsed = 0f;
            }

            float duration = Mathf.Max(0.01f, tree.HarvestHoldSeconds);
            _holdElapsed += Time.deltaTime;
            float normalized = Mathf.Clamp01(_holdElapsed / duration);

            if (_holdProgressUi != null)
                _holdProgressUi.SetProgress(normalized, true);

            if (normalized < 1f)
                return;

            tree.CompleteHarvest(inventory);
            ResetTreeHold();
        }

        void ResetTreeHold()
        {
            _holdTree = null;
            _holdElapsed = 0f;
            if (_holdProgressUi != null)
                _holdProgressUi.SetProgress(0f, false);
        }

        void ResolveCamera()
        {
            if (_camera != null)
                return;

            if (cameraTransform != null)
                _camera = cameraTransform.GetComponent<Camera>();

            if (_camera == null)
                _camera = GetComponentInChildren<Camera>();

            if (_camera == null)
                _camera = Camera.main;
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
