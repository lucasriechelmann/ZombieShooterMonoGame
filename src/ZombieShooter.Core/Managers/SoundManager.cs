using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace ZombieShooter.Core.Managers;

public class SoundManager
{
    AudioListener _audioLister;
    List<SoundEffectInstance> _sfxToDispose;
    public SoundManager()
    {
        _audioLister = new();
        _sfxToDispose = new();
    }
    public void SetPosition(Vector3 position) => _audioLister.Position = position;
    public void Play(SoundEffectInstance sfx, AudioEmitter emitter)
    {
        sfx.Apply3D(_audioLister, emitter);
        sfx.Play();
        _sfxToDispose.Add(sfx);
    }
    public void CleanSFX()
    {
        List<SoundEffectInstance> toDispose = new();
        foreach (SoundEffectInstance sfx in _sfxToDispose)
            if (sfx.State != SoundState.Playing)
                toDispose.Add(sfx);

        foreach(SoundEffectInstance sfx in toDispose)
        {
            sfx.Dispose();
            _sfxToDispose.Remove(sfx);
        }
    }
}
