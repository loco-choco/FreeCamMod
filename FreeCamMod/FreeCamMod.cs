using UnityEngine;


namespace FCM
{

    public class FreeCamMod : MonoBehaviour
    {
        

        private GameObject freeCamObject;
        private float freeCamRotationX = 0f;
        private float freeCamRotationAround = 0f;
        private Vector3 freeCamPosition;
        
        private Transform currentCameraTransform;
        
        private Transform playerTransform;
        private PlayerLockOnTargeting playerCamController;
        private GameObject playerLockingViewOn;

        private bool rotateWithTheCamera = true;

        
        public static void  ModInnit(string porOndeTaInicializando)
        {
            
            Debug.Log("FreeCamMod foi iniciado em "+ porOndeTaInicializando);
            GameObject.FindGameObjectsWithTag("MainCamera")[0].AddComponent<FreeCamMod>();
            Debug.Log("O script do mod foi colocado na 'MainCamera' ");

        }

        void Start()
        {
           
            currentCameraTransform = gameObject.FindWithRequiredTag("MainCamera").transform;

            
            playerTransform = gameObject.FindWithRequiredTag("Player").transform;
            playerCamController = playerTransform.GetRequiredComponentInChildren<PlayerLockOnTargeting>();
            

            freeCamObject = new GameObject("FreeCamModCamera");
            
            freeCamObject.AddComponent<Camera>();
            freeCamObject.camera.enabled = false;
            freeCamObject.camera.CopyFrom(currentCameraTransform.camera);

            playerLockingViewOn = new GameObject("FreeCamModViewLock");
			playerLockingViewOn.transform.position = currentCameraTransform.position + currentCameraTransform.forward;

            freeCamPosition = Vector3.zero;

            



        }

       

        void Update()
        {

            //Para ele não ficar "deslizando" pelo chão
            if (playerTransform.gameObject.GetComponent<CharacterMovementModel>().IsGrounded() && IsFreeCamEnable())
            {
                playerTransform.gameObject.GetComponent<CharacterMovementModel>().LockMovement();
            }

            if (Input.GetKeyDown(KeyCode.CapsLock))
            {
                if (!IsFreeCamEnable())
                {
                    
                    Debug.Log("Congelando o mov. do player");
                        
                    playerTransform.gameObject.GetComponent<CharacterMovementModel>().LockMovement();
                    playerCamController.LockOn(playerLockingViewOn.transform);
                    

                    Debug.Log("Coisas magicas");
                    currentCameraTransform.camera.enabled = false;
                    freeCamObject.camera.enabled = true;

                    
                    

             
                }

                else
                {
                    Debug.Log("Descongelando o mov. do player");
                    playerTransform.gameObject.GetComponent<CharacterMovementModel>().UnlockMovement();
                    playerCamController.BreakLock();
                    
                    Debug.Log("Mais coisas magicas");
                    currentCameraTransform.camera.enabled = true;
                    freeCamObject.camera.enabled = false;

                    freeCamPosition = Vector3.zero;
                    freeCamRotationAround = 0f;
                    freeCamRotationX = 0f;

                    
                   
                }
            }


            float cameraSpeed = 3f;
            

            if (IsFreeCamEnable())
            {
                

                //Cuidar da aceleração e veloc. da camera
                
                if (Input.GetKey(KeyCode.Tab))
                    cameraSpeed = 9f;
                
                else
                    cameraSpeed = 3f;


                //Direção q a camera vai virar
                freeCamRotationAround += PlayerCameraInput.lookX.GetInput() * Time.deltaTime * 500f%360;
                freeCamRotationX -= PlayerCameraInput.lookY.GetInput() * Time.deltaTime * 500f;
                

                
                

                freeCamRotationX = Mathf.Clamp(freeCamRotationX, -90f, 90f);

                if (rotateWithTheCamera)
                    freeCamObject.transform.rotation = currentCameraTransform.rotation;
                else
                    freeCamObject.transform.eulerAngles = Vector3.zero;

                freeCamObject.transform.rotation = freeCamObject.transform.rotation *  Quaternion.Euler(freeCamRotationAround * playerLockingViewOn.transform.up) * Quaternion.Euler(freeCamRotationX, 0f, 0f);




                //Direção q a camera vai andar
                freeCamObject.transform.position = currentCameraTransform.position;

                if (Input.GetKey(KeyCode.W))
                    freeCamPosition += freeCamObject.transform.forward * cameraSpeed * Time.deltaTime;

                if (Input.GetKey(KeyCode.S))
                    freeCamPosition += -freeCamObject.transform.forward * cameraSpeed * Time.deltaTime;

                if (Input.GetKey(KeyCode.D))
                    freeCamPosition += freeCamObject.transform.right * cameraSpeed * Time.deltaTime;

                if (Input.GetKey(KeyCode.A))
                    freeCamPosition += -freeCamObject.transform.right * cameraSpeed * Time.deltaTime;

                freeCamObject.transform.position += freeCamPosition;
                //Fazer com q isso sempre esteja uma unidade afrente da camera principal

                playerLockingViewOn.transform.position = currentCameraTransform.position + currentCameraTransform.forward;



            }

        }

        
        

        bool IsFreeCamEnable()
        {
            return freeCamObject.camera.enabled;
        }

    } 
}

