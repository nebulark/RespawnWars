#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;

#endregion

namespace MMP1_Prototype
{
    /// <summary>
    /// Kern des Spieles
    /// Erstellt alle benötigten elemente und lädt texturen
    /// Über die Static elemente lassen sich bestimmte (globale) Spieleigenschaften leicht ändern oder überprüfen
    /// </summary>  
    public class IngameState : I_State
    {
        public static readonly Vector2[] ScorePos = { new Vector2(100,0),  new Vector2(400,0)};
        public static readonly Rectangle FieldSize = new Rectangle(0,0,800,600);
        private static readonly Point[] controlPointPosition = { new Point(75, 100), new Point(625, 100), new Point(350, 450) };
        private static readonly string[] unitimages = { "circle", "quad", "triangle" };
        private static readonly string[] projectileImages = { "rocket", "explosion", "sniper", "burst" };       
        
        
        public static byte Takt = 0;
        public static Point offset = new Point(0, 0); 
        private I_Player[] players;        
        private byte[] sendingBuffer;
        private SendingManager sendingManager;       
        private ControlPointManager controlPointManager;       
        private SpriteFont font1;
        private Texture2D background;     
        private int timeout = 600; //10sec       
        private Game1 game;      

        public IngameState(Game1 game1)            
        {
            font1 = game1.font1;
            game = game1;
            background = game.Content.Load<Texture2D>("backgrounds\\Background");
            players = new I_Player[2];
            Texture2D[] projectileTextures = new Texture2D[projectileImages.Length];
            for (int i = 0; i < projectileImages.Length; i++)
            {
                projectileTextures[i] = game.Content.Load<Texture2D>("projectileimages\\" + projectileImages[i]);
            }
            Texture2D[] UnitTextures = new Texture2D[unitimages.Length];           
            for (int i = 0; i < unitimages.Length; i++)
            {
                UnitTextures[i] = game.Content.Load<Texture2D>("unitimages\\" + unitimages[i]);
            }
            sendingBuffer = new byte[2048];
            players[0] = new LocalPlayer(projectileTextures, UnitTextures, game.font1, Utilities.MyColor);
            players[1] = new NetworkPlayer(projectileTextures, UnitTextures, game.font1, Utilities.EnemyColor);
            for (byte i = 0; i < players.Length; i++)
            {
                players[i].Initialize(i);
            }
            Texture2D[] controlPointTextures = { game.Content.Load<Texture2D>("capturepoint"), game.Content.Load<Texture2D>("capturepointbackground") };
            controlPointManager = new ControlPointManager(controlPointTextures, controlPointPosition);           
            sendingManager = new SendingManager();
            timeout = 600;
            Takt = 0;  
        }
        /// <summary>
        /// Verarbeitet die Daten aus dem Bytearray und gibt den nächsten Gamestate zurück
        /// </summary>
        /// <param name="data">Bytearray von Daten</param>
        /// <returns>Nächster Gamestate oder Gamestate.NoChange</returns>
        private Gamestate processData(byte[] data)
        {
            timeout = 600;
            switch ((PacketType)data[0])
            {
                //Gegner hat gewonnen
                case PacketType.Victory:
                    return Gamestate.Defeat;
                case PacketType.Defeat:
                    return Gamestate.Victory;
                //daten des Netzwerkspielers: Positionen projectile und units,  und Punkte
                case PacketType.GameData:
                    //Verschiebe Daten aus Buffer in dem "Buffer" des Networkplayers, um dann vom diesem verarbeitet zu werden                  
                    ((NetworkPlayer)players[1]).SetStreamData(data);                                                        
                    break;

                //daten geben an wieviel Schaden die eingenen Einheiten genommen haben
                case PacketType.Damage:
                    ((LocalPlayer)players[0]).unitManager.AdjustUnitHealthToData(data);                    
                    break;
                case PacketType.Promise:
                    //Packet erhalten, Lesebestätigung
                    game.Udp.addSendingData(new byte[] { (byte)PacketType.PromiseResponse, data[1] });
                    //Daten Extrahieren und nochmal neu bearbeiten
                    byte[] extractedData = new byte[data.Length - 2];
                    Array.Copy(data, 2, extractedData, 0, extractedData.Length);                   
                    return processData(extractedData);                                       
                case PacketType.PromiseResponse:
                    //Index steht nach packettype
                    //der andere hat Nachricht erhalte --> kann rausgelöscht werden
                    sendingManager.removeItem(data[1]);
                    break;
            }
            return Gamestate.NoChange;            
        }
        public Gamestate Update(GameTime gameTime)
        {
            controlPointManager.Update();
            Gamestate nextstate = Gamestate.NoChange;
            byte[] receivedData;
            //Hole Packete falls welche da
            receivedData = game.Udp.GetNextPacket();          
            while (receivedData != null)
            {
                //Verarbeite Packet
                nextstate = processData(receivedData);
                if (nextstate != Gamestate.NoChange) { return nextstate; }
                receivedData = game.Udp.GetNextPacket();
            }
            receivedData = null;          
            //offset += new Vector2(1, 1);
            if (timeout-- < 0) { Debug.Assert(false, "Timout"); }
             if (Takt == 255) { Takt = 0; }
                else { Takt++; }
             UI.output ="   "+ timeout+"  ";    
                 for (int i = 0; i < players.Length; i++)
                 {
                     players[i].Update(gameTime, offset);                     
                 }
                 //Regelmäßiges Updaten
                 //abwechselnd damit nicht zuviel auf einmal gemacht werden muss
                 if (Takt %2 == 0) {

                     //Beschreibe Daten
                     int length = ((LocalPlayer)players[0]).WriteBytes(sendingBuffer);
                     // füge sie der SendeQueue hinzu                                       
                     game.Udp.addSendingData(sendingBuffer, length);  
                 }
                //abwechselnd damit nicht zuviel auf einmal gemacht werden muss
                 else
                 {
                     //Hitdetection gegnerische Einheiten vs Eigene Projectile
                     //gibt zurück wieviele daten beschriebn wurden
                     int length = 0;
                     length = ((NetworkPlayer)players[1]).unitManager.Hitdetection(((LocalPlayer)players[0]).projectileFactory, sendingBuffer);
                     //wenn die länge 2 oder weniger ist, dann sind keine sinvollen daten vorhanden und senden kann sich gespart werden
                     
                     //Fall bedingung nicht erfüllt ist kann das packet keine sinvollen daten enthalten da das packet "leer" 1+UnitTypeCount groß ist
                     if (length > 1 + UnitManager.UnitTypeCount)
                     {
                         //wichtige Daten, sie unbedingt ankommen sollten
                         sendingManager.addItem(sendingBuffer, length);
                     }
                 }                 
                 //1 Sieg -1 Niederlage
                 int winningStatus = ((LocalPlayer)players[0]).GetWinningStatus();
                 if (winningStatus != 0)
                 {
                     if (winningStatus == 1)
                     {
                         return Gamestate.Victory;
                     }
                     else
                     {
                         return Gamestate.Defeat;
                     }
                 }

                 //fügt packete hinzu falls welche vorhanden sind
                 game.Udp.addSendingData(sendingManager.GetData());
                 //absenden
                 game.Udp.Send();
                 return Gamestate.NoChange;
        }       
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero + offset.ToVector2(), Color.White);
            controlPointManager.Draw(spriteBatch,  offset);                           
            for (int i = 0; i < players.Length; i++)
            {
                players[i].Draw(spriteBatch, offset);
            }
            controlPointManager.AddPoints((LocalPlayer)players[0]);            
        }


    }
}
