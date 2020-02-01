using Cinemachine;
using System;
using UnityEngine;

namespace Modules.CameraSystem
{
    public class GameCamera : MonoBehaviour, IShake
    {
        #region Constant Fields

        private const float DefaultAmplitude = 0f;
        private const float DefaultFrequency = 0f;

        #endregion

        #region Serialized Fields

        [SerializeField] private CameraShakeData defaultCameraShakeData = null;
        [SerializeField] private Camera blurCamera = null;

        #endregion

        #region Private Fields

        private CinemachineBasicMultiChannelPerlin currentCamPerlin = null;

        private CameraShakeData shakeData = null;

        private CinemachineVirtualCamera defaultVirtualCamera;        

        #endregion

        #region Properties

        public CinemachineVirtualCamera CurrentVirtualCamera { get; private set; } = null;

        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            SetShakeData(defaultCameraShakeData);
        }
        
        #endregion

        #region Public Methods
        
        public void SetCamera(CinemachineVirtualCamera virtualCamera)
        {
            if (virtualCamera == null)
                return;

            if (CurrentVirtualCamera != null)
            {
                CurrentVirtualCamera.m_Priority = 0;
            }

            CurrentVirtualCamera = virtualCamera;
            CurrentVirtualCamera.m_Priority = 1;

            currentCamPerlin = CurrentVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public void SetConfiner(Collider2D confiner)
        {
            CurrentVirtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = confiner;
        }

        public void SetDefaultVirtualCamera(CinemachineVirtualCamera virtualCamera)
        {
            defaultVirtualCamera = virtualCamera;
        }

        public void Reset()
        {
            SetCamera(defaultVirtualCamera);
            SetShakeData(defaultCameraShakeData);
        }

        public void EnableBlurCamera()
        {
            blurCamera.gameObject.SetActive(true);
        }

        public void DisableBlurCamera()
        {
            blurCamera.gameObject.SetActive(false);
        }

        public void SetShakeData(CameraShakeData data)
        {
            if (data == null)
                return;

            shakeData = data;
        }

        public async void Shake(CameraShakeData data = null)
        {
            if (data == null && shakeData == null)
                throw new Exception("No usable shake data available");

            CameraShakeData dataToUse = data == null ? shakeData : data;

            currentCamPerlin.m_AmplitudeGain = dataToUse.Amplitude;
            currentCamPerlin.m_FrequencyGain = dataToUse.Frequency;

            await new WaitForSeconds(dataToUse.Duration);

            currentCamPerlin.m_AmplitudeGain = DefaultAmplitude;
            currentCamPerlin.m_FrequencyGain = DefaultFrequency;
        }

        public async void Shake(float duration, CameraShakeData data = null)
        {
            if (data == null && shakeData == null)
                throw new Exception("No usable shake data available");

            CameraShakeData dataToUse = data == null ? shakeData : data;

            currentCamPerlin.m_AmplitudeGain = dataToUse.Amplitude;
            currentCamPerlin.m_FrequencyGain = dataToUse.Frequency;

            await new WaitForSeconds(duration);

            currentCamPerlin.m_AmplitudeGain = DefaultAmplitude;
            currentCamPerlin.m_FrequencyGain = DefaultFrequency;
        }
        
        #endregion
    }
}