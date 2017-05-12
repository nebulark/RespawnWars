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
    /// Projektilart bei der das Projektil erst nach einer verzögerung losfliegt
    /// </summary>
    public class PseudoSniper : A_Projectile, I_Hitable
    {
        const int movementSpeed = 10;
        const int delayTime = 100; //100frames
        const int reach = 800;
        const byte schaden = 100;
        static readonly Vector2 size = new Vector2(16,16) ;                 
        Texture2D texture;             
        byte delay;
             
        public PseudoSniper():base() { }
       
        public override void Initialise(Color color, params Texture2D[] texture)
        {
            base.Initialise(size, movementSpeed,reach,color,schaden);
            this.texture = texture[0];       
        }

        public override void Set(Point fromCenter, Point toCenter, bool alive = true)
        {
            base.Set(fromCenter, toCenter, alive);          
            delay = delayTime;
            
          
        }      
        public override void Update()
        {
            if (!base.Alive) { return; }
            if (base.Position.Center == base.Target)
            {
                Alive = false;
            }
            if (delay > 0)
            {
                delay--;
            }
            else
            {
                base.Update();
            }           
            Hitbox = GetHitdetectionData();
   
        }
        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {            
            base.Draw(spriteBatch, texture, offset);           
       }
        public override int WriteBytes(byte[] result, int offset)
        {
            offset = base.WriteBytes(result, offset);
            result[offset] = delay;
            return offset + 1;
        }

        public override int SetFromBytes(byte[] data, int offset)
        {
            offset = base.SetFromBytes(data, offset);                   
            delay = data[offset];
            return offset + 1;
           
        }
       
        public override byte Hitdetection(BasicHitBox OtherHitBox)
        {
            if (Hitbox == null) { Hitbox = GetHitdetectionData(); }
            if (Hitbox.Colides(OtherHitBox))
            {
                //getroffen und projectil "löschen"
                base.Alive = false;
                return base.damage;
            }
            return 0;

        }
        

    }
}
