using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace MMP1_Prototype
{
    /// <summary>
    /// Zeichnet Maus auf den Bildschirm (und gegebenfalls Text)
    /// </summary>
    class UI
    {               
        private Texture2D mouseImage;
        private SpriteFont font1;
        private Vector2 fontpos;

        //Debug
        public static string output;            
        public static string Perm ="";
        public void LoadContent(SpriteFont font, Vector2 pos, Texture2D mi)
        {
            font1 = font;
            fontpos = pos;
            output = "test";
            mouseImage = mi;
        }
        
        public void Update(GameTime gametime) {
            //Debug
           /* if (gametime.IsRunningSlowly) { output += "tooslow  "; }
             else { output += "fastenough"; }
            output += "  " + Perm;*/
        }
        public void Draw(SpriteBatch spriteBatch) {
           /* try
            {
                spriteBatch.DrawString(font1, output, fontpos, Color.LightGreen);
                output = "";
            }
            catch (Exception e) { throw new Exception(e.Message+"   "+output+"  "+(int)output[output.Length-1]); }*/
            spriteBatch.Draw(mouseImage, Utilities.CurrentMouseState.Position.ToVector2(),Color.White);
       
        }
        public void WriteAt(SpriteBatch spriteBatch, string text, Vector2 position)
        {
            spriteBatch.DrawString(font1, text, position, Color.Black);
        }
    }
}
