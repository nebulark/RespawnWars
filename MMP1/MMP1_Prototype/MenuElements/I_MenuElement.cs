using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input; 
using Microsoft.Xna.Framework.Content;
namespace MMP1_Prototype
{
    interface I_MenuElement
    {
        void Draw(SpriteBatch spritebatch, Color color);
        void Update();        
        void Clicked();
        /// <summary>
        /// Überprüft ob sich der Mauszeiger über dem Menüelement befindet
        /// </summary>
        /// <returns></returns>
        bool DetectCollion();
        string getText();
    }
}
