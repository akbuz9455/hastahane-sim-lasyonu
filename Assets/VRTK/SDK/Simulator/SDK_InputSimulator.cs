// VR Simulator|Prefabs|0005
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The `VRSimulatorCameraRig` prefab is a mock Camera Rig set up that can be used to develop with VRTK without the need for VR Hardware.
    /// </summary>
    /// <remarks>
    /// Use the mouse and keyboard to move around both play area and hands and interacting with objects without the need of a hmd or VR controls.
    /// </remarks>
    public class SDK_InputSimulator : MonoBehaviour
    {
        /// <summary>
        /// Mouse input mode types
        /// </summary>
        /// <param name="Always">Mouse movement is always treated as mouse input.</param>
        /// <param name="RequiresButtonPress">Mouse movement is only treated as movement when a button is pressed.</param>
        public enum MouseInputMode
        {
            Always,
            RequiresButtonPress
        }

        #region Public fields

        [Tooltip("Kontrol bilgilerini ekranın sol üst köşesinde göster.")]
        public bool showControlHints = true;
        [Tooltip("Devre dışı bırakırken ellerinizi gizleyin.")]
        public bool hideHandsAtSwitch = false;
        [Tooltip("El pozisyonunu ve dönüşünü etkinleştirirken sıfırlayın.")]
        public bool resetHandsAtSwitch = true;
        [Tooltip("Fare hareketinin her zaman girdi işlevi görüp görmediği veya bir düğmeye basılması gerekip gerekmediği.")]
        public MouseInputMode mouseMovementInput = MouseInputMode.Always;
        [Tooltip("Fare hareket tuşuna basıldığında fare imlecini oyun penceresine kilitleyin.")]
        public bool lockMouseToView = true;

        [Header("Ayarlamalar")]

        [Tooltip("El hareket hızını ayarlayın.")]
        public float handMoveMultiplier = 0.002f;
        [Tooltip("El dönüş hızını ayarlayın.")]
        public float handRotationMultiplier = 0.5f;
        [Tooltip("Oyuncu hareket hızını ayarlayın.")]
        public float playerMoveMultiplier = 5;
        [Tooltip("Oyuncu dönüş hızını ayarlayın.")]
        public float playerRotationMultiplier = 0.5f;
        [Tooltip("Oyuncu sprint hızını ayarlayın.")]
        public float playerSprintMultiplier = 2;

        [Header("İşlem Anahtar Bağlantıları")]

        [Tooltip("Bir düğmeye basılması gerektiğinde fare girişini etkinleştirmek için kullanılan tuş.")]
        public KeyCode mouseMovementKey = KeyCode.Mouse1;
        [Tooltip("Kontrol ipuçlarını açmak/kapatmak için kullanılan tuş.")]
        public KeyCode toggleControlHints = KeyCode.F1;
        [Tooltip("Sağ ve sol el arasında geçiş yapmak için kullanılan tuş.")]
        public KeyCode changeHands = KeyCode.Tab;
        [Tooltip("Elleri Açmak/Kapatmak için kullanılan tuş.")]
        public KeyCode handsOnOff = KeyCode.LeftAlt;
        [Tooltip("Konumsal ve rotasyonel hareket arasında geçiş yapmak için kullanılan tuş.")]
        public KeyCode rotationPosition = KeyCode.LeftShift;
        [Tooltip("X/Y ve X/Z ekseni arasında geçiş yapmak için kullanılan tuş.")]
        public KeyCode changeAxis = KeyCode.LeftControl;
        [Tooltip("Sol elle mesafe almak için kullanılan anahtar.")]
        public KeyCode distancePickupLeft = KeyCode.Mouse0;
        [Tooltip("Sağ el ile mesafe almak için kullanılan anahtar.")]
        public KeyCode distancePickupRight = KeyCode.Mouse1;
        [Tooltip("Mesafe alımını etkinleştirmek için kullanılan anahtar.")]
        public KeyCode distancePickupModifier = KeyCode.LeftControl;

        [Header("Hareket Tuşu Bağlantıları")]

        [Tooltip("İlerlemek için kullanılan tuş.")]
        public KeyCode moveForward = KeyCode.W;
        [Tooltip("Sola gitmek için kullanılan tuş.")]
        public KeyCode moveLeft = KeyCode.A;
        [Tooltip("Geri gitmek için kullanılan tuş.")]
        public KeyCode moveBackward = KeyCode.S;
        [Tooltip("Sağa hareket etmek için kullanılan tuş.")]
        public KeyCode moveRight = KeyCode.D;
        [Tooltip("Koşmak için kullanılan anahtar.")]
        public KeyCode sprint = KeyCode.LeftShift;

        [Header("Denetleyici Anahtar Bağlantıları")]
        [Tooltip("Tetik düğmesini simüle etmek için kullanılan anahtar.")]
        public KeyCode triggerAlias = KeyCode.Mouse1;
        [Tooltip("Kavrama düğmesini simüle etmek için kullanılan tuş.")]
        public KeyCode gripAlias = KeyCode.Mouse0;
        [Tooltip("Dokunmatik yüzey düğmesini simüle etmek için kullanılan tuş.")]
        public KeyCode touchpadAlias = KeyCode.Q;
        [Tooltip("Birinci düğmeyi simüle etmek için kullanılan anahtar.")]
        public KeyCode buttonOneAlias = KeyCode.E;
        [Tooltip("İkinci düğmeyi simüle etmek için kullanılan tuş.")]
        public KeyCode buttonTwoAlias = KeyCode.R;
        [Tooltip("Başlat menüsü düğmesini simüle etmek için kullanılan tuş.")]
        public KeyCode startMenuAlias = KeyCode.F;
        [Tooltip("Düğmeye dokunma ve düğmeye basma modu arasında geçiş yapmak için kullanılan tuş.")]
        public KeyCode touchModifier = KeyCode.T;
        [Tooltip("Saça dokunma modu arasında geçiş yapmak için kullanılan tuş.")]
        public KeyCode hairTouchModifier = KeyCode.H;

        #endregion
        #region Private fields

        private bool isHand = false;
        private GameObject hintCanvas;
        private Text hintText;
        private Transform rightHand;
        private Transform leftHand;
        private Transform currentHand;
        private Vector3 oldPos;
        private Transform neck;
        private SDK_ControllerSim rightController;
        private SDK_ControllerSim leftController;
        private static GameObject cachedCameraRig;
        private static bool destroyed = false;
        private float sprintMultiplier = 1;
        private GameObject crossHairPanel;

        #endregion

        /// <summary>
        /// The FindInScene method is used to find the `VRSimulatorCameraRig` GameObject within the current scene.
        /// </summary>
        /// <returns>Returns the found `VRSimulatorCameraRig` GameObject if it is found. If it is not found then it prints a debug log error.</returns>
        public static GameObject FindInScene()
        {
            if (cachedCameraRig == null && !destroyed)
            {
                cachedCameraRig = VRTK_SharedMethods.FindEvenInactiveGameObject<SDK_InputSimulator>();
                if (!cachedCameraRig)
                {
                    VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRSimulatorCameraRig", "SDK_InputSimulator", ". check that the `VRTK/Prefabs/VRSimulatorCameraRig` prefab been added to the scene."));
                }
            }
            return cachedCameraRig;
        }

        private void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        private void OnEnable()
        {
            hintCanvas = transform.Find("Canvas/Control Hints").gameObject;
            crossHairPanel = transform.Find("Canvas/CrosshairPanel").gameObject;
            hintText = hintCanvas.GetComponentInChildren<Text>();
            hintCanvas.SetActive(showControlHints);
            rightHand = transform.Find("RightHand");
            rightHand.gameObject.SetActive(false);
            leftHand = transform.Find("LeftHand");
            leftHand.gameObject.SetActive(false);
            currentHand = rightHand;
            oldPos = Input.mousePosition;
            neck = transform.Find("Neck");
            leftHand.Find("Hand").GetComponent<Renderer>().material.color = Color.red;
            rightHand.Find("Hand").GetComponent<Renderer>().material.color = Color.green;
            rightController = rightHand.GetComponent<SDK_ControllerSim>();
            leftController = leftHand.GetComponent<SDK_ControllerSim>();
            rightController.Selected = true;
            leftController.Selected = false;
            destroyed = false;

            var controllerSDK = VRTK_SDK_Bridge.GetControllerSDK() as SDK_SimController;
            if (controllerSDK != null)
            {
                Dictionary<string, KeyCode> keyMappings = new Dictionary<string, KeyCode>()
                {
                    {"Trigger", triggerAlias },
                    {"Grip", gripAlias },
                    {"TouchpadPress", touchpadAlias },
                    {"ButtonOne", buttonOneAlias },
                    {"ButtonTwo", buttonTwoAlias },
                    {"StartMenu", startMenuAlias },
                    {"TouchModifier", touchModifier },
                    {"HairTouchModifier", hairTouchModifier }
                };
                controllerSDK.SetKeyMappings(keyMappings);
            }
            rightHand.gameObject.SetActive(true);
            leftHand.gameObject.SetActive(true);
            crossHairPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
            destroyed = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleControlHints))
            {
                showControlHints = !showControlHints;
                hintCanvas.SetActive(showControlHints);
            }

            if (mouseMovementInput == MouseInputMode.RequiresButtonPress)
            {
                if (lockMouseToView)
                {
                    Cursor.lockState = Input.GetKey(mouseMovementKey) ? CursorLockMode.Locked : CursorLockMode.None;
                }
                else if (Input.GetKeyDown(mouseMovementKey))
                {
                    oldPos = Input.mousePosition;
                }
            }

            if (Input.GetKeyDown(handsOnOff))
            {
                if (isHand)
                {
                    SetMove();
                }
                else
                {
                    SetHand();
                }
            }

            if (Input.GetKeyDown(changeHands))
            {
                if (currentHand.name == "LeftHand")
                {
                    currentHand = rightHand;
                    rightController.Selected = true;
                    leftController.Selected = false;
                }
                else
                {
                    currentHand = leftHand;
                    rightController.Selected = false;
                    leftController.Selected = true;
                }
            }

            if (isHand)
            {
                UpdateHands();
            }
            else
            {
                UpdateRotation();
                if(Input.GetKeyDown(distancePickupRight) && Input.GetKey(distancePickupModifier))
                {
                    TryPickup(true);
                }
                else if(Input.GetKeyDown(distancePickupLeft) && Input.GetKey(distancePickupModifier))
                {
                    TryPickup(false);
                }
                if(Input.GetKey(sprint))
                {
                    sprintMultiplier = playerSprintMultiplier;
                }
                else
                {
                    sprintMultiplier = 1;
                }
                if(Input.GetKeyDown(distancePickupModifier))
                {
                    crossHairPanel.SetActive(true);
                }
                else if(Input.GetKeyUp(distancePickupModifier))
                {
                    crossHairPanel.SetActive(false);
                }
            }

            UpdatePosition();

            if (showControlHints)
            {
                UpdateHints();
            }
        }

        private void TryPickup(bool rightHand)
        {
            Ray screenRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            if(Physics.Raycast(screenRay, out hit))
            {
                VRTK_InteractableObject io = hit.collider.gameObject.GetComponent<VRTK_InteractableObject>();
                if(io)
                {
                    GameObject hand;
                    if(rightHand)
                    {
                        hand = VRTK_DeviceFinder.GetControllerRightHand();
                    }
                    else
                    {
                        hand = VRTK_DeviceFinder.GetControllerLeftHand();
                    }
                    VRTK_InteractGrab grab = hand.GetComponent<VRTK_InteractGrab>();
                    if(grab.GetGrabbedObject() == null)
                    {
                        hand.GetComponent<VRTK_InteractTouch>().ForceTouch(hit.collider.gameObject);
                        grab.AttemptGrab();
                    }
                }
            }
        }

        private void UpdateHands()
        {
            Vector3 mouseDiff = GetMouseDelta();

            if (IsAcceptingMouseInput())
            {
                if (Input.GetKey(rotationPosition)) //Rotation
                {
                    if (Input.GetKey(changeAxis))
                    {
                        Vector3 rot = Vector3.zero;
                        rot.x += (mouseDiff * handRotationMultiplier).y;
                        rot.y += (mouseDiff * handRotationMultiplier).x;
                        currentHand.transform.Rotate(rot * Time.deltaTime);
                    }
                    else
                    {
                        Vector3 rot = Vector3.zero;
                        rot.z += (mouseDiff * handRotationMultiplier).x;
                        rot.x += (mouseDiff * handRotationMultiplier).y;
                        currentHand.transform.Rotate(rot * Time.deltaTime);
                    }
                }
                else //Position
                {
                    if (Input.GetKey(changeAxis))
                    {
                        Vector3 pos = Vector3.zero;
                        pos += mouseDiff * handMoveMultiplier;
                        currentHand.transform.Translate(pos * Time.deltaTime);
                    }
                    else
                    {
                        Vector3 pos = Vector3.zero;
                        pos.x += (mouseDiff * handMoveMultiplier).x;
                        pos.z += (mouseDiff * handMoveMultiplier).y;
                        currentHand.transform.Translate(pos * Time.deltaTime);
                    }
                }
            }
        }

        private void UpdateRotation()
        {
            Vector3 mouseDiff = GetMouseDelta();

            if (IsAcceptingMouseInput())
            {
                Vector3 rot = transform.localRotation.eulerAngles;
                rot.y += (mouseDiff * playerRotationMultiplier).x;
                transform.localRotation = Quaternion.Euler(rot);

                rot = neck.rotation.eulerAngles;

                if (rot.x > 180)
                {
                    rot.x -= 360;
                }

                if (rot.x < 80 && rot.x > -80)
                {
                    rot.x += (mouseDiff * playerRotationMultiplier).y * -1;
                    rot.x = Mathf.Clamp(rot.x, -79, 79);
                    neck.rotation = Quaternion.Euler(rot);
                }
            }
        }

        private void UpdatePosition()
        {
            float moveMod = Time.deltaTime * playerMoveMultiplier * sprintMultiplier;
            if (Input.GetKey(moveForward))
            {
                transform.Translate(transform.forward * moveMod, Space.World);
            }
            else if (Input.GetKey(moveBackward))
            {
                transform.Translate(-transform.forward * moveMod, Space.World);
            }
            if (Input.GetKey(moveLeft))
            {
                transform.Translate(-transform.right * moveMod, Space.World);
            }
            else if (Input.GetKey(moveRight))
            {
                transform.Translate(transform.right * moveMod, Space.World);
            }
        }

        private void SetHand()
        {
            Cursor.visible = false;
            isHand = true;
            rightHand.gameObject.SetActive(true);
            leftHand.gameObject.SetActive(true);
            oldPos = Input.mousePosition;
            if (resetHandsAtSwitch)
            {
                rightHand.transform.localPosition = new Vector3(0.2f, 1.2f, 0.5f);
                rightHand.transform.localRotation = Quaternion.identity;
                leftHand.transform.localPosition = new Vector3(-0.2f, 1.2f, 0.5f);
                leftHand.transform.localRotation = Quaternion.identity;
            }
        }

        private void SetMove()
        {
            Cursor.visible = true;
            isHand = false;
            if (hideHandsAtSwitch)
            {
                rightHand.gameObject.SetActive(false);
                leftHand.gameObject.SetActive(false);
            }
        }

        private void UpdateHints()
        {
            string hints = "";
            Func<KeyCode, string> key = (k) => "<b>" + k.ToString() + "</b>";

            string mouseInputRequires = "";
            if (mouseMovementInput == MouseInputMode.RequiresButtonPress)
            {
                mouseInputRequires = " (" + key(mouseMovementKey) + ")";
            }

            // WASD Movement
            string movementKeys = moveForward.ToString() + moveLeft.ToString() + moveBackward.ToString() + moveRight.ToString();
            hints += "Kontrol İpuçlarını Aç/Kapat: " + key(toggleControlHints) + "\n\n";
            hints += "Karakter Hareket: <b>" + movementKeys + "</b>\n";
            hints += "Konumsal/Pozisyonel Hareket Değiştirici (" + key(sprint) + ")\n\n";

            if (isHand)
            {
                // Controllers
                if (Input.GetKey(rotationPosition))
                {
                    hints += "Mouse: <b>Rotasyon Kontrolü" + mouseInputRequires + "</b>\n";
                }
                else
                {
                    hints += "Mouse: <b>Pozisyon Kontrolü" + mouseInputRequires + "</b>\n";
                }
                hints += "Mod: HMD (" + key(handsOnOff) + "), Rotasyon (" + key(rotationPosition) + ")\n";

                hints += "Denetleyici El: " + currentHand.name.Replace("Hand", "") + " (" + key(changeHands) + ")\n";

                string axis = Input.GetKey(changeAxis) ? "X/Y" : "X/Z";
                hints += "Eksen: " + axis + " (" + key(changeAxis) + ")\n";

                // Controller Buttons
                string pressMode = "Basmak";
                if (Input.GetKey(hairTouchModifier))
                {
                    pressMode = "Saç Dokunuşu";
                }
                else if (Input.GetKey(touchModifier))
                {
                    pressMode = "Dokunmak";
                }

                hints += "\nButona Basma Modu Değiştiricileri: Dokunmak (" + key(touchModifier) + "), Saç Dokunuşu (" + key(hairTouchModifier) + ")\n";

                hints += "Tetikleyerek " + pressMode + ": " + key(triggerAlias) + "\n";
                hints += "Kavrayarak " + pressMode + ": " + key(gripAlias) + "\n";
                if (!Input.GetKey(hairTouchModifier))
                {
                    hints += "Dokunmatik yüzeye " + pressMode + ": " + key(touchpadAlias) + "\n";
                    hints += "Buton Bire" + pressMode + ": " + key(buttonOneAlias) + "\n";
                    hints += "Button İkiye " + pressMode + ": " + key(buttonTwoAlias) + "\n";
                    hints += "Başlangıç Menüsü " + pressMode + ": " + key(startMenuAlias) + "\n";
                }
            }
            else
            {
                // HMD Input
                hints += "Mouse: <b>HMD Rotasyon" + mouseInputRequires + "</b>\n";
                hints += "Mod: Kontrol (" + key(handsOnOff) + ")\n";
                hints += "Mesafeli Yakalama: (" + key(distancePickupModifier) + ")\n";
                hints += "Sol El Mesafeli Yakalama: (" + key(distancePickupLeft) + ")\n";
                hints += "Sağ El Mesafeli Yakalama: (" + key(distancePickupRight) + ")\n";
            }

            hintText.text = hints.TrimEnd();
        }

        private bool IsAcceptingMouseInput()
        {
            return mouseMovementInput == MouseInputMode.Always || Input.GetKey(mouseMovementKey);
        }

        private Vector3 GetMouseDelta()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                return new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
            else
            {
                Vector3 mouseDiff = Input.mousePosition - oldPos;
                oldPos = Input.mousePosition;
                return mouseDiff;
            }
        }
    }
}
