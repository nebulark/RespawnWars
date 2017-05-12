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
    /// <summary>
    /// Startstate des Spieles
    /// </summary>
    class MenuState : A_Menu, I_State
    {
        static readonly string menuname = "Main Menu";
        readonly string[] buttonTextes = { "Set Name", "Networkgame" };
        public MenuState(Game1 game):base(game)
        {
            base.Initialize(menuname,buttonTextes);
        }
        public Gamestate Update(GameTime gametime)
        {
            string clickedButton = base.GetInput(gametime);
            if (clickedButton == null) { return Gamestate.NoChange; }
            switch (clickedButton)
            {
                case "Start":
                    return Gamestate.Ingame;
                case "Set Name":
                    return Gamestate.EnterName;
                case "Networkgame":
                    return Gamestate.IPSelection;
                default:
                    return Gamestate.NoChange;
            }

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        
    }
}
