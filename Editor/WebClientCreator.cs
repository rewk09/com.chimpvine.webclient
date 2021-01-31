using UnityEngine;
using UnityEditor;
using System.Runtime.CompilerServices;
namespace Chimpvine.WebClient.Editor
{
	public class WebClientCreator : EditorWindow
	{
		[MenuItem("Chimpvine/Create Web Client", false, 0)]
		static void CreateManager()
		{
			GameObject xapi = GameObject.FindObjectOfType<ChimpvineMessenger>().gameObject;
			if (xapi == null)
			{
				xapi = new GameObject("ChimpvineAPI");
				xapi.AddComponent<ChimpvineMessenger>();
				EditorUtility.DisplayDialog("ChimpvineAPI", "Chimpvine API Client has been added to the scene", "OK");
			}
			else
			{
				EditorUtility.DisplayDialog("Client API is already present", "You only need one in a scene", "OK");
			}

		}
	}
}
