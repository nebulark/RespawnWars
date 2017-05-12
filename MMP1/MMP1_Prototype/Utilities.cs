using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input; 
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
namespace MMP1_Prototype
{    
    /// <summary>
    /// Statische Klasse, enthält globale Variabeln, sowie einige Globale Funktionen
    /// </summary>
    static class Utilities
    {

       
        #region Globale Variablen
        public static string PlayerName = null;
        public static Color MyColor = Color.Green;
        public static Color EnemyColor = Color.Red;
        public const int DisplaySize = 16;
        static public KeyboardState LastKeyboardState, CurrentKeyboardState;
        static public MouseState LastMouseState, CurrentMouseState;        
        #endregion 
        #region Properities
        public static IPAddress LocalAdress
        {
            get { return localAdress; }
            set
            {
                localAdress = value;
                LocalAdressInBytes = value.GetAddressBytes();
            }
        }
        public static byte[] LocalAdressInBytes;
        private static IPAddress localAdress;
        
        public static bool Leftclicked
        {
            get { return LastMouseState.LeftButton != ButtonState.Pressed && CurrentMouseState.LeftButton == ButtonState.Pressed; }
        }
        public static bool Rightclicked
        {
            get { return LastMouseState.RightButton != ButtonState.Pressed && CurrentMouseState.RightButton == ButtonState.Pressed; }
        }
        public static bool LeftHold
        {
            get { return LastMouseState.LeftButton == ButtonState.Pressed && CurrentMouseState.LeftButton == ButtonState.Pressed; }
        }
        public static bool LeftReleased
        {
            get{ return LastMouseState.LeftButton == ButtonState.Pressed && CurrentMouseState.LeftButton != ButtonState.Pressed; }
        }
        public static bool ControlPressed
        {
            get { return CurrentKeyboardState.IsKeyDown(Keys.LeftControl) || CurrentKeyboardState.IsKeyDown(Keys.RightControl); }
        }
        #endregion
        //Für Utility Funktionen benötigte Variabeln, die sich nie verändern        
        static Texture2D pixel;
        static Vector2 line;
       
        //Zeichnet eine Linie Zwischen zwei Punkten in der angegebenen Farbe.
        static public void DrawLine(SpriteBatch spriteBatch, Point begin, Point end, Color color)
        {
            if (pixel == null)
            {
                pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                pixel.SetData<Color>(new Color[] { Color.White });
            }
            line = (end - begin).ToVector2();
            double lineLength = line.Length();
            line = Vector2.Normalize(line);
            for (int i = 0; i <= (int)lineLength + 0.5; i++)
            {
                spriteBatch.Draw(pixel, begin.ToVector2() + line * i, color);
            }
        }
       
        //Zeichnet ein Hohles Rechteck
        static public void DrawRectangle(SpriteBatch spriteBatch, Point begin, Point end, Color color)
        {
            DrawLine(spriteBatch, begin, new Point(begin.X, end.Y), color);
            DrawLine(spriteBatch, begin, new Point(end.X, begin.Y), color);
            DrawLine(spriteBatch, new Point(begin.X, end.Y), end, color);
            DrawLine(spriteBatch, new Point(end.X, begin.Y), end, color);
        }        
        
        static public Rectangle GetRectangleFromOposingCorners(Point corner, Point opposingCorner)
        {
            return new Rectangle(Math.Min(corner.X, opposingCorner.X),
                                    Math.Min(corner.Y, opposingCorner.Y),
                                    Math.Abs(corner.X - opposingCorner.X),
                                    Math.Abs(corner.Y - opposingCorner.Y));
        }
        
