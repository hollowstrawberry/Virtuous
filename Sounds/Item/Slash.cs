using Microsoft.Xna.Framework.Audio;
using Terraria.ModLoader;

namespace Virtuous.Sounds.Item
{
    public class Slash : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
        {
            soundInstance = sound.CreateInstance();
            soundInstance.Volume = volume * 0.1f;
            soundInstance.Pan = pan;
            soundInstance.Pitch = -0.25f;
            return soundInstance;
        }
    }
}
