using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{
    /// <summary>
    /// Wird benutzt für Menugamestates. Bietet alle notwendigen Funktionen zum Zeichnen und Abfragen von Menues.
    /// </summary>
    abstract class A_Menu
    {
        const int offsetY = 175;
        const int width = 250;
        const int height = 150;
        const int gaps = 50;
        protected List<I_MenuElement> elements;        
        SpriteFont spriteFont;
        Texture2D[] textures;       
        string[] texturesNames = { "Button" , "TextField"};
        //Wird nur gezeichnet
        Button MenuNameButton;
        private Game1 game;              
        protected int selected;
        public A_Menu(Game1 game1)
        {            
            this.game = game1;            
            elements = new List<I_MenuElement>();           
            textures = new Texture2D[texturesNames.Length];
            spriteFont = game.font1;                     
            
        }
        public void Initialize(string MenuName, string[] ButtonText, string[] TextFieldText = null)
        {
            
            for (int i = 0; i < textures.Length; i++)
            {
                textures[i] = game.Content.Load<Texture2D>(texturesNames[i]);
            }
            this.MenuNameButton = new Button(textures[0],spriteFont,MenuName,new Rectangle(gaps, gaps/2, width * 2 + gaps, height) );
            if (ButtonText != null && ButtonText.Length >= 1)
            {
               ;
               for (int y = 0; y < ButtonText.Length; y++)
                {
                    addButton(ButtonText[y]);                
                }
            }
            selected = -1;
            if (TextFieldText == null||TextFieldText.Length == 0) { return; }
            for (int i = 0; i < TextFieldText.Length; i++)
            {
                addTextfield(TextFieldText[i]);
            }
            
        }
        /// <summary>
        /// Überprüft welcher Button gedrücktwurden und gibt einen Entsprechenden String zurück
        /// </summary>
        /// <param name="gameTime">Gametime aus Game1</param>
        /// <returns>Text des Gedrückten Knopf, null wenn keiner gedrückt wurden</returns>
        public string GetInput(GameTime gameTime)
        {
            if (Utilities.isKeyPressed(Keys.Down))
            {
                selected++;
                if (selected >= elements.Count())
                {
                    selected = 0;
                }
            }
            if (Utilities.isKeyPressed(Keys.Up))
            {

                selected--;
                if (selected < 0)
                {
                    selected = elements.Count() - 1;
                }
            }
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].DetectCollion())
                {
                    selected = i;
                    break;
                }

            }
            if ((Utilities.Leftclicked || Utilities.isKeyPressed(Keys.Enter)) && selected != -1)
            {                
               return elements[selected].getText();
               
            }
            foreach (I_MenuElement e in elements)
            {
                e.Update();
            }
            //Nichts Gedrückt
            return null;
        }
            
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            

            MenuNameButton.Draw(spriteBatch, Color.Green);
            if (elements.Count == 0) { return; }           
            for (int i = 0; i < elements.Count; i++)
            {                
                if (i == selected)
                {
                    elements[i].Draw(spriteBatch, Color.Red);
                }
                else
                {
                    elements[i].Draw(spriteBatch, Color.Blue);
                }
            }
        }
        /// <summary>
        /// Fügt einen Button dem Menu hinzu.
        /// </summary>
        /// <param name="text">Text des Buttons</param>
        public void addButton(string text)
        {
            //keine doppelten texte
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].getText() == text) { return; }
            }
            elements.Add(new Button(textures[0], spriteFont, text,
                new Rectangle(gaps + (elements.Count % 2) * (gaps + width) ,
                    gaps + (elements.Count / 2) * (gaps + height) + offsetY,
                    width, height)));
        }
        /// <summary>
        /// Fügt eine Textfeld dem Menu hinzu.
        /// </summary>
        /// <param name="text">Name/Beschreibung des Textfeldes</param>
        public void addTextfield(string text)
        {
            //keine doppelten texte
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].getText() == text) { return; }
            }
            elements.Add(new TextField(textures[1], spriteFont, text,
                new Rectangle(gaps + (elements.Count % 2) * (gaps+width),
                    gaps + (elements.Count / 2) * (gaps+height) + offsetY,
                    width, height)));
        }
       
    }
}
