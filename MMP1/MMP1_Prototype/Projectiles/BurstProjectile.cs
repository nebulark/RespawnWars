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
    /// Projectil, das eine leichten Streuwinkel besitzt
    /// </summary>
    public class BurstProjectile: A_Projectile, I_Hitable
    {
        const int movementSpeed = 10;        
        const float spreadangle = (float)(30 * Math.PI / 180);
        const byte schaden = 60;
        const int reach = 200;
        static readonly Vector2 size = new Vector2(16,16) ;
        static Random rand = new Random();        
        Texture2D texture;            
        
        
        public BurstProjectile() : base() { }        
        public override void Initialise(Color color, params Texture2D[] texture)
        {
            base.Initialise(size, movementSpeed,reach,color,schaden);
            this.texture = texture[0];       
        }
        
        public override void Set(Point fromCenter, Point toCenter, bool alive = true)
        {
            Vector2 direction = (toCenter - fromCenter).ToVector2();
            Set(fromCenter, direction, alive);
        }
        public override void Set(Point fromCenter, Vector2 direction, bool alive = true)
        {
            direction.Normalize();
            //berechne zufälligen winkel in einem bestimmenten bereich
            float angle = (float)(spreadangle * (rand.NextDouble() * 2 - 1));
            
            direction = Vector2.Transform(direction, Matrix.CreateRotationZ(angle));
            base.Set(fromCenter, direction, alive);
        } 
        public override void Update()
        {
            if (!base.Alive) { return; }
            if (base.Position.Center == base.Target)
            {
                Alive = false;
            }                      
            base.Update();           
            Hitbox = GetHitdetectionData();
   
        }
        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {            
            base.Draw(spriteBatch, texture, offset);           
       }
        public override int WriteBytes(byte[] result, int offset)
        {
            offset = base.WriteBytes(result, offset);
            return offset;
        }

        public override int SetFromBytes(byte[] data, int offset)
        {
            offset = base.SetFromBytes(data, offset);
            return offset;           
        }        
        public override byte Hitdetection(BasicHitBox OtherHitBox)
        {
            if (Hitbox == null) { Hitbox = GetHitdetectionData(); }
            if (Hitbox.Colides(OtherHitBox))
            {
                //getroffen und projectil "löschen"
                Alive = false;
                return base.damage;
            }
            return 0;

        }
    }
}
