using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace ZombieShooter.Core.Components;

public class FootstepComponent
{
    AudioEmitter _audioEmitter;
    float _distanceAcumulator;
    float _stepThreshold;
    SoundEffect _stepSFX;
    public FootstepComponent(SoundEffect stepSound, float stepThreshold = 30f)
    {
        _stepSFX = stepSound;
        _distanceAcumulator = 0f;
        _stepThreshold = stepThreshold;
        _audioEmitter = new();
    }
    public SoundEffectInstance SFX => _stepSFX.CreateInstance();
    public AudioEmitter Emitter => _audioEmitter;
    public void SetPosition(Vector3 position) => _audioEmitter.Position = position;     
    public bool ShouldPlay() => _distanceAcumulator >= _stepThreshold;
    public void AddToAcumulator(float distanceMoved) => _distanceAcumulator += distanceMoved;
    public void ResetAcumulator() => _distanceAcumulator = 0;
    
}
