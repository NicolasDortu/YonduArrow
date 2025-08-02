using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace YonduArrow.Content.Projectiles
{
    public class YonduArrowProjectile : ModProjectile
    {
        /// <summary>
        ///  TO DO: 
        ///  smooth moving (check rainbow rod)
        ///  ability to ride the arrow
        ///  Esthetic -> normal looking arrow with a red trail (if possible, make the red parts glows)
        /// </summary>
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.light = 0.1f;
            //Projectile.aiStyle = 9;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            //DrawOriginOffsetX = 1;
        }

        // public override Color? GetAlpha(Color lightColor) => new Color(255, 0, 0, 50);

        public override void AI()
        {
            // Play sound while moving
            if (Projectile.soundDelay == 0 && Projectile.velocity.Length() > 2f)
            {
                Projectile.soundDelay = 10;
                //SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }

            // Dust effect (optional)
            //if (Main.rand.NextBool(2))
            //{
            //    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 150, default, 1.2f);
            //    dust.noGravity = true;
            //}

            // Lighting.AddLight(Projectile.Center, new Color(255, 0, 0, 255).ToVector3());

            // Magic missile AI
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 0f)
            {
                Player player = Main.player[Projectile.owner];
                // behavior durring channeling
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

                    //Projectile.spriteDirection = Projectile.velocity.X >= 0 ? 1 : -1;

                    //Projectile.rotation = Projectile.velocity.ToRotation();

                    //if (Projectile.velocity == Vector2.Zero)
                    //    Projectile.spriteDirection = Projectile.direction;
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

        //public override void OnKill(int timeLeft)
        //{
        //    SoundEngine.PlaySound(SoundID.Dig, Projectile.position); // Plays the basic sound most projectiles make when hitting blocks.
        //    for (int i = 0; i < 5; i++) // Creates a splash of dust around the position the projectile dies.
        //    {
        //        Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Silver);
        //        dust.noGravity = true;
        //        dust.velocity *= 1.5f;
        //        dust.scale *= 0.9f;
        //    }
        //}
    }
}