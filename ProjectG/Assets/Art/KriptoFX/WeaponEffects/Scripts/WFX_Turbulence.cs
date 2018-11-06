using System;
using System.Threading;
using UnityEngine;

[ExecuteInEditMode]
public class WFX_Turbulence : MonoBehaviour
{
    public float TurbulenceStrenght = 1;
    public bool TurbulenceByTime;
    public AnimationCurve TurbulenceStrengthByTime = AnimationCurve.EaseInOut(1, 1, 1, 1);
    public Vector3 Frequency = new Vector3(1, 1, 1);
    public Vector3 OffsetSpeed = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 Amplitude = new Vector3(5, 5, 5);
    public Vector3 GlobalForce;
    public MoveMethodEnum MoveMethod;
    public PerfomanceEnum Perfomance = PerfomanceEnum.UltraRealTime;
    

    public enum MoveMethodEnum
    {
        Position,
        Velocity,
        RelativePositionHalf,
        RelativePosition
    }

    public enum PerfomanceEnum
    {
        UltraRealTime,
        High,
        Low
    }

    private float lastStopTime;
    private Vector3 currentOffset;
    private float deltaTime;
    private float deltaTimeLastUpdateOffset;
    private ParticleSystem.Particle[] particleArray;
    private ParticleSystem particleSys;
    private float time;
    private int currentSplit;
    private float fpsTime;
    private int FPS;
    private int splitUpdate;
    private PerfomanceEnum perfomanceOldSettings;
    private bool skipFrame;

    private void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
#if UNITY_5_5_OR_NEWER
        if (particleArray==null || particleArray.Length < particleSys.main.maxParticles) 
            particleArray = new ParticleSystem.Particle[particleSys.main.maxParticles];
#else
            if (particleArray==null || particleArray.Length < particleSys.maxParticles) 
            particleArray = new ParticleSystem.Particle[particleSys.maxParticles];
#endif
        perfomanceOldSettings = Perfomance;
        UpdatePerfomanceSettings();
    }


    private void Update()
    {
        if (!Application.isPlaying) {
            deltaTime = Time.realtimeSinceStartup - lastStopTime;
            lastStopTime = Time.realtimeSinceStartup;
            UpdateTurbulence();
            return;
        }
        else
            deltaTime = Time.deltaTime;
        currentOffset += OffsetSpeed * deltaTime;

        if (Perfomance!=perfomanceOldSettings) {
            perfomanceOldSettings = Perfomance;
            UpdatePerfomanceSettings();
        }
        time += deltaTime;
        if (FPS==0) {
            UpdateTurbulence();
            return;
        }

        if (QualitySettings.vSyncCount==2)
            UpdateTurbulence();
        else if (QualitySettings.vSyncCount==1) {
            if (Perfomance==PerfomanceEnum.Low) {
                if (skipFrame)
                    UpdateTurbulence();
                skipFrame = !skipFrame;
            }
            if (Perfomance==PerfomanceEnum.High)
                UpdateTurbulence();
        }
        else {
            if (QualitySettings.vSyncCount==0) {
                if (time >= fpsTime)
                {
                    time = 0;
                    UpdateTurbulence();
                    deltaTimeLastUpdateOffset = 0;
                }
                else
                    deltaTimeLastUpdateOffset += deltaTime;
            }
        }
       
    }

    private void UpdatePerfomanceSettings()
    {
        if (Perfomance==PerfomanceEnum.UltraRealTime) {
            FPS = 0;
            splitUpdate = 2;
        }
        if (Perfomance==PerfomanceEnum.High) {
            FPS = 60;
            splitUpdate = 2;
        }
        if (Perfomance==PerfomanceEnum.Low) {
            FPS = 30;
            splitUpdate = 2;
        }
        fpsTime = 1.0f / FPS;
    }

    private void UpdateTurbulence()
    {
        int start;
        int end;
        var numParticlesAlive = particleSys.GetParticles(particleArray);
        var turbulenceStrenghtMultiplier = 1;
        if (splitUpdate > 1) {
            start = (numParticlesAlive / splitUpdate) * currentSplit;
            end = (numParticlesAlive / splitUpdate) * (currentSplit + 1);
            turbulenceStrenghtMultiplier = splitUpdate;
        }
        else {
            start = 0;
            end = numParticlesAlive;
        }
        for (int i = start; i < end; i++) {
            var particle = particleArray[i];
            float timeTurbulenceStrength = 1;
#if UNITY_5_5_OR_NEWER
            if (TurbulenceByTime)
                timeTurbulenceStrength = TurbulenceStrengthByTime.Evaluate(1 - particle.remainingLifetime / particle.startLifetime);
#else
           if (TurbulenceByTime)
                timeTurbulenceStrength = TurbulenceStrengthByTime.Evaluate(1 - particle.lifetime / particle.startLifetime);
#endif

            var pos = particle.position;
            pos.x /= Frequency.x;
            pos.y /= Frequency.y;
            pos.z /= Frequency.z;
            var turbulenceVector = new Vector3();
            var timeOffset = deltaTime + deltaTimeLastUpdateOffset;
            turbulenceVector.x = ((Mathf.PerlinNoise(pos.z - currentOffset.z, pos.y - currentOffset.y) * 2 - 1) * Amplitude.x + GlobalForce.x) * timeOffset;
            turbulenceVector.y = ((Mathf.PerlinNoise(pos.x - currentOffset.x, pos.z - currentOffset.z) * 2 - 1) * Amplitude.y + GlobalForce.y) * timeOffset;
            turbulenceVector.z = ((Mathf.PerlinNoise(pos.y - currentOffset.y, pos.x - currentOffset.x) * 2 - 1) * Amplitude.z + GlobalForce.z) * timeOffset;
            turbulenceVector *= TurbulenceStrenght * timeTurbulenceStrength * turbulenceStrenghtMultiplier;
            if (MoveMethod==MoveMethodEnum.Position)
                particleArray[i].position += turbulenceVector;
            if (MoveMethod==MoveMethodEnum.Velocity)
                particleArray[i].velocity += turbulenceVector;
            if (MoveMethod==MoveMethodEnum.RelativePositionHalf) {
                particleArray[i].position += turbulenceVector;
                particleArray[i].velocity = particleArray[i].velocity * 0.5f + turbulenceVector.normalized / 10;
            }
            if (MoveMethod==MoveMethodEnum.RelativePosition) {
                particleArray[i].position += turbulenceVector;
                particleArray[i].velocity = particleArray[i].velocity * 0.9f + turbulenceVector.normalized / 10;
            }
        }
        particleSys.SetParticles(particleArray, numParticlesAlive);

        currentSplit++;
        if (currentSplit >= splitUpdate)
            currentSplit = 0;
    }
}