using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace MMP1_Prototype
{
    /// <summary>
    /// Gibt Informationen zum Inhalt eines Bytearrays. Steht immer an erster Stelle des zu sendenden Bytearrays.
    /// </summary>
    enum PacketType : byte
    {
        /// <summary>
        /// Kennzeichnet Packet, welches Daten zum Zustand des Spieles (Postions und Bewegeungsdaten von Spieler/Projectil und Score) beinhaltet.
        /// </summary>
        GameData,
        /// <summary>
        /// Kennzeichnet Packet, welches IP adresse und Name eines Spielers enthält, welcher sich in der Netzwerklobby befindet.
        /// </summary>
        LobbyBeacon,
        /// <summary>
        /// Kennzeichnet Packet, welches IP Adresse eines Herausforderes enthält.
        /// </summary>
        ChallengeRequest,
        /// <summary>
        /// Gibt an dass der Spieler einen Herausforderung animmt. IP adresse des absenders ist enthalten.
        /// </summary>
        ChallengeAccepted,
        /// <summary>
        /// Gibt an dass der Spieler einen Herausforderung ablehtn. IP adresse des absenders ist enthalten.
        /// </summary>
        ChalleneDenied,
        /// <summary>
        /// Initiert den Start eines Spieles
        /// </summary>
        ChallengeStart,
        /// <summary>
        /// Gibt an, dass der Spieler die Herausforderun zum Spiel abgebrochen hat
        /// </summary>
        ChallengeAbort,
        /// <summary>
        /// Derzeit Unbenutzt.
        /// </summary>
        GameSync,
        /// <summary>
        /// Kennzeichnet Packet, welches Daten enhält die sicher ankommen müssen. Enthält Kennummer und ein weiteres beliebiges Packet.
        /// </summary>
        Promise,
        /// <summary>
        /// Bestätigt Ankunft eines sicheren Packets und enthält dessen Kennummer.
        /// </summary>
        PromiseResponse,        
        /// <summary>
        /// Enthält Daten die angeben welche Einheit wieviel Schaden erlitten hat. Muss sicher gesendet werden.
        /// </summary>
        Damage,
        /// <summary>
        /// Gibt an, dass der Absender gewonnen hat.
        /// </summary>
        Victory,
        /// <summary>
        /// Gibt an, dass der Absender verloren hat.
        /// </summary>
        Defeat
    }
    /// <summary>
    /// Enthält informationen zur Verbindung
    /// Sendet und erhält Packete
    /// Senden und Empfangen läuft seperat vom Hauptthread ab.
    /// </summary>
    public class UdpVerbindung : IDisposable
    {
        #region Klassen Variabeln
        /// <summary>
        /// Sendet Daten.
        /// </summary>
        Socket sender;
        /// <summary>
        /// Empfängt Daten.
        /// </summary>
        Socket listener;
        /// <summary>
        /// Wird beim empfang eines Packetes aufgerufen.
        /// </summary>
        AsyncCallback ProcessData;
        /// <summary>
        /// Wird nach dem Senden eines Pacektes aufgerufen.
        /// </summary>
        AsyncCallback endSend;
        /// <summary>
        /// Eigene IP adresse und listen Port.
        /// </summary>
        public EndPoint Local;
        /// <summary>
        /// IP adresse und empfangs Port des Netzwerkspielers.
        /// </summary>
        public EndPoint Remote;
        /// <summary>
        /// Buffer zum Empfangen von Daten. Muss größer sein als größte mögliche empfangbare Bytearrays
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// Gibt an ob das Object bereits "gelöscht" ist um mehrfaches schließen von Sockets zu verhindern.
        /// </summary>
        private bool isDead;
        
        /// <summary>
        /// Erhaltene Daten werden von eingereiht und dann von einem ANDEREN (!) Thread ausgereiht. Verschiedene Threads könnten gleichzeitig darauf zugreifen. 
        /// </summary>
        volatile Queue<byte[]> sendingQueue;
        /// <summary>
        /// Zu sendene Daten werden eingereiht und von einem ANDEREN (!) Thread ausgereiht. Verschiedene Threads könnten gleichzeitig darauf zugreifen.
        /// </summary>
        volatile Queue<byte[]> receivedQueue;        

        /// <summary>
        /// True fall gerade gesendet wird. Benutzt um nicht mehrfach zu senden.
        /// </summary>
        bool sending;
        /// <summary>
        /// True fall gerade empfangen wird. Benutzt um nicht mehrfach zu emfangen.
        /// </summary>
        bool receiving;
        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="IPv4">IP adresse des Netzwerkspielers</param>
        /// <param name="port">Sende und Listen Port</param>
        public UdpVerbindung(IPAddress IPv4, int port = 4004)
        {
            ProcessData = new AsyncCallback(processData);
            endSend = new AsyncCallback(EndSend);
            receivedQueue = new Queue<byte[]>(16);
            receiving = false;
            sendingQueue = new Queue<byte[]>(16);
            sending = false;            
            isDead = false;
            sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sender.EnableBroadcast = true;
            listener.EnableBroadcast = true;
            Local = new IPEndPoint(IPv4, port);
            Remote = new IPEndPoint(IPAddress.Broadcast, port);
            
            listener.Bind(new IPEndPoint(IPAddress.Any, port));                     
            buffer = new byte[2048];          
        }
        public UdpVerbindung(string IPv4, int port = 4004)
            : this(IPAddress.Parse(IPv4), port)
        {
        }
        public UdpVerbindung(int port = 4004)
            : this(IPAddress.Broadcast, port)
        {
        }
        /// <summary>
        /// Ändert die Verbindung auf die angebene IP Adresse und Port
        /// </summary>
        /// <param name="IPv4">IP adresse des Netzwerkspielers</param>
        /// <param name="port">Port des Netzwerkspielers</param>
        public void SetConnection(IPAddress IPv4, int port = 4004)
        {
            Remote = new IPEndPoint(IPv4, port);
            
        }
        /// <summary>
        /// Ändert die Verbindung auf die angebene IP Adresse und Port
        /// </summary>
        /// <param name="IPv4">IP adresse des Netzwerkspielers</param>
        /// <param name="port">Port des Netzwerkspielers</param>
        public void SetConnection(string IPv4, int port = 4004)
        {
            Remote = new IPEndPoint(IPAddress.Parse(IPv4), port);
        }
       /// <summary>
       /// Wird vom Garbage Collector aufgerufen. Schließt Sockets falls nicht bereits geschlossen
       /// </summary>
        ~UdpVerbindung()
        {
            if (!isDead)
            {
                listener.Close();
                sender.Close();
                isDead = true;              
            }
        }
        /// <summary>
        /// Bereitet Object zum Löschen vor.
        /// </summary>
        public void Dispose()
        {
            if (!isDead)
            {
                listener.Close();
                sender.Close();
                isDead = true;

            }
        }
        /// <summary>
        /// Fügt zu sendende Daten der Sendeschlange hinzu. Startet NICHT das versenden.
        /// Die größe kann zugeschnitten werden.
        /// </summary>
        /// <param name="data">Die zu sendenden Daten. Darf nicht NULL sein</param>
        /// <param name="length">Länge der daten falls etwas abgeschnitten werden soll</param>
        public void addSendingData(byte[] data, int length = 0)
        {            
            Debug.Assert(data != null);
            if (length == 0) { length = data.Length; }
            Debug.Assert(length > 0);            
            byte[] packet = new byte[length];
            Array.Copy(data, packet, length);
           
            sendingQueue.Enqueue(packet);
        }
        /// <summary>
        /// Fügt mehrer Daten der Sendeschlange hinzu.
        /// </summary>
        /// <param name="multipleData">Einzureihende Daten, wenn null wird nichts eingefügt. Beiinhaltete Arrays drüfen nicht NULL sein</param>
        public void addSendingData(byte[][] multipleData)
        {
            if(multipleData == null){return;}
            for (int i = 0; i < multipleData.Length; i++)
            {
                addSendingData(multipleData[i]);
            }
        }      

        /// <summary>
        /// Beginnt senden der Daten in der Sendingqueue. Daten werden solange in einem speraten Thread gesendet bis die Queue leer ist.
        /// </summary>
        public void Send()
        {
            //!sending verhindert mehrfaches ausführen von beginsend
            if (sendingQueue.Count > 0 && !sending)           {  
          
                sending = true;

                byte[] data = sendingQueue.Dequeue();

                sender.BeginSendTo(data, 0, data.Length, SocketFlags.None, Remote, EndSend, sender);
            }
        }
        /// <summary>
        /// Wird nach dem erfolgen Senden aufgerufen und sendet erneut falls noch Packete in der Sendingqueue sind.
        /// </summary>
        /// <param name="iar">Das sende Socket</param>
        private void EndSend(IAsyncResult iar)
        {            
            if (isDead) { return; }
            //geht nicht anders, da sogar normales beenden eine Exeption wirft.
            try
            {
                sender.EndSendTo(iar);
            }
            catch (ObjectDisposedException) { return; }
            catch (SocketException) { } 
            if (sendingQueue.Count > 0)
            {
                byte[] sendingData = sendingQueue.Dequeue();
                sender.BeginSendTo(sendingData, 0, sendingData.Length, SocketFlags.None, Remote, EndSend, sender);
               
            }
            else { sending = false; }

        }
        /// <summary>
        /// Startet das Empfangen von Daten in einem seperaten Thread. Wird beendet durch EndReceive.    
        /// </summary>
        public void BeginReceive()
        {
            Debug.Assert(!receiving);
            receiving = true;
            listener.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, processData, listener);
        }
        /// <summary>
        /// Beendet Senden von Daten
        /// </summary>
        public void EndReceive()
        {
            listener.EndReceive(null);
            receiving = false;
        }
        /// <summary>
        /// Wird aufgerufen wenn Daten erhalten werden und fügt die Daten in der EmfangsQueue ein. Beginnt danach wieder auf Daten zu warten.
        /// </summary>
        /// <param name="ar">Das Emfangssocket</param>
        void processData(IAsyncResult ar)
        {
            //Stoppt falls das Empfangen beendet wurde
            if (!receiving) { return; }                     
            //Beende empfangen
            Socket recvSock = (Socket)ar.AsyncState;
            int msgLen;
            //geht nicht anders, da sogar normales beenden eine Exeption wirft.
            try{
                msgLen = recvSock.EndReceive(ar);
            }
            catch (ObjectDisposedException) { return; }
            byte[] newData = new byte[msgLen];
            //Kopiere Daten in neu erstelltes Array und lege es in der Queue ab
            Array.Copy(buffer, newData, msgLen);
            receivedQueue.Enqueue(newData);
            //Horche weiter
            listener.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,  ProcessData, listener);
        }
        /// <summary>
        /// Gibt das älteste Packet der Sendingqueue zurück.
        /// </summary>
        /// <returns>Packet oder null bei leerer SendingQueue</returns>
        public byte[] GetNextPacket()
        {            
            if (receivedQueue.Count <= 0) { return null; }           
            return receivedQueue.Dequeue();
        }
    }
}
