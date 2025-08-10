using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using YonduArrow.Content.Dusts;

namespace YonduArrow.Content.Projectiles
{
    public class YonduArrowProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 0;
        }

        // ******** //
        // Glowmask //
        // ******** //
        public override void PostDraw(Color lightColor)
        {
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

        // ******** //
        // Behavior //
        // ******** //
        private Vector2 toCursor = Vector2.UnitX;
        private bool isRidingArrow = false;
        private int groundCollisionCounter = 0;

        public override void AI()
        {
            // Red lighting effect
            Lighting.AddLight(Projectile.Center, 0.3f, 0f, 0f);

            // Dust
            // Issue is that the dust starts at the center but using an offset break the smoothness
            // Because dust spawned are based on Projectile.Center
            // So i delete the dusts hitboxed when the velocity is zero
            int dustCount = 3;
            float spacing = 6f;

            if (Projectile.velocity != Vector2.Zero)
            {
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                Vector2 centerProj = Projectile.Center;

                for (int i = 0; i < dustCount; i++)
                {
                    Vector2 offset = direction * i * -spacing;
                    Vector2 dustPos = centerProj + offset;

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
            else 
            {
                Rectangle arrowHitbox = Projectile.Hitbox;
                for (int i = 0; i < Main.maxDust; i++)
                {
                    Dust dust = Main.dust[i];
                    if (dust != null && dust.active)
                    {
                        if (dust.type == ModContent.DustType<RedTrail>())
                        {
                            if (arrowHitbox.Contains(dust.position.ToPoint()))
                            {
                                dust.active = false;
                            }
                        }
                    }
                }
            }


            // Behavior
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 0f)
            {
                Player player = Main.player[Projectile.owner];
                // Behavior during channeling
                if (player.channel)
                {
                    Projectile.timeLeft = 2; // avoid despawning the arrow after 30 seconds
                    player.GetModPlayer<Players.YonduPlayer>().yonduArrowChanneled = true;
                    bool hasHat = player.GetModPlayer<Players.YonduPlayer>().yonduHelmetEquipped;

                    // Riding Arrow behavior
                    Rectangle arrowHitbox = Projectile.Hitbox;
                    Rectangle playerHitbox = player.Hitbox;

                    if (player.controlUp && arrowHitbox.Intersects(playerHitbox) && groundCollisionCounter == 0 && hasHat)
                    {
                        isRidingArrow = true;
                    }
                    else if (groundCollisionCounter > 0 && hasHat)
                    {
                        if (player.controlUp && arrowHitbox.Intersects(playerHitbox) && groundCollisionCounter > collisionCounterValue * 0.8f)
                            player.velocity.Y = -3f; // avoid getting stuck in the ground
                        isRidingArrow = false;
                        groundCollisionCounter--;
                    }

                    if (isRidingArrow)
                    {
                        // Limit arrow speed while riding
                        float maxRideSpeed = 10f;
                        float minRideDistance = 5f;
                        Vector2 toCursor = Main.MouseWorld - Projectile.Center;
                        float dist = toCursor.Length();
                        if (dist < minRideDistance)
                        {
                            Projectile.velocity = Vector2.Zero;
                        }
                        else
                        {
                            if (dist > maxRideSpeed)
                            {
                                toCursor *= maxRideSpeed / dist;
                            }
                            Projectile.velocity = toCursor;
                        }

                        float handOffsetY = 35f; // Offset to looks like it's holding the arrow
                        player.position = new Vector2(
                            Projectile.Center.X - player.width / 2f,
                            Projectile.Center.Y + handOffsetY - player.height
                        );
                        player.velocity = Vector2.Zero;

                        player.noFallDmg = true;

                        // Face direction of arrow
                        if (Projectile.velocity.X != 0)
                            player.direction = Projectile.velocity.X > 0 ? 1 : -1;

                        if (!player.controlUp)
                            isRidingArrow = false;
                    }
                    else
                    {
                        // Non-riding behavior
                        float maxDistance = 22f;
                        Vector2 toCursor = Main.MouseWorld - Projectile.Center;
                        float dist = toCursor.Length();
                        if (dist > maxDistance)
                        {
                            toCursor *= maxDistance / dist;
                        }
                        Projectile.velocity = toCursor;
                    }

                    int vx = (int)(toCursor.X * 1000f);
                    int ovx = (int)(Projectile.velocity.X * 1000f);
                    int vy = (int)(toCursor.Y * 1000f);
                    int ovy = (int)(Projectile.velocity.Y * 1000f);

                    if (vx != ovx || vy != ovy)
                        Projectile.netUpdate = true;

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
                    Projectile.timeLeft = 200;
                }
            }
        }


        // ********* //
        // Collision //
        // ********* //
        private int collisionCounterValue = 50;
        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

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
                groundCollisionCounter = collisionCounterValue;
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