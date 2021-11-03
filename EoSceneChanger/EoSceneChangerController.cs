using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

using BS_Utils.Utilities;

namespace EoSceneChanger
{
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
                EoJSON.OBSRequest rqst = new EoJSON.OBSRequest();
                rqst.RequestType = "SetCurrentScene";
                rqst.MessageId = "1";
                rqst.SceneName = "FBT Game";
                Plugin.OBS_WS.Send(JsonConvert.SerializeObject(rqst));
            };
            BSEvents.menuSceneLoaded += () =>
            {
                EoJSON.OBSRequest rqst = new EoJSON.OBSRequest();
                rqst.RequestType = "SetCurrentScene";
                rqst.MessageId = "2";
                rqst.SceneName = "Desktop Game";
                Plugin.OBS_WS.Send(JsonConvert.SerializeObject(rqst));
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
