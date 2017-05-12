using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;


namespace MMP1_Prototype
{
    
    class Button : I_MenuElement
    {
        Texture2D buttonTexture;
        Rectangle position;
        SpriteFont font;
        string text;

        Point positionEnd
        {
            get { return  (position.Location + position.Size); }
        }        

        public Button(Texture2D buttonTexture, SpriteFont spriteFont, string text, Rectangle postion)
        {
            this.buttonTexture = buttonTexture;
            this.position = postion;
            this.text = text;
            this.font = spriteFont;
            
            
        }
        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            Debug.Assert(buttonTexture != null);
            Debug.Assert(position != null);            
            spriteBatch.Draw(buttonTexture, position, color);
            //Zentriert die Schrift: (Ende - Anfang)/2 = Mitte
            //Mitte - Textgröße/2 = gewünsche Startposition
            spriteBatch.DrawString(font, text, position.Center.ToVector2() - font.MeasureString(text)/2,Color.Black);
        }             
        public bool DetectCollion()
        {
            return position.Contains(Utilities.CurrentMouseState.Position);            
        }
        public void Update() { }
        public void Clicked(){}
        public string getText()
        {
            return this.text;
        }


       
    }
}
