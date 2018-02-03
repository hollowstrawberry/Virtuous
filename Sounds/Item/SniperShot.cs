using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace Virtuous.Sounds.Item
{
    public class SniperShot : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
        {
            soundInstance = sound.CreateInstance();
            soundInstance.Volume = volume * 2f;
            soundInstance.Pan = pan;
            soundInstance.Pitch = -1.0f;
            return soundInstance;
        }
    }
}
