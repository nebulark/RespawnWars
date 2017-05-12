using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace MMP1_Prototype
{
    /// <summary>
    /// Localer Spieler
    /// Verarbeitet Tastatur/MausInput
    /// </summary>
    class LocalPlayer : I_Player
    {
             
        public byte PlayerNr = 0;
        public const float WinningScore = 50000;
        public ProjectileManager projectileFactory;
        public  UnitManager unitManager;
        private SpriteFont font1;
        private Color farbe;
        public  float Score;        
        //Punkt ab dem die linke Maustaste gehalten wurde
        Point drag;

        public LocalPlayer(Texture2D[] projectileTextures, Texture2D[] unitTextures, SpriteFont font, Color color)
        {
            projectileFactory = new ProjectileManager(projectileTextures, color);
           
            font1 = font;
            unitManager = new UnitManager(unitTextures, color);
            Score = 0f;
            farbe = color;
            
        }
        public void Initialize( byte nr )
        {
 
            PlayerNr = nr;
            drag = Utilities.CurrentMouseState.Position;

        }
        public void AddScore(float points)
        {
            Score += points;
        }       
       
        public void Update(GameTime gameTime, Point offset) {
           
            //Zieht Auswahlfenster
            if (Utilities.Leftclicked)
            {
                drag = Utilities.CurrentMouseState.Position;                               
            }
            //Beim Loslassen der Linken Maustaste wird alles im Auswahlfenster Ausgewählt
            if (Utilities.LeftReleased)                
            {
                unitManager.selectWithinBox(Utilities.GetRectangleFromOposingCorners(drag, Utilities.CurrentMouseState.Position));                       
            }           

            //Maus muss sich im Fenster befinden
            if (Utilities.Rightclicked
             && IngameState.FieldSize.Contains(Utilities.CurrentMouseState.Position))
            {

                if (Utilities.ControlPressed)
                {
                    unitManager.selectedUnitsShootAt(Utilities.CurrentMouseState.Position, projectileFactory);                    
                }
                else if (Utilities.CurrentKeyboardState.IsKeyDown(Keys.A))
                {
                    unitManager.CreateUnit(Utilities.CurrentMouseState.Position - offset, UnitType.RocketMan);
                }
                else if (Utilities.CurrentKeyboardState.IsKeyDown(Keys.S))
                {
                    unitManager.CreateUnit(Utilities.CurrentMouseState.Position - offset, UnitType.Sniper);
                }
                else if (Utilities.CurrentKeyboardState.IsKeyDown(Keys.Y))
                {
                    unitManager.CreateUnit(Utilities.CurrentMouseState.Position - offset, UnitType.Burst);
                }                 
                else
                {
                    unitManager.selectedUnitsMoveTo(Utilities.CurrentMouseState.Position - offset);                                    
                }
            }
            //Berechnet Neue Position für alle Units
            unitManager.Update();
            projectileFactory.Update();
            int pressedNumber = Utilities.GetNumberPressed();
            //Wenn pressedNumber == -1 -> Keine Taste gedrückt
            if (pressedNumber != -1)
            {
                unitManager.selectNumberPressedfromSelected(pressedNumber);             
            }

           
        }
         public void Draw(SpriteBatch spritebatch, Point offset)
        {
            projectileFactory.Draw(spritebatch, offset);
            unitManager.Draw(spritebatch, offset);
            unitManager.Display(spritebatch);
            
            if (Utilities.CurrentMouseState.LeftButton == ButtonState.Pressed && Utilities.LastMouseState.LeftButton == ButtonState.Pressed)
            {
                Utilities.DrawRectangle(spritebatch, drag, Utilities.CurrentMouseState.Position, Color.Black);
            }
            spritebatch.DrawString(font1,  String.Format( "Your Score : {0:0}" , Score), IngameState.ScorePos[0], Color.White);
        }
         public int WriteBytes(byte [] result, int offset=0)
         {
             
             result[0 + offset] = (byte)PacketType.GameData;
             //playerNr ist im moment egal
             result[1 + offset] = PlayerNr;
             result[2 + offset] = IngameState.Takt;
             Array.Copy(BitConverter.GetBytes(Score), 0, result, 3, 4);
             offset = 7;
             offset = unitManager.WriteBytes(result, offset);
             offset = projectileFactory.WriteBytes(result, offset);
             
             return offset;                        
         }
         // 0 -> nix, 1 -> sieg, -1 ->niederlage
         public int GetWinningStatus()
         {             
             if (unitManager.DefeatLimitReached()) { return -1; }
             //PunkteLimitErreicht
             if (Score >= WinningScore) { return +1; }
             return 0;          
         }
        
       
    }
}
