using System;
using System.Collections;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.SceneManagement;

namespace Chimpvine.WebClient
{
    /// <summary>
    /// Singleton Class that handles all the communication with Game Server Rest API
    /// </summary>
    public class ChimpvineMessenger : MonoSingleton<ChimpvineMessenger>
    {
        #region Private Variables
        
        /// <summary>
        /// Endpoint of the API and necessary IDs
        /// </summary>
        string apiURI, userID, sessionID, fileID, gameUri;
        
        /// <summary>
        /// Current Entry ID in the database table for this session of the gameplay
        /// </summary>
        int currentEntryID;
        
        /// <summary>
        /// JSONNode from SimpleJSON to handle resopnse from API
        /// </summary>
        JSONNode apiResponse;
        #endregion

        public JSONNode ApiResponse { get; private set; }

        #region Events Raised
        
        public static Action<string, string, string, string, string> idInitialized;
        
        #endregion

        #region Mono Callbacks
        void OnEnable()
        {
            Init();
        }
        #endregion

        #region Initialization for IDs
        protected override void Init()
        {
            setWebServiceURI();
            setFileID();
            setIDFromCookie();
            idInitialized(sessionID, userID, fileID, apiURI, gameUri);
        }
        #endregion

        #region Private Methods
        void setWebServiceURI() 
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var absoluteUri = new Uri(Application.absoluteURL);
            apiURI = absoluteUri.Authority + "/Game-API/main.php";
#else       
            apiURI = "https://test314159.chimpvine.com/Game-API/main.php";
#endif
        }

        void setFileID() 
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var uri = new Uri(ChimpvineWebPlugin.GetURLFromPage());
            fileID = HttpUtility.ParseQueryString(uri.Query).Get("id");
            gameUri = uri.ToString();
#else
            var uri = new Uri("https://test314159.chimpvine.com/mod/resource/view.php?id=122&forceview=1");
            fileID = HttpUtility.ParseQueryString(uri.Query).Get("id");
#endif
        }

        void setIDFromCookie() 
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            sessionID = ChimpvineWebPlugin.GetCookie("MoodleSession");
            userID = ChimpvineWebPlugin.GetCookie("ChimpID");            
#else
            sessionID = "dummySessionID";
            userID = "1557";
#endif
        }
        #endregion

        #region Public Methods
        public static IEnumerator SendGetDataRequest() 
        {
            yield return Instance.StartCoroutine(Instance.GetRequestCoroutine());
            Debug.Log(Instance.apiResponse);
        }

        public static void SendGameStartRequest(string level) 
        { 
            Instance.StartCoroutine(Instance.StartPostRequestCoroutine(level));
        }

        public static void SendGameUpdateRequest(string level, int score) 
        {
            Instance.StartCoroutine(Instance.UpdatePostRequestCoroutine(level, score));
        }

        public void startReq() 
        {
            SendGameStartRequest(SceneManager.GetActiveScene().name);
        }

        public void updateReq() 
        {
            SendGameUpdateRequest(SceneManager.GetActiveScene().name,100);
        }
        #endregion

        public void getReq() 
        {
            StartCoroutine(SendGetDataRequest());
        }

#region Web Request Coroutines
        static UnityWebRequest BuildPostRequestStart(string level)
        {
            WWWForm form = new WWWForm();
            form.AddField("user_id", Instance.userID);
            form.AddField("session_id", Instance.sessionID);
            form.AddField("file_id", Instance.fileID);
            form.AddField("level_start", level);
            form.AddField("date_start", DateTime.Now.ToString("o"));
            form.AddField("platform", Application.platform.ToString());
            UnityWebRequest request = UnityWebRequest.Post(Instance.apiURI, form);
            return request;
        }

        static UnityWebRequest BuildPostRequestUpdate(string level, int score)
        {
            WWWForm form = new WWWForm();
            form.AddField("user_id", Instance.userID);
            form.AddField("id", Instance.currentEntryID);
            form.AddField("date_end", DateTime.Now.ToString("o"));
            form.AddField("level_end", level);
            form.AddField("score", score);
            UnityWebRequest request = UnityWebRequest.Post(Instance.apiURI, form);
            return request;
        }

        static UnityWebRequest BuildGetRequest() 
        {
            UnityWebRequest request = UnityWebRequest.Get(Instance.apiURI + "?user_id=" + Instance.userID + "&file_id=" + Instance.fileID);
            return request;
        }

        IEnumerator GetRequestCoroutine() 
        {
            using (UnityWebRequest req = BuildGetRequest()) 
            {
                yield return req.SendWebRequest();
                if (req.isNetworkError || req.isHttpError)
                {
                    Debug.LogError(req.error);
                }
                else 
                {
                    apiResponse = JSONNode.Parse(req.downloadHandler.text);
                    yield return apiResponse;
                }
            }
        }

        IEnumerator StartPostRequestCoroutine(string level) 
        {
            using (UnityWebRequest req = BuildPostRequestStart(level))
            {
                yield return req.SendWebRequest();
                if (req.isNetworkError || req.isHttpError)
                {
                    Debug.LogError(req.error);
                }
                else
                {
                    apiResponse = JSONNode.Parse(req.downloadHandler.text);
                    currentEntryID = apiResponse["data"]["id"];
                    Debug.Log(apiResponse["data"]);
                }
            }
        }

        IEnumerator UpdatePostRequestCoroutine(string level, int score) 
        {
            using (UnityWebRequest req = BuildPostRequestUpdate(level, score))
            {
                yield return req.SendWebRequest();
                if (req.isNetworkError || req.isHttpError)
                {
                    Debug.LogError(req.error);
                }
                else
                {
                    apiResponse = JSONNode.Parse(req.downloadHandler.text);
                    Debug.Log(apiResponse);
                }
            }
        }
#endregion
    }
}
