using System;
using System.Text;
using UnityEngine;

namespace DefaultNamespace
{
    public class MySpectrum : MonoBehaviour
    {
        public float scale = 1f;
        public float highScaleAverage = 2f;
        public float highScaleNotAverage = 3f;
        
        
        private void Start()
        {
            SpectrumUtil.Inst.Init(4);
        }

        private void Update()
        {
            var data = SpectrumUtil.Inst.GetFFtData();
            if (data == null)
            {
                return;
            }

            var max = 0f;
            
            for (int i = 0; i < data.Length; i++)
            {
                data[i] /= 150f;
                data[i] += this.highScaleAverage * Mathf.Sqrt((float)i / 4f) * data[i];

                max = Math.Max(max, data[i]);

            }
            
            Shader.SetGlobalFloat("SOUND_MULTIPLIER", max * scale);
        }

        void LogData(float[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append(t + ",");
            }
            Debug.Log(sb.ToString());
        }
    }
}