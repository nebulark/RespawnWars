using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{
    /// <summary>
    /// Projektilart die beim zusammenstoß oder erreichen des Zieles explodiert
    /// </summary>
    class RocketProjectile : A_Projectile, I_Hitable
    {                      
        const int movementSpeed = 5;
        const int explosionFrames = 10;
        const int explosionRadius = 40;
        //Hitdetection wird jedes 2te frame durchgeführt
        const byte schaden = (byte)(100 / explosionFrames +0.5) * 2;
        const int reach = 400;
        static readonly Rectangle explosionSize = new Rectangle(0, 0, explosionRadius * 2, explosionRadius * 2);        
        static readonly Vector2 size = new Vector2(10,10);   
        private Texture2D texture;
        private Texture2D explosionTexture;                  
        
        //0 -> keine explosion, bei explosion zählt von 1 bis explosionFrames;
        private byte exploding;
        
        public RocketProjectile():base() { }       
        public override void Initialise(Color color,params Texture2D[] texture)
        {
            base.Initialise(size, movementSpeed,reach,color,schaden, false);
            this.texture = texture[0];
            this.explosionTexture = texture[1];
        }

        public override void Set(Point fromCenter, Point toCenter, bool alive = true)
        {
            base.Set(fromCenter, toCenter, alive);
            exploding = 0;                     
        }      
      
        public override void Update()
        {
            if (!base.Alive) { return; }
            
            if (exploding == 0)
            {
                
                base.Update();
                if (base.Position.Center == base.Target) { exploding = 1; }
            }
            else
            {
                exploding++;
                if (exploding > explosionFrames)
                {
                    exploding = 0;
                    Alive = false;
                }
                return;
            }
            Hitbox = base.GetHitdetectionData();
        }
        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            if (!Alive) { return; }
            if (exploding == 0)
            {
                base.Draw(spriteBatch, texture, offset);                
            }
            else
            {
                Rectangle drawLoc = explosionSize;
                drawLoc.Offset(base.Position.Center - drawLoc.Center );                
                spriteBatch.Draw(explosionTexture, drawLoc, Color.White);
            }
            
        }
        public override int WriteBytes(byte[] result, int offset)
        {
            offset = base.WriteBytes(result, offset);
            result[offset] = exploding;
            return offset + 1;
        }

        public override int SetFromBytes(byte[] data, int offset)
        {
            offset = base.SetFromBytes(data, offset);
            exploding = data[offset];
            return offset + 1;
           
        }
        public override BasicHitBox GetHitdetectionData()
        {
            if(exploding == 0){
                return base.GetHitdetectionData();
            }
            return new KreisHitBox(base.Position.Center.ToVector2(), explosionRadius);       
        }
        public override byte Hitdetection(BasicHitBox OtherHitBox)
        {            
            if (Hitbox.Colides(OtherHitBox))
            {
                //getroffen und explodieren
                if (this.exploding == 0) { exploding = 1; }
                return base.damage;
            }
            return 0;

        }
    }
}
