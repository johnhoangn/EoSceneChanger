using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;

using BS_Utils.Utilities;
using System.IO;
using System.Threading;

namespace EoSceneChanger {
    public class EoSceneChangerController : MonoBehaviour {
        public static EoSceneChangerController Instance { get; private set; }
        private static string IDENTIFY_REQUEST = "{\"op\":1,\"d\":{\"rpcVersion\":1}}";
        private static string CONFIG_PATH = Path.Combine(Application.dataPath, @"..\UserData\Eo\SceneChanger\Config.json");

        private readonly WebSocket BHS_WS = new WebSocket("ws://localhost:6557/socket"); // BS HTTP-Status-Protocol (IN)
        private WebSocket OBS_WS; // OBS-Websocket (OUT)

        private bool Destroying = false;

        private EoJSON.Configuration Configuration;
        private Thread RejoinThread;

        private void Awake()
        {
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }

            string directory = Path.GetDirectoryName(CONFIG_PATH);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(CONFIG_PATH)) {
                using (StreamWriter sw = File.AppendText(CONFIG_PATH)) {
                    sw.WriteLine(JsonConvert.SerializeObject(new EoJSON.Configuration()));
                }
            }

            Plugin.Log?.Debug("Config: " + File.ReadAllText(CONFIG_PATH));

            Configuration = JsonConvert.DeserializeObject<EoJSON.Configuration>(File.ReadAllText(CONFIG_PATH));
            OBS_WS = new WebSocket("ws://localhost:" + Configuration.OBSPort);

            GameObject.DontDestroyOnLoad(this);
            Instance = this;
            Plugin.Log?.Debug($"{name}: Awake()");
        }

        private void Start()
        {
            BSEvents.gameSceneLoaded += GameSceneLoaded;
            BSEvents.menuSceneLoaded += MenuSceneLoaded;

            OBS_WS.OnOpen += OnOpen;
            OBS_WS.OnMessage += OnOBSMessage;

            BHS_WS.OnMessage += HttpStatusEvent;

            OBS_WS.Connect();
            BHS_WS.Connect();

            Plugin.Log?.Debug("Connected sockets!");

            RejoinThread = new Thread(new ThreadStart(() => {
                while (!Destroying) {
                    Plugin.Log?.Debug("Checking sockets' health... " + BHS_WS.IsAlive + " " + OBS_WS.IsAlive);
                    if (!BHS_WS.IsAlive)
                        BHS_WS.Connect();

                    if (!OBS_WS.IsAlive)
                        OBS_WS.Connect();

                    Thread.Sleep(5000);
                }
            }));

            RejoinThread.Start();
        }

        private void OnOBSMessage(object sender, MessageEventArgs e) {
            Plugin.Log.Debug("GOT OBS MESSAGE! " + e.Data);
        }

        private void OnOpen(object sender, System.EventArgs e) {
            Plugin.Log.Debug("Socket open, identifying");
            OBS_WS.SendAsync(IDENTIFY_REQUEST, (_) => Plugin.Log.Debug("Identified!")); ;
        }

        private void HttpStatusEvent(object sender, MessageEventArgs e) {
            Plugin.Log.Info("HTTP WS EVENT! " + e.Data);
        }

        private void MenuSceneLoaded() {
            EoJSON.OBSRequest rqst = new EoJSON.OBSRequest(6, "SetCurrentProgramScene", Configuration.MenuScene);
            OBS_WS.SendAsync(JsonConvert.SerializeObject(rqst), null);
        }

        private void GameSceneLoaded() {
            EoJSON.OBSRequest rqst = new EoJSON.OBSRequest(6, "SetCurrentProgramScene", Configuration.GameScene);
            OBS_WS.SendAsync(JsonConvert.SerializeObject(rqst), null);
        }

        private void OnDestroy()
        {
            Destroying = true;
            RejoinThread.Abort();
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null;
        }
    }
}
