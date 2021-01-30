﻿using System;
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
        string apiURI, userID, sessionID, fileID;
        
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

        #region Mono Callbacks
        void OnEnable()
        {
            Init();
        }
        #endregion

        #region Initialization for IDs
        protected override void Init()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            setWebServiceURI();
            setFileID();
            setIDFromCookie();
#else
            setForLocalBuild();
#endif
        }
#endregion

#region Private Methods
        void setWebServiceURI() 
        {
            var absoluteUri = new Uri(Application.absoluteURL);
            apiURI = absoluteUri.Authority + "/Game-API/main.php";
        }

        void setFileID() 
        {
            var uri = new Uri(ChimpvineWebPlugin.GetURLFromPage());
            fileID = HttpUtility.ParseQueryString(uri.Query).Get("id");

        }

        void setIDFromCookie() 
        {
            sessionID = ChimpvineWebPlugin.GetCookie("MoodleSession");
            userID = ChimpvineWebPlugin.GetCookie("ChimpID");
        }

        void setForLocalBuild() 
        {
            apiURI = "https://test314159.chimpvine.com/Game-API/main.php";
            var uri = new Uri("https://test314159.chimpvine.com/mod/resource/view.php?id=555&forceview=1");
            fileID = HttpUtility.ParseQueryString(uri.Query).Get("id");
            sessionID = "dummySessionID";
            userID = "1557";
        }

#endregion

#region Public Methods
        public static void SendGetDataRequest(Action<JSONNode> callback) 
        {
            Instance.StartCoroutine(Instance.GetRequestCoroutine(callback));
        }

        public static void SendGameStartRequest(string level) 
        { 
            Instance.StartCoroutine(Instance.StartPostRequestCoroutine(level));
        }

        public static void SendGameUpdateRequest(string level, int score) 
        {
            Instance.StartCoroutine(Instance.UpdatePostRequestCoroutine(level, score));
        }

        public void getReq() 
        {
            SendGetDataRequest(res => 
            {
                Debug.Log(res["prev_data"].Count);
            });
        }

        public void sendStartDataReq() 
        {
            SendGameStartRequest(1.ToString());
        }

        public void sendUpdateDataReq() 
        {
            SendGameUpdateRequest(5.ToString(), 50);
        }
#endregion

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

        IEnumerator GetRequestCoroutine(Action<JSONNode> callback) 
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
                    callback(apiResponse);
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
