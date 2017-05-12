using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{
    /// <summary>
    /// Spezifischere Hitbox, stell einen Kreis dar
    /// </summary>
    public class KreisHitBox : BasicHitBox
    {            
        Vector2 center;
        float radius;
        public KreisHitBox(Vector2 Center ,float radius)
        :base(new Rectangle((int)(Center.X -radius + 0.5f),(int)(Center.Y -radius + 0.5f), (int)(radius*2+0.5f ), (int)(radius*2+0.5f ) ),HitBoxType.Kreis)
        {
            this.center = Center;
            this.radius = radius;
            
        }
        public KreisHitBox(Rectangle pos)
            :base(pos, HitBoxType.Kreis)
        {
            this.center = pos.Center.ToVector2();
            Debug.Assert(pos.Height == pos.Width);
            this.radius = pos.Height / (float)2;
        }
        public override bool Colides(BasicHitBox other)
        {
            //Falls sie die Bounding Hitboxen nicht schneiden kann abgebrochen werden
            if (!this.BoundingBox.Intersects(other.BoundingBox)) { return false; }
            switch (other.GetHitBoxType())
            {                
                case HitBoxType.Basic:
                    return ColidesWithBasic((BasicHitBox)other);
                case HitBoxType.Kreis:
                    return ColidesWithCircle((KreisHitBox)other);
                default:
                    return true;
            }
        }        
        
        public bool ColidesWithCircle(KreisHitBox other)
        {
            return Vector2.DistanceSquared(other.center, this.center) < Math.Pow((other.radius + this.radius), 2);
        }
        public bool ColidesWithBasic(BasicHitBox other)
        {
            //Berechnung mithilfe des umkreises der anderen Hitbox,
            //da vorherige überprüfung den sonderfall dass beide direkt nebeneinander sind und sich nicht berühren ausgeschlossen hat
            double distance = Vector2.Distance(this.center, other.BoundingBox.Center.ToVector2());
            double addRadi = this.radius + Utilities.Pytagoras(other.BoundingBox.Height,other.BoundingBox.Width)/2;
            return  distance<addRadi ;
        }               
        public void SetHitbox(Vector2 Center, float radius)
        {
            base.SetHitbox(new Rectangle((int)(Center.X -radius + 0.5f),(int)(Center.Y -radius + 0.5f), (int)(radius*2+0.5f ), (int)(radius*2+0.5f ) ),HitBoxType.Kreis);           
            this.center = Center;
            this.radius = radius;
        }




    }
}
