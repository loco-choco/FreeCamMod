using UnityEngine;
using BepInEx;
using CAMOWA;

namespace FCM
{
    [BepInDependency("locochoco.plugins.CAMOWA", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("Locochoco.OWA.FreeCamMod","Free Cam Mod","1.1.3")]
    [BepInProcess("OuterWilds_Alpha_1_2.exe")]
    public class FreeCamMod : BaseUnityPlugin
    {
        public bool isOnMainMenu = false;
        
        private Transform currentCameraTransform;

        private Transform freeCamTransform;

        private float timeWhenFreezed = 0f;
        bool isTimeFreezed = false;

        void Start()
        {
            FreeCamInputs.InnitFreeCamInputs();
            CreateFreeCam(0);
            SceneLoading.OnSceneLoad += SceneLoading_OnSceneLoad;

            GlobalMessenger<ReferenceFrame>.AddListener("TargetReferenceFrame", OnTargetReferenceFrame);
            GlobalMessenger.AddListener("UntargetReferenceFrame", OnUntargetReferenceFrame);
        }

        private void OnUntargetReferenceFrame()
        {
            if(freeCamTransform != null)
                freeCamTransform.parent = null;
        }

        private void OnTargetReferenceFrame(ReferenceFrame referenceFrame)
        {
            if (freeCamTransform != null)
                freeCamTransform.parent = referenceFrame.GetOWRigidBody().transform;
        }

        private void CreateFreeCam(int sceneID) 
        {
            if (sceneID == 1)
            {
                currentCameraTransform = gameObject.FindWithRequiredTag("MainCamera").transform;
            }
            else
                currentCameraTransform = ((Camera)FindObjectOfType(typeof(Camera))).transform;
            var freeCamGO = new GameObject("FreeCam");
            freeCamTransform = freeCamGO.transform;
            Camera cam = freeCamGO.AddComponent<Camera>();
            cam.enabled = false;
            cam.CopyFrom(currentCameraTransform.GetComponent<Camera>());
        }

        private void SceneLoading_OnSceneLoad(int sceneId)
        {
            isOnMainMenu = sceneId == 0;
            CreateFreeCam(sceneId);

            if (isTimeFreezed)
                Time.timeScale = 1f;
        }
        CameraSpeeds cameraSpeed;
        const float DefaultCameraSpeed = 3f;        

        void Update()
        {
            float deltaTime;
            if (Time.timeScale != 0)
                deltaTime = Time.deltaTime;
            else
                deltaTime = DeltaTimeWhenFreezed();

            if ((OWInput.GetAxisReleased(PlayerCameraInput.toggleFlashlight, -1) || Input.GetKeyUp(KeyCode.CapsLock)) && CanFreeCamBeEnabled())
            {
                if (!IsFreeCamEnable())
                {
                    if (!isOnMainMenu)
                        GlobalMessenger<Camera>.FireEvent("SwitchActiveCamera", freeCamTransform.camera);

                    GlobalMessenger.FireEvent("EnterFreeCamMode");

                    freeCamTransform.position = currentCameraTransform.position;
                    freeCamTransform.rotation = currentCameraTransform.rotation;

                    currentCameraTransform.camera.enabled = false;
                    freeCamTransform.camera.enabled = true;
                }
                else
                {
                    if (Time.timeScale != 1f && !((SettingsMenu)FindObjectsOfType(typeof(SettingsMenu))[0]).enabled)
                        Time.timeScale = 1f;

                    isTimeFreezed = false;

                    if (!isOnMainMenu)
                        GlobalMessenger<Camera>.FireEvent("SwitchActiveCamera", currentCameraTransform.camera);

                    GlobalMessenger.FireEvent("ExitFreeCamMode");
                    currentCameraTransform.camera.enabled = true;
                    freeCamTransform.camera.enabled = false;
                }
            }
            if (IsFreeCamEnable())
            {
                if (OWInput.GetButtonUp(FreeCamInputs.PararTempo))
                {
                    if (Time.timeScale != 0f) //Se o timeScale já não é zero, fazer ele virar zero
                    {
                        isTimeFreezed = true;
                        Time.timeScale = 0f;
                    }
                    //Se estamos no menu, não tem que se preocupar em voltar ao normal, mas se estamos no Solar System, vereficar se o settings menu ta aberto
                    else if (!((SettingsMenu)FindObjectsOfType(typeof(SettingsMenu))[0]).enabled)
                    {
                        Time.timeScale = 1f;
                        isTimeFreezed = false;
                    }
                }
                

                Vector3 rotationInput;

                if (OWInput.GetButton(FreeCamInputs.TrocarRodarHorizontalPorRolar))
                    rotationInput = new Vector3(-OWInput.GetAxis(FreeCamInputs.RodarNaVertical), 0f, -OWInput.GetAxis(FreeCamInputs.RolarOuRodarNaHorizontal));
                else
                    rotationInput = new Vector3(-OWInput.GetAxis(FreeCamInputs.RodarNaVertical), OWInput.GetAxis(FreeCamInputs.RolarOuRodarNaHorizontal), 0f);

                if (OWInput.GetButtonUp(FreeCamInputs.MudarAcelercaoDaCamera))
                    cameraSpeed = (CameraSpeeds)(((int)cameraSpeed + 1) % (int)CameraSpeeds.CameraSpeeds_Size);

                float cameraVelocity = DefaultCameraSpeed;

                if (OWInput.GetButton(FreeCamInputs.AcelerarCamera))
                    cameraVelocity *= GetCameraSpeed(cameraSpeed);

                freeCamTransform.localRotation *=  Quaternion.Euler(rotationInput * 500f * deltaTime);

                Vector3 positionInput = OWInput.GetAxis(FreeCamInputs.MoveFoward) * freeCamTransform.forward + OWInput.GetAxis(FreeCamInputs.MoveRight) * freeCamTransform.right + (OWInput.GetAxis(FreeCamInputs.MoveUp) - OWInput.GetAxis(FreeCamInputs.MoveDown)) * freeCamTransform.up;
                if(Time.timeScale == 0f)
                    positionInput = InputChannels.moveZ.GetAxisRaw() * freeCamTransform.forward + InputChannels.moveX.GetAxisRaw() * freeCamTransform.right + (InputChannels.moveUp.GetAxisRaw() - InputChannels.moveDown.GetAxisRaw()) * freeCamTransform.up;

                freeCamTransform.position += positionInput * cameraVelocity * deltaTime;

                timeWhenFreezed = Time.realtimeSinceStartup;
            }

        }
        float DeltaTimeWhenFreezed()
        {
            return Time.realtimeSinceStartup - timeWhenFreezed;
        }

        bool CanFreeCamBeEnabled() 
        {
            return freeCamTransform != null;
        }
        bool IsFreeCamEnable()
        {
            if(freeCamTransform == null)
                return false;

            return freeCamTransform.GetComponent<Camera>().enabled;
        }

        enum CameraSpeeds : int
        {
            ONE_HUNDRED,
            THREE_HUNDRED,
            NINE_HUNDRED,
            TWENTY_HUNDRED,
            FIFTY,
            ONE_QUARTER,
            CameraSpeeds_Size
        }

        private float GetCameraSpeed(CameraSpeeds speed) 
        {
            switch (speed)
            {
                case CameraSpeeds.ONE_HUNDRED:
                    return 1f;
                case CameraSpeeds.THREE_HUNDRED:
                    return 2f;
                case CameraSpeeds.NINE_HUNDRED:
                    return 9f;
                case CameraSpeeds.TWENTY_HUNDRED:
                    return 20f;
                case CameraSpeeds.FIFTY:
                    return 0.5f;
                case CameraSpeeds.ONE_QUARTER:
                    return 0.25f;
                default:
                    return 1f;
            }
        }

    } 
}

