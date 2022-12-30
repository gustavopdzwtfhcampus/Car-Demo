using System;
using uk.vroad.api;
using uk.vroad.api.enums;
using uk.vroad.api.etc;
using uk.vroad.api.map;
using uk.vroad.api.str;
using uk.vroad.api.xmpl;
using uk.vroad.apk;
using uk.vroad.ucm;
using UnityEngine;
using UnityEngine.UI;

using BP = uk.vroad.api.enums.BuildParameter;
using BF = uk.vroad.api.enums.BuildFlag;

namespace uk.vroad.EditorObjects
{
    /// <summary> This class (BuildParameters) contains parameters that must be set at build time,
    /// to define values that are set in  the .vroad file.
    /// There is a much larger set of parameters in UaMapMesh, which control how the data in
    /// the .vroad file is rendered </summary>
    [System.Serializable] public class BuildParameters : Inspectable
    {
        [Range(BP.MIN_STOREY, BP.MAX_STOREY)] 
        [Tooltip("The height of one storey on a building, in metres")]
        public float buildingStoreyHeight = BP.BuildingStoreyHeight.FloatValue();
        
        [Range(BP.MIN_LEVELS, BP.MAX_LEVELS)] 
        [Tooltip("The default number of levels (storeys) on a building, if it is not specified in OSM")]
        public int buildingLevels = BP.BuildingLevels.IntValue();
        
        [Range(BP.MIN_ISLAND_SIDEWALKS, BP.MAX_ISLAND_SIDEWALKS)] 
        [Tooltip("Where there is an island surrounded by roads, if it less than this area (in sq. m.) then" +
                 "the island will have no sidewalks.")]
        public float islandSidewalkArea = BP.IslandAreaSidewalks.FloatValue();
        
        [Range(0, 100)] [Tooltip("The proportion (%) of cars in the simulation relative to trucks and coaches")]
        public int carsToOthers = BP.CarsToOthers.IntValue();

        [Range(0, 100)] [Tooltip("The proportion of coaches to trucks in the simulation")]
        public int coachesToTrucks = BP.CoachesToTrucks.IntValue();
    }
    
    /// <summary> EDITOR-ONLY Class: marshals parameters for running Osm2VRoad in an external process </summary>
    /// This class cannot go in the Editor folder
    public class UBuildMap: MonoBehaviour
    {
#if UNITY_EDITOR
        public static UBuildMap MostRecentInstance { get; private set;  }

        [Tooltip("A UI Text object in the scene that can be used to display a build error. " +
                 "This can be null, in which case build errors will be shown in the Console only")]
        public Text buildErrorText;
        
        [Tooltip("A progress-bar slider to show the progress of reading an OSM file and converting it to VROAD format")]
        public Slider osmSlider;
        [Tooltip("A progress-bar slider to show the progress of reading a VROAD file and creating the meshes for display")]
        public Slider vRoadSlider;

        //  [Tooltip("")]
        [Tooltip("A set of build-time parameters used to adjust the objects written into the VROAD file ")]
        public BuildParameters parameters;
        
        [SerializeField] [HideInInspector] private String args;
        
        private App app;
        private bool argsValid;
        private bool hasUI;
        private bool buildInProgress;
        private bool loadInProgress;
        private string progressActivity;
        private int progressRaw;
        private int progressSmoothed;

        protected virtual void Awake()
        {
            app = ExampleApp.AwakeInstance();
            
            MostRecentInstance = this;
            hasUI = buildErrorText != null && osmSlider != null && vRoadSlider != null;

            if (args != null)
            {
                if (!StartExternalBuild(ExternalMapBuilder.BuildMap(args)))
                {
                    Debug.LogError(ExternalMapBuilder.BuildError());
                }
            }
            else Debug.LogWarning("No location specified for map. " +
                                  "Expected Lat-Long specification or OSM JSON file");
        }
        
        /// <summary> Called after the external process has been started, to set some state variables in this class </summary>
        /// <returns> true if the build succeeded, false on failure </returns>
        private bool StartExternalBuild(bool buildSuccess)
        {
            if (hasUI) vRoadSlider.gameObject.SetActive(false);
            buildInProgress = buildSuccess;
            loadInProgress = false;
            progressRaw = 0;
            progressSmoothed = 0;
            progressActivity = buildSuccess? "Building Map": "Failed to Build";
            if (buildInProgress && hasUI) osmSlider.gameObject.SetActive(true);

            return buildSuccess;
        }
        
        /// <summary> This method is called to start loading the VROAD file, after it has been written
        /// by the external building process  </summary>
        private void StartLoad(KFile mapFile)
        {
            buildInProgress = false;
            loadInProgress = true;
            progressRaw = 0;
            progressSmoothed = 0;
            progressActivity = "Loading Map";
            if (hasUI) vRoadSlider.gameObject.SetActive(true);

            SetupBuildParametersForMesh();
            
            Reporter.ProgressPartsUI(1);
            pac.VRoad.Load(app, mapFile);
        }
        
