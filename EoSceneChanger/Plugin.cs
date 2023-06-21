using IPA;
using WebSocketSharp;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;


namespace EoSceneChanger {
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        internal static readonly WebSocket BHS_WS = new WebSocket("ws://localhost:6557/socket"); // BS HTTP-Status-Protocol (IN)
        internal static readonly WebSocket OBS_WS = new WebSocket("ws://localhost:9085"); // OBS-Websocket (OUT)

        [Init]
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Log.Info("EoSceneChanger initialized.");
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            new GameObject("EoSceneChangerController").AddComponent<EoSceneChangerController>();
            Log.Debug("Instantiated!");
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");

        }
    }
}
