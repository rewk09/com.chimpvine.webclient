using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Chimpvine.WebClient
{
    /// <summary>
    /// Class to provide bridge for browser communication
    /// </summary>
    public static class ChimpvineWebPlugin
    {
        #region JS Function Handles
        /// <summary>
        /// Check for mobile browser
        /// </summary>
        /// <returns>True if the browser is mobile.</returns>
        [DllImport("__Internal")]
        private static extern bool isMobileBrowser();

        /// <summary>
        /// Display Alert for error handling
        /// </summary>
        /// <param name="msg">The error message for alert.</param>
        [DllImport("__Internal")]
        private static extern void showAlert(string msg);

        /// <summary>
        /// Get all cookies
        /// </summary>
        /// <returns></returns>
        [DllImport("__Internal")]
        private static extern string getHttpCookies();

        /// <summary>
        /// Get cookie by name
        /// </summary>
        /// <param name="name">Name of the cookie</param>
        /// <returns></returns>
        [DllImport("__Internal")]
        private static extern string getHttpCookie(string name);

        /// <summary>
        /// Get Page URL, sometimes different from absoluteURL
        /// </summary>
        /// <returns>url of the page</returns>
        [DllImport("__Internal")]
        private static extern string getURLFromPage();
        #endregion

        #region JS Function Wrappers
        /// <summary>
        /// Check for mobile browser
        /// </summary>
        /// <returns>True if the game is running in mobile browser</returns>
        public static bool IsMobileBrowser() 
        {
            return isMobileBrowser();
        }

        /// <summary>
        /// Display alert in the browser
        /// </summary>
        /// <param name="msg">The message to display</param>
        public static void ShowAlert(string msg) 
        {
            showAlert(msg);
        }

        /// <summary>
        /// Get Cookie function 
        /// </summary>
        /// <param name="name">Name of the cookie</param>
        /// <returns></returns>
        public static string GetCookie(string name) 
        {
            return getHttpCookie(name);
        }

        /// <summary>
        /// Get URL from page
        /// </summary>
        /// <returns>The url from page, could be different from absolute URL</returns>
        public static string GetURLFromPage() 
        {
            return getURLFromPage();
        }
        #endregion
    }
}
