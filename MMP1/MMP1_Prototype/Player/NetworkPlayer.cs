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
    /// Netzwerkspieler, Daten werden über Datenpacketet gesetzt oder durch berechnung vorhergesagt
    /// </summary>
    class NetworkPlayer : I_Player
    {
        //Gibt an wie der Framerückstand aussieht und kann Packetreihenfolge identifizieren
        byte Takt;
        byte PlayerNr;              
        public UnitManager unitManager;
        public ProjectileManager projectileFactory;
        
        
        private bool newStuff;
        private byte[] playerData;            
        private float score;
        private SpriteFont font;


        public NetworkPlayer(Texture2D[] projectileTextures, Texture2D[] unitTextures, SpriteFont font, Color color)
        {
            this.font = font;
            projectileFactory = new ProjectileManager(projectileTextures,color);
            playerData = new byte[2048];            
            unitManager = new UnitManager(unitTextures,color);
            score = 0;
        }
        public void Initialize(byte nr)
        {         
            newStuff = false;
            PlayerNr = nr;           
        }
        private int setFromStream()
        {           
            //Speichert die erhaltenen Daten in das private StreamDataRead und gibt die zuvor benutze daten ins
            //öffentlich stream data write da sie breits ausgewertet wurden und überschrieben werden können              
            //Muss vor dem Funktionsaufruf überprüft werden
            Debug.Assert(playerData != null);
            //Sollte nicht eintreten können
            Debug.Assert(playerData.Length != 0);
            //Stellt sicher, dass es sich um den richtigen Packettyp handelt
            Debug.Assert((PacketType)playerData[0] == PacketType.GameData);
                           
            //Handelt sich es um den Richtigen Spieler ... wenn nicht Abbrechen                
            //if (PlayerNr != StreamDataRead[1]) { return; }
                
            //Überprüfung ob es sich um eine "altes" oder Doppeltes Packet Handelt
            //BEISPIEL DOPPELTES PACKET: (100 -101) = -1  ,   -1 % 256 = 255
            //Funktioniert natürlich nicht wenn ein Update sehr weit zurückligt ( 2 Sekunden +)
            if (((playerData[2] - (Takt + 1)) + byte.MaxValue) % byte.MaxValue > byte.MaxValue / 2) { return 0; }

            Takt = playerData[2];
            int taktRuckstand = IngameState.Takt - Takt ;
            if (taktRuckstand < 0) { taktRuckstand += byte.MaxValue; }
            score = BitConverter.ToSingle(playerData, 3);
            int offset = 7;
            offset = unitManager.SetFromBytes(playerData, offset, taktRuckstand);
            offset = projectileFactory.SetFromBytes(playerData, offset);                
            return offset;    
        }
        public void SetStreamData(byte[] data)
        {
            playerData = data;
            newStuff = true;
        }

        public void Update(GameTime gameTime, Point offset)
        {
           
            //Wir haben Daten und werden sie benutzen
            if (newStuff)
            {
                newStuff = false;               
                setFromStream();
            }
            //Wir haben keine Daten und müssen eine Vorhersage treffen
            else
            {
                unitManager.Update();
                projectileFactory.Update();
            }
            
           
        }
        public void Draw(SpriteBatch spritebatch, Point offset)
        {
            unitManager.Draw(spritebatch, offset);
            projectileFactory.Draw(spritebatch, offset);
            spritebatch.DrawString(font, String.Format("Enemy Score : {0:0}", score), IngameState.ScorePos[1], Color.White);
        }
        
    }
}
