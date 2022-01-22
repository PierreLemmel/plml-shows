using UnityEngine;

namespace Plml
{
    public class RunInBackground : MonoBehaviour
    {
        private void OnEnable() => Application.runInBackground = true;
        private void OnDisable() => Application.runInBackground = false;
    }
}