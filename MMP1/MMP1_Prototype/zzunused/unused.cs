using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{
    class unused
    {
        //Für Syncronisierung nach dem start des spieles

        /*else
             {
                 byte[] syncPacket = Utilities.GetSyncPacket(Takt);
                 game.Udp.Send(syncPacket);
                 UI.output +=" \n lol " + Taktdiff;
                 
                 //Verbindung.sender.SendTo(syncPacket, Verbindung.sending_end_point);
                 //Kann das Spiel Starten?
                 if (Math.Abs(Taktdiff) < 5) { ready = true; }
                 //Kann nicht sein. VERMUTUNG: Startpaket verlorengegangen, weshalb nie ein SyncPacketgesendet wurde, oder hohe Latenz
                 //StartPacket nochmal Schicken
                 else if (Math.Abs(Taktdiff) > 130)
                 {
                     byte[] sendStart = Utilities.GetTypeAndIPPacket(PacketType.ChallengeStart);
                     //Verbindung.sender.SendTo(sendStart, Verbindung.sending_end_point); 
                 }
                //Takt ausgleichen
                //Taktdiff gibt an um wie viel ich kompensieren müsste
                //ACHTUNG: Der Andere kompensiert auch!
                 else
                 {
                     if (diffChanged)
                     {
                         diffChanged = false;
                         //Langsam annähern
                         int neuerTakt = Takt + Taktdiff / 3;
                         //Takt ist Zyklisch!
                         if (neuerTakt < 0) { neuerTakt += byte.MaxValue + 1; }
                         if (neuerTakt > byte.MaxValue) { neuerTakt -= byte.MaxValue + 1; }
                         Takt = (byte)neuerTakt;
                     }
                 }
             }*/
			 
			 //gehört in packet received
			 /*
			  case PacketType.GameSync:
                    Taktdiff =  localMsg[1] - Takt ;
                    #region Erklärung
                    // ||... Betragstriche
                    //Es können Werte von -255 bis + 255 rauskommen
                    //Es sollen aber Werte von -128 bis + 128 rauskommen
                    //if(|Taktdiff| > 128) -> Übertrag -> muss nachbearbeitet werden
                    //am ende darf die if bedingung nicht mehr erfüllt werden
                    //WENN  128 < Taktdiff < 255  DANN |Taktdiff - 255| < 128
                    #endregion

                    if (Math.Abs(Taktdiff) > byte.MaxValue / 2)
                    {
                        if (Taktdiff > 0) { Taktdiff -= byte.MaxValue+1; }
                        else { Taktdiff += byte.MaxValue+1; }
                    }
                    
                    diffChanged = true;
                    break;
			 */
            
    }
}
