using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SimpleJSON;

namespace Chimpvine.WebClient
{
    /// <summary>
    /// Wrapper Class Exposed to handle all Rest API Calls
    /// </summary>
    public sealed class ChimpvineRestClient
    {
        public static JSONNode ServerResponse { get; private set; }
        static Action<JSONNode> apiCallback = apiCallbackFunction;
        
        static void apiCallbackFunction(JSONNode res) 
        {
            ServerResponse = ChimpvineMessenger.Instance.ApiResponse;
            Debug.Log(ServerResponse);
        }

        /// <summary>
        /// Initial Get Request Coroutine to fetch game data from previos gameplay session
        /// </summary>
        public static void GetPreviousGameData(Action<JSONNode> callback = null) 
        {
            CheckMessenger();
            if (callback == null)
            {
                ChimpvineMessenger.SendGetDataRequest(apiCallback);
            }
            else 
            {
                ChimpvineMessenger.SendGetDataRequest(callback);
            }
        }

        /// <summary>
        /// Initial Post Request to set game data of this gameplay session
        /// </summary>
        public static void SendGameStartRequest(string level) 
        {
            CheckMessenger();
            ChimpvineMessenger.SendGameStartRequest(level);
        }

        /// <summary>
        /// Post Request to update game data of this gameplay session
        /// </summary>
        public static void SendGameUpdateRequest(string level, int score) 
        {
            CheckMessenger();
            ChimpvineMessenger.SendGameUpdateRequest(level, score);
        }

        private static void CheckMessenger() 
        {
            if (ChimpvineMessenger.InstanceExists == false)
            {
                // Create an empty game object following our convention
                new GameObject("ChimpvineClient").AddComponent<ChimpvineMessenger>();
            }
        }
    }
}
