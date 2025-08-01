using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace YonduArrow.Content.Projectiles
{
    public class YonduArrowProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Yondu's Arrow");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            //Projectile.penetrate = 1;
            Projectile.light = 0.8f;
            Projectile.aiStyle = 9;
            //Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 0);

        public override void AI()
        {
            // Play sound while moving
            if (Projectile.soundDelay == 0 && Projectile.velocity.Length() > 2f)
            {
                Projectile.soundDelay = 10;
                //SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }

            // Dust effect (optional)
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 150, default, 1.2f);
                dust.noGravity = true;
            }

            // Magic missile AI
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 0f)
            {
                Player player = Main.player[Projectile.owner];
                if (player.channel)
                {
                    float maxDistance = 18f;
                    Vector2 toCursor = Main.MouseWorld - Projectile.Center;
                    float dist = toCursor.Length();
                    if (dist > maxDistance)
                    {
                        toCursor *= maxDistance / dist;
                    }

                    int vx = (int)(toCursor.X * 1000f);
                    int ovx = (int)(Projectile.velocity.X * 1000f);
                    int vy = (int)(toCursor.Y * 1000f);
                    int ovy = (int)(Projectile.velocity.Y * 1000f);

                    if (vx != ovx || vy != ovy)
                        Projectile.netUpdate = true;

                    Projectile.velocity = toCursor;
                }
                else if (Projectile.ai[0] == 0f)
                {
                    Projectile.netUpdate = true;
                    float maxDistance = 14f;
                    Vector2 toCursor = Main.MouseWorld - Projectile.Center;
                    float dist = toCursor.Length();
                    if (dist == 0f)
                    {
                        toCursor = Projectile.Center - player.Center;
                        dist = toCursor.Length();
                    }
                    toCursor *= maxDistance / dist;
                    Projectile.velocity = toCursor;
                    if (Projectile.velocity == Vector2.Zero)
                        Projectile.Kill();
                    Projectile.ai[0] = 1f;
                }
            }

            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        //public override void Kill(int timeLeft)
        //{
        //    SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        //    for (int i = 0; i < 10; i++)
        //    {
        //        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 0, 0, 100, default, 1.2f);
        //    }
        //}
    }
}