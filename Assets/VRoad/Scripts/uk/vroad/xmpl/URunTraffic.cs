using uk.vroad.api;
using uk.vroad.api.etc;
using uk.vroad.api.geom;
using uk.vroad.api.map;
using uk.vroad.api.xmpl;
using uk.vroad.apk;
using uk.vroad.map;
using uk.vroad.pac;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace uk.vroad.xmpl
{
    public class URunTraffic: MonoBehaviour
    {
        public static URunTraffic MostRecentInstance { get; private set;  }

        public string vRoadFilePath;
        public Text buildErrorText;
        public Slider vRoadSlider;
        public int simulationWorkers = AppTools.N_WORKERS_INIT;
        
        private App app;
        private bool hasUI;
        private bool loadInProgress;
        private string progressActivity;
        private int progressRaw;
        private int progressSmoothed;
        
        public void SetupTraffic(string path)
        {
            vRoadFilePath = path;
        }

        protected virtual void Awake()
        {
            app = ExampleApp.AwakeInstance();
            
            MostRecentInstance = this;
            hasUI = buildErrorText != null && vRoadSlider != null;

            KFile mapFile = FindMapFile();
            if (mapFile != null && mapFile.Exists() && AppTools.SuitableAppFile(mapFile))
            {
                StartLoad(mapFile); // .vroad
            }
            else
            {
                Debug.LogWarning("Failed to build. No VRoad file "+vRoadFilePath);
            }
        }
        
        private void StartLoad(KFile mapFile)
        {
            loadInProgress = true;
            progressRaw = 0;
            progressSmoothed = 0;
            progressActivity = "Loading Map";
            if (hasUI) vRoadSlider.gameObject.SetActive(true);
            VRoad.SetWorkerCount(simulationWorkers);
            
            Reporter.ProgressPartsUI(1);
            VRoad.Load(app, mapFile);
        }
        void FixedUpdate()
        {
            if (loadInProgress)
            {
                progressRaw = Reporter.ProgressTotal();

                if (progressRaw < 100)
                {
                    int diff = (progressRaw * 100) - progressSmoothed;
                    if (diff > 0)
                    {
                        int inc = diff > 1000? 100: diff > 500? 20: 5;
                        progressSmoothed += inc;
                        if (hasUI) vRoadSlider.value = progressSmoothed;
                    }
                }
                else
                {
                    if (hasUI) vRoadSlider.gameObject.SetActive(false);
                    loadInProgress = false;
                    
                }
            }
        }

        public int Progress() { return progressRaw; }
        public string ProgressActivity() { return progressActivity; }

        private KFile FindMapFile()
        {
            // ** ALWAYS ** Call VroadWriteDir here (to initialize in correct thread)
            string userVRoadDir = KEnv.VroadWriteDir();
            
            if (vRoadFilePath == null) return null;
            vRoadFilePath = vRoadFilePath.Trim();
            if (vRoadFilePath.Length < 5) return null;
            
            // First check absolute path
            KFile mapFile = new KFile(vRoadFilePath);
            if (mapFile.Exists()) return mapFile;
            
            mapFile = new KFile(userVRoadDir, vRoadFilePath);  // .. then relative name
            if (mapFile.Exists()) return mapFile;
            
            return null;
        }

      
    }
}