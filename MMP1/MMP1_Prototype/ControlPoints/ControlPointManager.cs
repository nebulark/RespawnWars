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
    /// Fasst die Kontrolpunkte in ein Object zusammen
    /// </summary>
    class ControlPointManager
    {
        private ControlPoint[] controlPoints;        
        public ControlPointManager(Texture2D[] textures, Point[] positions)
        {
            controlPoints = new ControlPoint[positions.Length];
            for (int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i] = new ControlPoint(textures, positions[i]);
            }
        }
        public void Update()
        {
            for (int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i].Update();
            }
        }
        public void Draw(SpriteBatch spriteBatch, Point offset)
        {
            for (int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i].Draw(spriteBatch, offset);
            }
        }
        public void AddPoints(LocalPlayer player)
        {
            
            float sum = 0;
            for (int i = 0; i < controlPoints.Length; i++)
            {
                sum += controlPoints[i].CalculatePoints(player.unitManager);                
            }
            player.AddScore(sum);

        }
    }
}