        void FixedUpdate()
        {
            if (buildInProgress)
            {
                progressRaw = ExternalMapBuilder.Progress();
               
                string newBuild = ExternalMapBuilder.NewlyBuiltFile();
                string buildError = ExternalMapBuilder.BuildError();

                if (newBuild == null && buildError == null)
                {
                    int diff = (progressRaw * 100) - progressSmoothed;
                    if (diff > 0)
                    {
                        int inc = diff / 10;
                        progressSmoothed += inc;
                        if (hasUI) osmSlider.value =  progressSmoothed;
                    }
                }
                else
                {
                    if (hasUI) osmSlider.gameObject.SetActive(false);
                    
                    buildInProgress = false;
                    progressRaw = 0;
                    progressSmoothed = 0;
                    
                    KFile mapFile = newBuild != null? new KFile(newBuild): null;
                    if (mapFile != null && mapFile.Exists() && AppTools.SuitableAppFile(mapFile))
                    {
                        if (hasUI) vRoadSlider.gameObject.SetActive(true);
                        StartLoad(mapFile);
                    }
                    else
                    {
                        if (buildError == null) buildError = "No generated file, no error?";

                        if (hasUI)
                        {
                            buildErrorText.gameObject.SetActive(true);
                            buildErrorText.text = buildError;
                        }
                        else Debug.Log(buildError);
                    }
                }
            }
            else if (loadInProgress)
            {
                progressRaw = Reporter.ProgressTotal();

                if (progressRaw < 100)
                {
                    int diff = (progressRaw * 100) - progressSmoothed;
                    if (diff > 0)
                    {
                        int inc = diff / 10; // diff > 1000? 100: diff > 500? 20: 5;
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

        /// <summary> This is called from EditorWindow OnGUI (while not playing) so values must be set
        /// into serialized fields so that they are still available when Awake() is called </summary>
        public void SetupBuildParameters()
        {
            string path = BP.FilePath.Value();
            string latLong = BP.LatLong.Value();

            if (FoundOsmFile(path) || LooksLikeLatLong(latLong))
            {
                BP.CarsToOthers.Set(parameters.carsToOthers);
                BP.CoachesToTrucks.Set(parameters.coachesToTrucks);
                BP.BuildingStoreyHeight.Set(parameters.buildingStoreyHeight);
                BP.BuildingLevels.Set(parameters.buildingLevels);
                BP.IslandAreaSidewalks.Set(parameters.islandSidewalkArea);
                
                args = BuildParameter.ArgumentString();
            }
            else args = null;
        }

        // Set up any parameters that are used in the mesh creation
        void SetupBuildParametersForMesh()
        {
            BP.BuildingStoreyHeight.Set(parameters.buildingStoreyHeight);
        }
          
        private bool FoundOsmFile(string osmFilePath)
        {
            // ** ALWAYS ** Call OsmDir here (to initialize in correct thread)
            string userOsmDir = KEnv.OsmDir();
            
            if (osmFilePath == null) return false;
            osmFilePath = osmFilePath.Trim();
            if (osmFilePath.Length < 5) return false;
            
            // First check absolute path
            KFile mapFile = new KFile(osmFilePath);
            if (mapFile.Exists()) return true;
            
            mapFile = new KFile(userOsmDir, osmFilePath); // .. then relative name of OSM file
            if (mapFile.Exists()) return true;

            return false;
        }


        protected void OnApplicationQuit()
        {
            ExternalMapBuilder.ForceQuit();
        }
        
        
        public static bool LooksLikeLatLong(string s)
        {
            if (s == null) return false;
            s = s.Trim();
            if (s.Length < 4 || s.Length > 60) return false;
            
            char separator = CC.COMMA;
            if (s.IndexOf(separator) < 1) return false;

            double LATMAX = 85; // See also BoundsBox.LAT_MAX_PRACTICAL
            double LNGMAX = 180.001;
            string[] parts = KTools.SplitQuick(s, separator);
            
            if (parts.Length == 4)
            {
                double[] na = Numbers(parts);
                if (na == null) return false;

                if (-LATMAX > na[0] || na[0] > LATMAX) return false;
                if (-LNGMAX > na[1] || na[1] > LNGMAX) return false;
            }
           
            if (parts.Length == 2)
            {
                double[] na = Numbers(parts);
                if (na == null) return false;

                if (-LATMAX > na[0] || na[0] > LATMAX) return false;
                if (-LNGMAX > na[1] || na[1] > LNGMAX) return false;
            }
            
            // More tests could go here

            return true;
        }
        private static double[] Numbers(string[] parts)
        {
            double[] na = new double[parts.Length];
            for ( int i = 0; i < parts.Length; i++) 
            {
                try { na[i] = KTools.ParseDouble(parts[i]); }
                catch (FormatException) { return null; }
            }
            return na;
        }
#endif  // UNITY_EDITOR
    }
}