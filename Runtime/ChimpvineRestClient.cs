using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace Chimpvine.WebClient
{
    /// <summary>
    /// Wrapper Class Exposed to handle all Rest API Calls
    /// </summary>
    public sealed class ChimpvineRestClient
    {
        public JSONNode ServerResponse { get; private set; }
        
        /// <summary>
        /// Initial Get Request Coroutine to fetch game data from previos gameplay session
        /// </summary>
        public IEnumerator GetPreviousGameData() 
        {
            yield return ChimpvineMessenger.SendGetDataRequest();
            ServerResponse = ChimpvineMessenger.Instance.ApiResponse;
            Debug.Log(ServerResponse);
        }

        /// <summary>
        /// Initial Post Request to set game data of this gameplay session
        /// </summary>
        public static void SendGameStartRequest(string level) 
        {
            ChimpvineMessenger.SendGameStartRequest(level);
        }

        /// <summary>
        /// Post Request to update game data of this gameplay session
        /// </summary>
        public static void SendGameUpdateRequest(string level, int score) 
        {
            ChimpvineMessenger.SendGameUpdateRequest(level, score);
        }
    }
}
