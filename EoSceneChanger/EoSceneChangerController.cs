using Newtonsoft.Json;
using UnityEngine;

using BS_Utils.Utilities;

namespace EoSceneChanger {
    public class EoSceneChangerController : MonoBehaviour
    {
        public static EoSceneChangerController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this);
            Instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");
        }

        private void Start()
        {
            // Research phases, things are ugly and temporary
            // TODO: Move these lambdas to actual callback style methods
            BSEvents.gameSceneLoaded += () =>
            {
                EoJSON.OBSRequest rqst = new EoJSON.OBSRequest(6, "SetCurrentProgramScene", "GameScene");
                Plugin.OBS_WS.SendAsync(JsonConvert.SerializeObject(rqst), null);
            };
            BSEvents.menuSceneLoaded += () =>
            {
                EoJSON.OBSRequest rqst = new EoJSON.OBSRequest(6, "SetCurrentProgramScene", "MenuScene");
                Plugin.OBS_WS.SendAsync(JsonConvert.SerializeObject(rqst), null);
            };

            Plugin.BHS_WS.OnMessage += (sender, e) =>
            {
                Plugin.Log.Info("HTTP WS EVENT! " + e.Data);
            };

            Plugin.OBS_WS.OnOpen += (sender, e) => {
                Plugin.Log.Debug("Socket open, identifying");
                Plugin.OBS_WS.SendAsync("{\"op\":1,\"d\":{\"rpcVersion\":1}}", null);
                Plugin.Log.Debug("Identified!");
            };

            Plugin.OBS_WS.OnClose += (sender, e) => {
                Plugin.Log.Debug("GOT CLOSE RESPONSE! " + e.Reason);
            };

            Plugin.OBS_WS.OnMessage += (sender, e) => {
                Plugin.Log.Debug("GOT RESPONSE! " + e.Data);
            };

            Plugin.BHS_WS.Connect();
            Plugin.OBS_WS.Connect();
        }

        private void Update()
        {

        }

        private void LateUpdate()
        {

        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null;

        }
    }
}
