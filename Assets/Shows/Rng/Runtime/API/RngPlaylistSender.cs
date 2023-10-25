using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Plml.Rng
{
    public class RngPlaylistSender : MonoBehaviour
    {
        public string url = "";

        public void SendPlaylist(string[] titles) => StartCoroutine(SendPlaylistCoroutine(titles));

        private IEnumerator SendPlaylistCoroutine(string[] titles)
        {
            Debug.Log("Sending playlist");
            var postData = JsonUtility.ToJson(titles);
            var req = UnityWebRequest.Post(url, postData);
            yield return req.SendWebRequest();

            if (!req.error.IsNullOrEmpty())
            {
                Debug.LogError(req.error);
            }
            else
            {
                Debug.Log($"Playlist sent with result {req.result}");
            }
            req.Dispose();
        }
    }
}
