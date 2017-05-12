using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Microsoft.Xna.Framework.Storage;

using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace MMP1_Prototype
{
    /// <summary>
    /// Wir aufgerufen wenn man eine Challange erhält. Diese Kann man dann Akzeptieren oder Ablehnen. Danach wird man zurück in die Lobby geleitet oder wartet
    /// bis der Herausforderer die Annahme erhalten hat und gelangt in den Ingame State
    /// </summary>
    class MatchRequestState : A_Menu, I_State
    {
        static readonly string menuname = "You have a Challenger";
        string[] buttonText = { "Accept", "Decline"};
        byte[] sendingBuffer;
               
        Game1 game;        
        public MatchRequestState(Game1 game1):base(game1)
        {
            sendingBuffer = new byte[5];
            game = game1;
           
            base.Initialize(menuname,buttonText);
           
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
            string input = base.GetInput(gameTime);          
            if (input != null)
            {
                switch(input){
                    case "Accept":
                        Utilities.GetTypeAndIPPacket(sendingBuffer, PacketType.ChallengeAccepted);
                        game.Udp.addSendingData(sendingBuffer);
                        game.Udp.Send();                                            
                        break;
                    case "Decline":
                        Utilities.GetTypeAndIPPacket(sendingBuffer, PacketType.ChalleneDenied);
                        game.Udp.addSendingData(sendingBuffer);
                        game.Udp.Send();                        
                        return Gamestate.NetworkLobby;                                          
                }
            }           
            return Gamestate.NoChange;
        }
        public Gamestate ProcessReceivedData(byte[] data)
        {                          
            switch ((PacketType)data[0])
            {
                case PacketType.ChallengeStart:                    
                    return Gamestate.Ingame;
                case PacketType.ChallengeAbort:                               
                    game.Udp.SetConnection(IPAddress.Broadcast);
                    return Gamestate.NetworkLobby;
                default:
                    return Gamestate.NoChange;                    
            }
        }
    }
}
