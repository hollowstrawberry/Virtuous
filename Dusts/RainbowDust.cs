using Terraria;
using Terraria.ModLoader;

namespace Virtuous.Dusts
{
    public class RainbowDust : ModDust
    {
        const float LightDivisor = 500f;


        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
            dust.noGravity = true;
            dust.position.X -= 1;
            dust.position.Y -= 1;
        }


        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += 0.2f;
            dust.scale -= 0.05f;

            if (dust.scale < 0.5f) dust.active = false;

            Lighting.AddLight(dust.position, dust.color.R/LightDivisor, dust.color.G/LightDivisor, dust.color.B/LightDivisor);

            return false;
        }
    }
}