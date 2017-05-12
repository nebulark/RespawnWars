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
    /// Enthält alle Wichtigen Grundfunktionen für Units
    /// </summary>
    abstract public class A_Unit
    {
        public bool Selected;
        public ProjectileType WeaponType;
        public byte Health;
        //Zeit vom platzieren bis aktivität
        //solange spawning nicht 0 ist kann die einheit nichts tun
        public const byte spawnTime = 100;
        byte spawning;
        //2 alive, 1 dieing, 0 dead
        private byte alive;
                
       
        private byte movementSpeed;        
                   
        
        
        protected int reattackTime;
        public bool Alive
        {
            set { alive = (byte)((value) ? 2 : 0); }
        }
             
        public Rectangle Position;
        BasicHitBox hitbox;
       
        //Richtung Normalisiert
        public Vector2 MovementDirection;       
        Point target;
        Color color;
        public Point RightDown
        { get { return new Point(Position.Bottom, Position.Right); } }
        public A_Unit() { }
        public void Initialize(Vector2 size, ProjectileType type, Color color)
        {
            Position = new Rectangle(0, 0, (int)(size.X + 0.5f), (int)(size.Y + 0.5));
            alive = 2;
            reattackTime = 0;
            WeaponType = type;
            this.color = color;                       
        }
        public void SetWithStandartValues(Point posCenter)
        {          
            Set(true,GetBaseHealth(),  GetBaseSpeed(), GetBaseProjectile(), posCenter, posCenter);
        }
        public void Set(bool alive, byte health, byte movementSpeed, ProjectileType weapontype,
            Point posCenter, Point target, byte spawning = spawnTime)
        {
            this.alive = (byte)(alive ? 2 : 0);                       
            Health = health;
            this.movementSpeed = movementSpeed;
            Position.Offset(posCenter - Position.Center);
            this.target = target;
            reattackTime = GetMaxCooldown();            
            WeaponType = weapontype;
            this.spawning = spawning;            

        }
        public void Set(bool alive, byte health, byte movementSpeed, ProjectileType weapontype,
            int posx, int posy, int targetx, int targety, byte spawning = spawnTime, int taktRuckstand = 0)
        {
            Point target = new Point(targetx, targety);
            Point position = new Point(posx, posy);
            if (taktRuckstand != 0)
            {
                double distance = (target - position).ToVector2().Length();
                if (movementSpeed * taktRuckstand >= distance)
                {
                    position = target;
                }
                else
                {
                    Vector2 direction = Vector2.Normalize((position - target).ToVector2());
                    position = (direction * movementSpeed * taktRuckstand).ToPoint();
                }
            }
            Set(alive, health, movementSpeed,weapontype, position, target, spawning);         
        }
       
        public virtual bool isAlive()
        {
            return alive != 0;
        }
        public virtual bool isDieing()
        {
            return alive == 1;
        }
        public bool isReady()
        {
            return spawning == 0;
        }
        public virtual int SetFromBytes(byte[] data, int offset, int taktRuckstand = 0)
        {
            Set(
            Convert.ToBoolean(data[0 + offset]),
            data[1 + offset],
            data[2 + offset],
            (ProjectileType)data[3 + offset],
            BitConverter.ToInt32(data, 4 + offset),
            BitConverter.ToInt32(data, 8 + offset),
            BitConverter.ToInt32(data, 12 + offset),
            BitConverter.ToInt32(data, 16 + offset),
            data[20 + offset]
            );
            hitbox = getHitboxfromPosition();
            return offset + 21;
        }
        public virtual int WriteBytes(byte[] result, int offset)
        {            
            result[0 + offset] = Convert.ToByte(alive == 2);                      
            result[1 + offset] = Health;
            result[2 + offset] = movementSpeed;
            result[3 + offset] = (byte)WeaponType;
            //result [4 bis 8 (4+4)] = Pos.X
            Array.Copy(BitConverter.GetBytes(Position.Center.X), 0, result, 4 + offset, 4);
            //analog auch die anderen Werte
            Array.Copy(BitConverter.GetBytes(Position.Center.Y), 0, result, 8 + offset, 4);
            Array.Copy(BitConverter.GetBytes(target.X), 0, result, 12 + offset, 4);
            Array.Copy(BitConverter.GetBytes(target.Y), 0, result, 16 + offset, 4);
            result[20 + offset] = spawning;
            return offset + 21;           
        }
        public virtual void Update()
        {
            if (alive == 1)
            {
                alive = 0;
                return;
            }
            if (Health <= 0)
            {
                alive = 1;               
                return;
            }
            if (reattackTime > 0) { reattackTime--; }
            if (!isReady()) { spawning--; }
            else
            {
                if (Position.Center != target)
                {
                    Vector2 distance = (target - Position.Center).ToVector2();
                    if (distance.LengthSquared() < Math.Pow(movementSpeed, 2))
                    {
                        Position.Offset(target - Position.Center);
                    }
                    else
                    {
                        Position.Offset(Vector2.Normalize((target - Position.Center).ToVector2()) * movementSpeed);
                    }
                }
            }
            hitbox = getHitboxfromPosition();
           
        }
        public virtual BasicHitBox getHitboxfromPosition()
        {
            return new BasicHitBox(Position);
        }

       
        public override string ToString()
        {
            return "Alive : " + alive + "\nHealth: " + Health + "\nMovementSpeed: " + movementSpeed + "\nPosition: " + Position + "\nTarget: " + target + "\n";
        }
       
        public virtual void Shoot(Point targetCenter, ProjectileManager factory)
        {
            if (reattackTime == 0 && isReady())
            {
                factory.CreateProjectile(Position.Center, targetCenter, WeaponType);
                reattackTime = GetMaxCooldown();
            }           
        }
        public virtual BasicHitBox GetHitdetectionData()
        {
            if (hitbox == null) { getHitboxfromPosition(); }
            return hitbox;
            
        }
        public virtual int Hitdetection(ProjectileManager projectileManager)
        {
            BasicHitBox hitbox = GetHitdetectionData();
            return projectileManager.Hitdetection(hitbox);
        }
        public void changeTarget(Point Target)
        {
            target = Target;
            MovementDirection = (Target - Position.Center).ToVector2();
            MovementDirection.Normalize();            
        }
        public virtual void Draw(SpriteBatch spriteBatch, Texture2D texture, Point offset)
        {
           
            //Utilities.DrawLine(spriteBatch, Position.Center, target, Color.Black);
            //Falls die Eiheit gerade spawnt wird sie transparenter dargestellt
            spriteBatch.Draw(texture, new Rectangle(Position.X + offset.X, Position.Y + offset.Y, Position.Width, Position.Height), color * (1 - spawning / (float)spawnTime));
        }        
        public abstract void Draw(SpriteBatch spriteBatch, Point offset);
        public void Display(SpriteBatch spriteBatch, Texture2D texture, Rectangle pos)
        {
            spriteBatch.Draw(texture, pos, color);
        }
        public abstract void Display(SpriteBatch spriteBatch, Rectangle pos);
        public abstract void Initialize(Color color, Texture2D texture);
        public abstract byte GetBaseSpeed();
        public abstract byte GetBaseHealth();
        public abstract ProjectileType GetBaseProjectile();
        public abstract int GetMaxCooldown();
        
       
    }
}
