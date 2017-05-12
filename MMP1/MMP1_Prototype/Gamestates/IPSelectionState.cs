using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace MMP1_Prototype
{
    /// <summary>
    /// Lässt den Spieler seine IPAdresse auswählen (falls er mehrer Netzwerkadapter besitzt).
    /// Leitet beim Klicken weiter in die Network Lobby
    /// </summary>
    class IPSelectionState: A_Menu, I_State
    {               
        string[] ipAdresses;
        static readonly string menuname = "Select IP Adress";
        public IPSelectionState(Game1 game)
            : base(game)
        {
            Initialize();
            base.Initialize(menuname,ipAdresses);
        }
        public Gamestate Update(GameTime gametime)
        {
            string clickedButton = base.GetInput(gametime);
            if (clickedButton == null) { return Gamestate.NoChange; }
            Utilities.LocalAdress = IPAddress.Parse(clickedButton);
            return Gamestate.NetworkLobby;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public void Initialize()
        {
           
            ipAdresses = Utilities.GetOwnIPAdresses();
        }

       
       
    }
}
