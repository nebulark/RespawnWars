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
    /// Lässt einem den Spielernamen festlegen
    /// </summary>
    class OptionsState:A_Menu,I_State
    {
        static readonly string menuname = "Set Name";
        static readonly string[] buttons = { "Back" };
        static readonly string[] textfields = {"Name" };
        public OptionsState(Game1 game):base(game)
        {
            base.Initialize(menuname,buttons, textfields);
        }
        

        public  Gamestate Update(GameTime gameTime)
        {
            string input = base.GetInput(gameTime);
            switch (input)
            {
                case "Back":
                    return Gamestate.Menu;
                case "Name":
                    if (!((TextField)elements[base.selected]).Active)
                    {
                        ((TextField)elements[base.selected]).Active = true;
                    }
                    else
                    {
                        ((TextField)elements[base.selected]).Active = false;
                        Utilities.PlayerName = ((TextField)elements[base.selected]).Value;
                    }
                    return Gamestate.NoChange;
            }
            return Gamestate.NoChange;
        }
        
    }
}