        //Gibt die Zahl der Taste zurück die gerade losgelassen wurde, bei mehreren wird die niedrigere Zurückgegeben
        //0 Entspricht 10!
        //Falls keine Zahlentaste (!)  gedrückt wurde wird -1 zurückgeben
        //Zahlentasten des NumPads werden nicht berücksichtigt
        static public int GetNumberPressed()
        {
            if (!LastKeyboardState.IsKeyDown(Keys.D1) && CurrentKeyboardState.IsKeyDown(Keys.D1)) { return 1; }
            if (!LastKeyboardState.IsKeyDown(Keys.D2) && CurrentKeyboardState.IsKeyDown(Keys.D2)) { return 2; }
            if (!LastKeyboardState.IsKeyDown(Keys.D3) && CurrentKeyboardState.IsKeyDown(Keys.D3)) { return 3; }
            if (!LastKeyboardState.IsKeyDown(Keys.D4) && CurrentKeyboardState.IsKeyDown(Keys.D4)) { return 4; }
            if (!LastKeyboardState.IsKeyDown(Keys.D5) && CurrentKeyboardState.IsKeyDown(Keys.D5)) { return 5; }
            if (!LastKeyboardState.IsKeyDown(Keys.D6) && CurrentKeyboardState.IsKeyDown(Keys.D6)) { return 6; }
            if (!LastKeyboardState.IsKeyDown(Keys.D7) && CurrentKeyboardState.IsKeyDown(Keys.D7)) { return 7; }
            if (!LastKeyboardState.IsKeyDown(Keys.D8) && CurrentKeyboardState.IsKeyDown(Keys.D8)) { return 8; }
            if (!LastKeyboardState.IsKeyDown(Keys.D9) && CurrentKeyboardState.IsKeyDown(Keys.D9)) { return 9; }
            if (!LastKeyboardState.IsKeyDown(Keys.D0) && CurrentKeyboardState.IsKeyDown(Keys.D0)) { return 10; }
            return -1;
        }
        static public bool isKeyPressed(Keys key){

            return (!LastKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyDown(key));  
        }
        //überprüft tastaturinput und schreib sie in den string, gibt über Flags zurück welche Tasten gedrückt wurden (im moment nur Enter)
        static public byte WriteInput(ref string changingText)
        {
            Keys[] lk = LastKeyboardState.GetPressedKeys();
            Keys[] ck = CurrentKeyboardState.GetPressedKeys();
            Keys[] k = ck.Except(lk).ToArray();
            byte code = 0;
            string tmpstring = "";
            bool shiftpressed = CurrentKeyboardState.IsKeyDown(Keys.RightShift) || CurrentKeyboardState.IsKeyDown(Keys.LeftShift);
            bool backpressed = false;
            for (int i = 0; i < k.Length; i++)
            {
                switch(k[i])
                {
                    case Keys.Back: backpressed = true;
                        break;
                    case Keys.OemPeriod: tmpstring += ".";
                        break;
                    case Keys.Space: tmpstring += " ";
                        break;
                    //Enter Flag Setzen
                    case Keys.Enter: code |= 0x01;
                        break;
                    case Keys.LeftShift:                       
                    case Keys.RightShift: shiftpressed = true;
                        break;                    
                    default:
                        //Buchstabe zum string hinzufügen fall er sich im erlaubten Ascii bereich befindet
                        if((int)k[i] > 32 && (int)k[i] < 128)
                         tmpstring += (char)k[i];                       
                        break;
                }                  
            }
            if (!shiftpressed) { changingText += tmpstring.ToLower(); }
            else { changingText += tmpstring.ToUpper(); }
           
            if (backpressed && changingText != "")
            {
                //Lösche Letztes Zeichen
                changingText = changingText.Substring(0, changingText.Length-1);
            }
            return code;
        }        
        
        static public void UpdateInput()
        {
            LastKeyboardState = CurrentKeyboardState;
            LastMouseState = CurrentMouseState;
            CurrentKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();                      
        }
        static public string[] GetOwnIPAdresses()
        {
            //Sucht den Namen des Eigenen Computers und sucht danach alle IPadressen die diesen namen besitzen
            //ACHTUNG kann je nach Netztwerk fehlschlagen, derzeit jedoch die einzige Lösung die eigene IPAdresse(n) zu finden
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            //Kopiere alle zu einem string kovertierten IPAdressen, jedoch nur IPv4 !
            List<string> result = new List<string>();

            for (int i = 0; i < host.AddressList.Length; i++)
            {
                if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork) { 
                result.Add( host.AddressList[i].ToString());}
            }
            return result.ToArray();
        }
        static public int GetTypeAndIPPacket(byte[] result, PacketType type){
            Debug.Assert(result != null, "result darf nicht null sein da es der Ausgabewert ist");
            Debug.Assert(result.Length >= 5, "Die Funktion GetTypeAndIPPacket benötigt ein Array der Länge 5oder mehr");
            result[0] = (byte)type;
            Array.Copy(LocalAdressInBytes, 0, result, 1, 4);
            return 5;
        }       
        static public double CalculateAngleFromVector(Vector2 vector)
        {
            //sonderfälle
            if (vector.X == 0)
            {
                //Unmöglich Winkel zu berechnen
                Debug.Assert(vector.Y != 0, "Vector (0|0) ist nicht erlaubt");

                //Entweder 90 oder 270 grade je nach vorzeichen beim y
                return (vector.Y > 0) ? (Math.PI / 2) : (Math.PI * 3 / 2); 
            }
            double result = Math.Atan((vector.Y) / (vector.X));
            //Korrektur für jeweilige Quadranten
            if (vector.X < 0) { result += Math.PI; }            
            return result;                
        }
        static public double Pytagoras(double A, double B)
        {
            return Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2));
        }
      
        static public Color GetRGBformHSV(int hue, float saturation, float value)
        {
            float chroma = value * saturation;            
            hue = hue % 360;
            int dominantColor = hue / 60;
            float second = chroma * (1-Math.Abs((dominantColor%2)-1));
            float r=0, g=0, b=0;
            switch (dominantColor)
            {
                case 0:
                    r = chroma;
                    g = second;
                    b = 0;
                    break;
                 case 1:
                    r = second;
                    g = chroma;
                    b = 0;
                    break;
                 case 2:
                    r = 0;
                    g = chroma;
                    b = second;
                    break;
                 case 3:
                    r = 0;
                    g = second;
                    b = chroma;
                    break;
                 case 4:
                    r = second;
                    g = 0;
                    b = chroma;
                    break;
                 case 5:
                    r = chroma;
                    g = 0;
                    b = second;
                    break;
            }
            float rest = value -chroma;
            r += rest;
            g += rest;
            b += rest;
            return new Color(r,g,b);
        }
        
        



    }

}