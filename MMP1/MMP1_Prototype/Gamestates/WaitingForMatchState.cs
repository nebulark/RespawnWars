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
    /// State in dem man auf die Annahme oder Ablehnung der Herausforderung wartet
    /// </summary>
    class WaitingForMatchState : A_Menu, I_State
    {
        
        Game1 game;
        static readonly string menuname = "Waiting for Opponent";
        string[] buttonText = { "Abort"};        
        byte[] sendingBuffer;
        public WaitingForMatchState(Game1 game1):base(game1)
        {
            //Wichtig als erstes!!!
            game = game1;
            sendingBuffer = new byte[5];           
            Utilities.GetTypeAndIPPacket(sendingBuffer, PacketType.ChallengeRequest);
            game.Udp.addSendingData(sendingBuffer);
            game.Udp.Send();
            
            base.Initialize(menuname,buttonText);
        }      
       
        public Gamestate Update(GameTime gameTime)
        {
            Gamestate nextstate = Gamestate.NoChange;
            byte[] receivedData;
            receivedData = game.Udp.GetNextPacket();
            while (receivedData != null)
            {
                nextstate = ProcessReceivedData(receivedData);
                if (nextstate != Gamestate.NoChange) { return nextstate; }
                receivedData = game.Udp.GetNextPacket();
            }

            string input = base.GetInput(gameTime);
            if (input != null)
            {
                if (input == "Abort")
                {
                    Utilities.GetTypeAndIPPacket(sendingBuffer, PacketType.ChallengeAbort);
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
                case PacketType.ChallengeAccepted:
                    Utilities.GetTypeAndIPPacket(sendingBuffer, PacketType.ChallengeStart);
                    game.Udp.addSendingData(sendingBuffer);
                    game.Udp.Send();                                         
                    return Gamestate.Ingame;
                case PacketType.ChalleneDenied:
                    game.Udp.SetConnection(IPAddress.Broadcast);
                    return Gamestate.NetworkLobby;
                default:
                    return Gamestate.NoChange;     
            }
        }       
        
        
       
    }
}

