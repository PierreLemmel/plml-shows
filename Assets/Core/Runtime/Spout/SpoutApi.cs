using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Plml.Spout
{
    public static class SpoutApi
    {
        private enum ApiState
        {
            Idle,
            Running
        }

        private static ApiState state = ApiState.Idle;

        public static void Initialize()
        {
            if (state != ApiState.Running)
            {
                Debug.Log("Initializing Spout API");
                bool success = InitNative();
                if (success)
                    state = ApiState.Running;
                else
                    throw new SpoutApiException("Error initializing Spout API");

                Debug.Log("Spout API initialized");
            }
        }

        public static void InitDebug() => InitDebugConsoleNative();

        public static void CreateSender(string sharingName, Texture texture, TextureFormat format)
        {
            Debug.Log($"Creating Spout sender '{sharingName}'");
            bool success = CreateSenderNative(sharingName, texture.GetNativeTexturePtr(), (int)format);
            if (!success)
                throw new SpoutApiException($"Error while creating sender '{sharingName}'");
            Debug.Log($"Spout sender '{sharingName}' created");
        }

        public static void UpdateSender(string sharingName, Texture texture)
        {
            bool success = UpdateSenderNative(sharingName, texture.GetNativeTexturePtr());
            if (!success)
                throw new SpoutApiException($"Error while updating sender '{sharingName}'");
        }

        public static void CloseSender(string sharingName)
        {
            Debug.Log($"Closing Spout sender '{sharingName}'");
            bool success = CloseSenderNative(sharingName);
            if (!success)
                throw new SpoutApiException($"Error while closing sender '{sharingName}'");
            Debug.Log($"Spout sender '{sharingName}' closed");
        }

        public static void CleanUp()
        {
            if (state == ApiState.Running)
            {
                Debug.Log("Cleaning up Spout API");
                CleanNative();

                state = ApiState.Idle;
                Debug.Log("Spout API cleaned up");
            }
        }

        #region Imports
        private const string PluginName = "NativeSpoutPlugin";

        static SpoutApi()
        {
            Debug.Log("SpoutApi initialized");
        }

        [DllImport(PluginName, EntryPoint = "init")]
        private static extern bool InitNative();

        [DllImport(PluginName, EntryPoint = "initDebugConsole")]
        private static extern void InitDebugConsoleNative();

        [DllImport(PluginName, EntryPoint = "createSender")]
        private static extern bool CreateSenderNative(string sharingName, IntPtr texture, int texFormat);

        [DllImport(PluginName, EntryPoint = "updateSender")]
        private static extern bool UpdateSenderNative(string sharingName, IntPtr texture);

        [DllImport(PluginName, EntryPoint = "closeSender")]
        private static extern bool CloseSenderNative(string sharingName);

        [DllImport(PluginName, EntryPoint = "clean")]
        private static extern void CleanNative();
        #endregion
    }
}