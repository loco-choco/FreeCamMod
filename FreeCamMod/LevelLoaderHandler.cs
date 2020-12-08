//using CAMOWA;
//using UnityEngine;

//namespace FCM
//{
//    public class LevelLoaderHandler : MonoBehaviour
//    {

//        static private bool levelLoaderCreated = false;
//        private int levelIndex = -1;
//        [IMOWAModInnit("Free Cam Mod", -1, 2)]
//        public static void ModInnit(string porOndeTaInicializando)
//        {
//            if (!levelLoaderCreated)
//            {
//                Debug.Log("FreeCamMod foi iniciado em " + porOndeTaInicializando);
//                new GameObject("FreeCamLevelLoaderHandler").AddComponent<LevelLoaderHandler>(); // Será que vai funcionar?
//                Debug.Log("O script do mod foi colocado na 'MainCamera' ");

//                levelLoaderCreated = !levelLoaderCreated;
//            }

//        }

//        void Awake()
//        {
//            DontDestroyOnLoad(gameObject);
//        }

//        //Ideia, no lugar de fazer patch no Assembly do jogo e ter chance de dar ruim, fazer com que isso ocorra nessa classe, como separação de
//        //prioridade e em qual level o start vai ocorrer
//        //Se a pessoa quer que o script dela ocorra antes de algum Awake ou sla, ela faz usando Harmony e tals

//        void MainMenuStart()
//        {
//            Debug.Log("MainMenuStart");
//            Debug.Log("High Priority Mod");
//            Debug.Log("Regular Priority Mod");
//            Debug.Log("Low Priority Mod");
//        }

//        void SolarSystemStart()
//        {
//            Debug.Log("SolarSystemStart");
//            Debug.Log("High Priority Mod");
//            Debug.Log("Regular Priority Mod");
//            Debug.Log("Low Priority Mod");
//        }
//        void AllLevelStart()
//        {
//            Debug.Log("AllLevelStart");
//            Debug.Log("High Priority Mod");
//            Debug.Log("Regular Priority Mod");
//            Debug.Log("Low Priority Mod");
//            new GameObject("FreeCamModCamera").AddComponent<FreeCamMod>();
//        }

//        void Update()
//        {
//            if (levelIndex != Application.loadedLevel)
//            {
//                AllLevelStart();
//                levelIndex = Application.loadedLevel;
//                if (levelIndex == 0)
//                    MainMenuStart();
//                else if (levelIndex == 1)
//                    SolarSystemStart();

//            }

//        }




//    }
//}
