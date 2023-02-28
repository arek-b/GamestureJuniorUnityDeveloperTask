using UnityEngine;

namespace Performance
{
    public class PerformanceManager : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}