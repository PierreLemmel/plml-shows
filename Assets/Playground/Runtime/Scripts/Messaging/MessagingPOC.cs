using Firebase;
using Firebase.Database;
using Plml.Dmx;
using System;
using UnityEngine;

namespace Plml.Messaging
{
    public class MessagingPOC : MonoBehaviour
    {
        private DateTime startTime;
        public DmxTrack track;

        private void Start()
        {
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("rng/messages");

            startTime = DateTime.Now;

            reference.ChildAdded += Reference_ChildAdded;

            track.isPlaying = false;
        }

        private void Reference_ChildAdded(object sender, ChildChangedEventArgs e)
        {
            DateTime timeStamp = DateTime.Parse(e.Snapshot.Key);

            if (timeStamp > startTime)
            {
                string type = (string)e.Snapshot.Child("type").Value;

                if (type == "start")
                    OnStartMessage();
                else if (type == "stop")
                    OnStopMessage();
            }

        }

        private void OnStartMessage() => track.isPlaying = true;

        private void OnStopMessage() => track.isPlaying = false;
    }
}