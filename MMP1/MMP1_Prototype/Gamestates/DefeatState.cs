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
    /// Menustate. Wird aufgerufen wenn man verliert.
    /// </summary>
    class DefeatState : A_Menu, I_State
    {
        static readonly string Menuname = "You Lost";
        string[] buttons = new string[] { "Back to Menu" };
        Game1 game;
        SendingManager sendingManager;
        public DefeatState(Game1 game)
            : base(game)
        {
            this.game = game;
            sendingManager = new SendingManager();
            sendingManager.addItem(new byte[] { (byte)PacketType.Defeat });
            base.Initialize(Menuname,buttons);
        }
        public Gamestate Update(GameTime gameTime)
        {
            Gamestate nextState = Gamestate.NoChange;
            byte[] receivedData;
            receivedData = game.Udp.GetNextPacket();
            while (receivedData != null)
            {
                nextState = ProcessReceivedData(receivedData);
                if (nextState != Gamestate.NoChange) { return nextState; }
                receivedData = game.Udp.GetNextPacket();
            }
            game.Udp.addSendingData(sendingManager.GetData());
            game.Udp.Send();
            string input = base.GetInput(gameTime);
            switch (input)
            {
                case "Back to Menu":
                    return Gamestate.Menu;
            }
            return Gamestate.NoChange;

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }        
        Gamestate ProcessReceivedData(byte[] data)
        {
            switch ((PacketType)data[0])
            {
                case PacketType.Promise:
                    //Packet erhalten, Lesebestätigung
                    game.Udp.addSendingData(new byte[] { (byte)PacketType.PromiseResponse, data[1] });
                    break;
            }
            return Gamestate.NoChange;

        }
    }
}

