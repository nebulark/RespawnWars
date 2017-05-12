using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMP1_Prototype
{   
    
   
    /// <summary>
    /// Speichert Daten und verpackt sie mit einem Promise header und ID
    /// Zuständig um Packet solange wiederzuversenden bis sie bestätigt wurden
    /// </summary>
    class SendingManager
    {
        List<byte[]> storage;
        private int itemCount;
        public SendingManager()
        {
            storage = new List<byte[]>(128);
            itemCount = 0;
        }
        /// <summary>
        /// Fügt ein neues element hinzu
        /// </summary>
        /// <param name="data">Zu verpackende Daten</param>
        /// <param name="length">Wenn kleiner data.Length wird das Packet abgeschnitten</param>
        /// <returns>Index der Packetes</returns>
        public int addItem(byte[] data, int length = 0){
            Debug.Assert(itemCount < 128);
            Debug.Assert(data != null);
            Debug.Assert(data.Length != 0);
            
            //Erstellt ein neues Array für extra plätze für die promiseddatapackettype und die id
            byte[] newItem = new byte[(length == 0) ? data.Length + 2 : length + 2];
            Array.Copy(data, 0, newItem, 2, newItem.Length-2);
            itemCount++;
            newItem[0] = (byte)PacketType.Promise;             
           
            for (byte i = 0; i < storage.Count; i++)
			{
			    if(storage[i] == null){
                    //Index ist Id
                    newItem[1] = i;
                    storage[i] = newItem;
                    
                    return i;
                }
			}
            //Falls alles voll ist            
            //Indext ist die Letzte stelle
            newItem[1] = (byte)storage.Count;
            storage.Add(newItem);
            
            return newItem[1];

        }        
        /// <summary>
        /// Entfernt element mit bestimmer id
        /// </summary>
        /// <param name="id">Id des zu entfernenden Packetes</param>
        /// <returns>Erfolg</returns>
        public bool removeItem(int id)
        {
            if (storage[id] != null)
            {
                itemCount--;
                storage[id] = null;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Gibt alle Packete im SendingManager aus
        /// </summary>
        /// <returns>Array an Packeten</returns>
        public byte[][] GetData()
        {
            byte[][] result = new byte[itemCount][];
            int cnt = 0;
            for (int i = 0; i < storage.Count; i++)
            {   
                
                if (storage[i] != null)
                {
                    result[cnt] = storage[i];   
                    cnt++;                   
                }               
            }

            return result;
        }
    }
}
