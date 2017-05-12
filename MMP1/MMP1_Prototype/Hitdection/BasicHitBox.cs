using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype{
    /// <summary>
    /// Zuständig für die Erzeugung von Hitboxen und deren Kolisionsdetection
    /// Kinder dieser Klasse sind spezifischere Hitboxen
    /// </summary>
    public class BasicHitBox
    {
        public enum HitBoxType
        {
            Basic,
            Kreis            
        }
        public Rectangle BoundingBox;
        HitBoxType type;

        public BasicHitBox() { }
        public BasicHitBox(Rectangle rect)
        {
            BoundingBox = rect;
            this.type = HitBoxType.Basic;
        }
        public BasicHitBox(Rectangle rect, HitBoxType type) {
            BoundingBox = rect;
            this.type = type;
        }
        public virtual bool Colides(BasicHitBox other)
        {
            if (!this.BoundingBox.Intersects(other.BoundingBox))
            {
                return false;
            }
            switch (other.type)
            {
                case HitBoxType.Basic:
                    return true;
                case HitBoxType.Kreis:
                    return ((KreisHitBox)other).ColidesWithBasic(this);
                default:
                    return true;
            }
            
        }
        public HitBoxType GetHitBoxType()
        {
            return type;
        }
        public void SetHitbox(Rectangle rect, HitBoxType type)
        {
            BoundingBox = rect;
            this.type = type;
        }
        
    }
}
