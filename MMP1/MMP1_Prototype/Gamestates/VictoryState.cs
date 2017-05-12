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
   /// Siegesscreen
   /// </summary>
    class VictoryState:A_Menu, I_State
    {
        static readonly string menuname = "Victory";
        string[] buttons = new string[] { "Back to Menu" };
        Game1 game;
        SendingManager sendingManager;
        public VictoryState(Game1 game)
            : base(game)
        {
            this.game = game;
            sendingManager = new SendingManager();
            sendingManager.addItem(new byte[] { (byte)PacketType.Victory });
            base.Initialize(menuname,buttons);
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
        //bearbeite Daten, sie werden aber nicht verwendet
        Gamestate ProcessReceivedData(byte[] data){
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
