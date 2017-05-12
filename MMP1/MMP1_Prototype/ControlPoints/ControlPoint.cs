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
    /// Bringen Punkte wenn eine Einheit auf ihnen steht
    /// </summary>
    class ControlPoint : I_Hitable
    {
        private Texture2D[] textures;
        private Rectangle position;
        private BasicHitBox Hitbox;
        private const int radius = 50;

        //Für die Animation
        private int sizebg;
        private const int slowness = 2;
        private const int maxsizebg = 50;
        public ControlPoint(Texture2D[] textures, Point position)
        {
            this.textures = textures;
            this.position = new Rectangle(position.X, position.Y, radius*2, radius*2);
            this.Hitbox = new KreisHitBox(this.position.Center.ToVector2(), radius);
            sizebg = 0;
        }
        public float CalculatePoints(UnitManager unitManager)
        {
            int cnt = unitManager.GetBoxMatchesNumber(GetHitdetectionData());
            float result = 0f;
            //Dimishing returns
            for (int i = 0; i < cnt; i++)
            {
                result = result * 0.8f + 5;
            }
            return result;

        }
        public void Update()
        {            
            sizebg++;
            if (sizebg >= maxsizebg*slowness)
            {
                sizebg = 0;
            }           
        }
        public void Draw(SpriteBatch spriteBatch, Point offset)
        {
            Rectangle translatedPosition = position;
            translatedPosition.Offset(offset);            
            Rectangle rectbg = position;
            rectbg.Inflate(sizebg / slowness, sizebg / slowness);
            spriteBatch.Draw(textures[1], rectbg, Color.Blue * (1- (float)sizebg / (maxsizebg*slowness)));
            spriteBatch.Draw(textures[0], translatedPosition, Color.White );
        }

        public BasicHitBox GetHitdetectionData()
        {
            return Hitbox;
        }

    }     
            
}
