using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MMP1_Prototype
{
    class TextField : I_MenuElement
    {
        static readonly Point size = new Point(200, 150);
        Texture2D texture;
        Rectangle position;
        SpriteFont font;
        string description;
        public string Value = "";
        public bool Active;

        #region properties
        Point rightDown
        {
            get { return new Point(position.Right, position.Bottom); }
        }
        Vector2 descPos
        {
            get
            {
                return new Vector2(
                    position.X + ((size.X - font.MeasureString(description).X) / 2),
                    position.Y + font.MeasureString(description).Y -10
                    );
            }
        }
        Vector2 valuePos
        {
            get
            {
                return new Vector2(
                    position.X  + ((size.X - font.MeasureString(Value).X) / 2),
                    rightDown.Y - font.MeasureString(Value).Y -30
                    );
            }
        }
        #endregion
        public TextField(Texture2D texture, SpriteFont spriteFont, string description, Rectangle Position)
        {
            this.texture = texture;
            this.position = Position;
            this.description = description;
            this.font = spriteFont;            
        }
       
        public void Draw(SpriteBatch spriteBatch,Color color)
        {
            Color drawingColor = Active ? Color.Red : Color.Blue;
            spriteBatch.Draw(texture, position, drawingColor);
            //Zentriert die Schrift: (Ende - Anfang)/2 = Mitte
            //Mitte - Textgröße/2 = gewünsche Startposition            
            spriteBatch.DrawString(font, description, descPos, Color.Black);
            spriteBatch.DrawString(font, Value, valuePos, Color.White);
            
        }

        public bool DetectCollion()
        {
            return position.Contains(Utilities.CurrentMouseState.Position);
        }
        public void Clicked()
        {
            Active = !Active;            
        }        
        public void Update()
        {
            if (Active)
            {
                bool enterpressed = (Utilities.WriteInput(ref Value) & 0x01) != 0;
            }
        }
        public string getText()
        {
            return this.description;
        }
        
    }
}
