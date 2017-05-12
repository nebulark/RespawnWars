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
    /// Zeigt die Ipadressen anderer Spieler an und versendet Packete um auch von anderen gesehen zu werden.
    /// Beim Klicken auf eine IPadresse schickt man dem jeweiligen spieler eine Challange Packet
    /// </summary>
    class NetworkLobbyState : A_Menu, I_State
    {
        static readonly string menuname = "Network Lobby";
        Game1 game;             
        byte[] sendingBuffer;        
        int cnt;
        Dictionary<string, IPAddress> nameToIp;
        //Packete werden in zufälligen Intervallen geschickt um das Netzwerk nicht unnötigt zu belasten
        Random r;
        public NetworkLobbyState(Game1 game1):base(game1)
        {            
            game = game1;                      
            sendingBuffer = new byte[1024];          
            r = new Random();                    
            cnt = r.Next(0, 20);
            nameToIp = new Dictionary<string, IPAddress>();
            base.Initialize(menuname,null);         
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
            cnt--;
            if (cnt <= 0)
            {
                int packetLength = GetLobbyBeacon(sendingBuffer);
                game.Udp.addSendingData(sendingBuffer, packetLength);
                game.Udp.Send();
                cnt = r.Next(5, 20);
                
            }
            //Holen des gedrückten Buttons, if null --> nichts gedrückt
            string input = base.GetInput(gameTime);
            if (input != null)
            {                
                game.Udp.SetConnection(nameToIp[input]);
                return Gamestate.WaitForMatch;
            }
            return Gamestate.NoChange;

        }
        /// <summary>
        /// Baut einen BeaconPacket, das Ipaddresse und gegebenfalls den Namen des spielers enthält und schreibt es in das angegebene array
        /// </summary>
        /// <param name="result">Das zu beschreibende Byteadrray</param>
        /// <returns>Länge des Packetes</returns>
        public int GetLobbyBeacon(byte[] result)
        {            
            result[0] = (byte)PacketType.LobbyBeacon;            
            Array.Copy(Utilities.LocalAdressInBytes, 0, result, 1, 4);
            int offset = 5;
            int encodedNameLengthIndex = offset;
            offset++;
            if (Utilities.PlayerName != null && Utilities.PlayerName != "" && Utilities.PlayerName.Length < byte.MaxValue / 2 )
            {
                byte [] name = System.Text.Encoding.Unicode.GetBytes(Utilities.PlayerName);
                Array.Copy(name,0,result,offset,name.Length);
                offset += name.Length;
                //durch if bedingung abgedeckt
                result[encodedNameLengthIndex] = (byte)name.Length;
            }
            else 
            {
                result[encodedNameLengthIndex] = 0;
            }
            return offset;            
        }
        public Gamestate ProcessReceivedData(byte[] data)
        {
            //Packetheader
            int offset = 1;
            switch ((PacketType)data[0])
            {
                case PacketType.LobbyBeacon:                   
                    byte[] ipArray = new byte[4];
                    Array.Copy(data, offset, ipArray, 0, 4);
                    offset += 4;                    
                    IPAddress Ip = new IPAddress(ipArray);
                    byte nameLength = data[offset];
                    offset++;
                    //falls name nicht vorhanden ist der name die IP adresse
                    string name = (nameLength == 0)
                        ? Ip.ToString()
                        : System.Text.Encoding.Unicode.GetString(data, offset, nameLength);
                    offset += nameLength;  
                    //Falls die IP adresse weder die eigene ist oder bereits gespeichert wurde...
                    if (Ip.ToString() != (Utilities.LocalAdress).ToString()
                        && !nameToIp.ContainsKey(name))
                    {
                        //...speichere in Dictionary
                        nameToIp.Add(name, Ip);
                        //und füge der ButtonListe hinzu
                        base.addButton(name);                                               
                    }
                    return Gamestate.NoChange;


                case PacketType.ChallengeRequest:                    
                    byte[] challangerIP = new byte[4];
                    Array.Copy(data, 1, challangerIP, 0, 4);                    
                    game.Udp.SetConnection(new IPAddress(challangerIP));                    
                    return Gamestate.MatchRequest;
                default:
                    return Gamestate.NoChange;                   
            }
            
            
        }
    }
}
