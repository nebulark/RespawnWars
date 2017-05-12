using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{
    /// <summary>
    /// Verschießt 3 Projektile auf einmal dafür ungenau
    /// </summary>
    public class BurstUnit : A_Unit, I_Hitable
    {
        const int projectileCount = 3;
        static readonly Vector2 size = new Vector2(30, 30);
        static readonly ProjectileType type = ProjectileType.Burst;
        const byte cooldown = 240;
        const byte speed = 4;
        const byte standartHealth = 100;
        Texture2D texture;
        public BurstUnit()
            : base()  {}
       
        public override void Initialize(Color color, Texture2D texture)
        {
            base.Initialize(size, type,color);
            this.texture = texture;
        }              
        
        public override int SetFromBytes(byte[] data, int offset, int taktruckstand = 0)
        {
            offset = base.SetFromBytes(data, offset, taktruckstand);
            return offset;
        }
        public override bool isAlive()
        {
            return base.isAlive();
        }
        public override bool isDieing()
        {
            return base.isDieing();
        }

        public override int WriteBytes(byte[] result, int offset)
        {
            offset = base.WriteBytes(result, offset);
            return offset;
        }
        public override void Update()
        {
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            base.Draw(spriteBatch, texture, offset);
        }
        public override BasicHitBox GetHitdetectionData()
        {
            return base.GetHitdetectionData();
        }
        public override int Hitdetection(ProjectileManager projectileManager)
        {
            return base.Hitdetection(projectileManager);
        }
        public override void Display(SpriteBatch spriteBatch, Rectangle pos)
        {
            base.Display(spriteBatch, texture, pos);
        }
        public override void Shoot(Point targetCenter, ProjectileManager factory)
        {
            if (base.reattackTime == 0 && isReady())
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    factory.CreateProjectile(Position.Center, targetCenter, WeaponType);
                }               
                reattackTime = GetMaxCooldown();
            }   
        }
        public override byte GetBaseSpeed()
        {
            return speed;
        }
        public override byte GetBaseHealth()
        {
            return standartHealth;
        }
        public override ProjectileType GetBaseProjectile()
        {
            return type;
        }
        public override int GetMaxCooldown()
        {
            return cooldown;
        }
    }
    
}
