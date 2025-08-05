using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using YonduArrow.Content.Dusts;
using YonduArrow.Content.Players;
using static Terraria.ModLoader.ModContent;

namespace YonduArrow.Content.Projectiles
{
    public class YonduArrowProjectile : ModProjectile
    {
        ///  TO DO: 
        ///  ability to ride the arrow
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 72; //108
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        // Draw the projectile glowmask
        public override void PostDraw(Color lightColor)
        {
            // Load the glowmask texture
            Texture2D glowTexture = ModContent.Request<Texture2D>(
                "YonduArrow/Content/Projectiles/YonduArrowProjectile_Glow",
                ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            // Smooth sinusoidal pulse
            float sine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f);
            float rawPulse = (sine + 1f) / 2f;
            float easedPulse = rawPulse * rawPulse * (3f - 2f * rawPulse); // smoothstep easing
            float intensity = MathHelper.Lerp(0.6f, 1.3f, easedPulse); // range of brightness

            Color glowColor = new Color(
                (int)(255 * intensity),
                (int)(40 * intensity),
                (int)(40 * intensity),
                255
            );

            Main.EntitySpriteDraw(
                glowTexture,
                Projectile.Center - Main.screenPosition,
                new Rectangle(0, 0, glowTexture.Width, glowTexture.Height),
                glowColor,
                Projectile.rotation,
                new Vector2(glowTexture.Width / 2f, glowTexture.Height / 2f),
                Projectile.scale,
                SpriteEffects.None,
                0
            );
        }

        private Vector2 lastDirection = Vector2.UnitX;

        public override void AI()
        {
            // Add red lighting effect
            Lighting.AddLight(Projectile.Center, 0.2f, 0f, 0f);

            //// Play sound while moving
            //if (Projectile.soundDelay == 0 && Projectile.velocity.Length() > 2f)
            //{
            //    Projectile.soundDelay = 10;
            //    SoundEngine.PlaySound(SoundID.Item39, Projectile.position);
            //}

            // Draw the Red trail behind

            // check if projectile.spritedirection could help

            if (Projectile.velocity.LengthSquared() > 0.01f)
            {
                lastDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            }
            // Using the last known direction
            {
                Vector2 direction = lastDirection;

                int dustCount = 3;
                float spacing = 6f;

                // Vector2 tailOffset = direction * (Projectile.width / 2f);
                if (Projectile.velocity != Vector2.Zero)
                {
                    for (int i = 0; i < dustCount; i++)
                    {
                        Vector2 offset = direction * i * -spacing;
                        Vector2 dustPos = Projectile.Center + offset; //+ new Vector2(0f, -4f);

                        Dust dust = Dust.NewDustDirect(
                            dustPos,
                            0, 0,
                            ModContent.DustType<RedTrail>()
                        );

                        dust.scale = 1f;
                        dust.noGravity = true;
                        dust.velocity = Vector2.Zero;
                        dust.rotation = direction.ToRotation()+ MathHelper.PiOver2;
                        dust.fadeIn = 2.0f;
                        dust.color = new Color(255, 0, 0, 255);
                    }
                }


                // Similar to magic missile AI
                if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 0f)
                {
                    Player player = Main.player[Projectile.owner];
                    // behavior durring channeling
                    if (player.channel)
                    {
                        player.GetModPlayer<Players.YonduPlayer>().yonduArrowChanneled = true;
                        // Debug
                        //if (Main.myPlayer == Projectile.owner)
                        //{
                        //    var modPlayer = Main.player[Projectile.owner].GetModPlayer<Players.YonduPlayer>();
                        //    Main.NewText($"yonduHelmetChanneled: {modPlayer.yonduArrowChanneled}");
                        //}
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

                        // face the arrow in the pointed direction
                        if (Projectile.velocity != Vector2.Zero)
                        {
                            Projectile.rotation = Projectile.velocity.ToRotation();

                            // Store last non-zero rotation in localAI[0]
                            Projectile.localAI[0] = Projectile.rotation;
                        }
                        else
                        {
                            // Use stored rotation if velocity is zero
                            Projectile.rotation = Projectile.localAI[0];
                        }
                    }
                    // if the player stop channeling
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
                        Projectile.rotation = Projectile.velocity.ToRotation();

                        Projectile.ai[0] = 1f;
                        Projectile.timeLeft = 180;
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            //SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            // Retrieve is the player is channeling
            Player player = Main.player[Projectile.owner];

            // If the projectile hits the left or right side of the tile, reverse the X velocity
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X - 1;
            }

            // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y - 1;
            }

            // Only update rotation when NOT channeling
            if (!player.channel && Projectile.velocity != Vector2.Zero)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.localAI[0] = Projectile.rotation;
            }

            return false;
        }
    }
}