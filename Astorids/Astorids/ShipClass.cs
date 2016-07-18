using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astorids
{
    class ShipClass
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public float Acceleration;

        public float Rotation;
        public float RotationDelta;

        public Vector2 Size;
        public Vector2 MaxLimit;
        public Vector2 MinLimit;
        public float MaxSpeed = 3;

        public float FireRate = 2f;
        public bool Saber = false;

        public float Range = 75;

        public bool Visible;
        public bool Vulnerable;
        public bool Dead;
        public Vector2 m_spawnPosition;
        public float m_respawnTime;
        public float m_invulnerableTime;
        public float m_respawnTimer;
        public float m_invulnerableTimer;
        public float Score = 0;

        public void Respawn()
        {
            Position = new Vector2(m_spawnPosition.X,
                m_spawnPosition.Y);
            Velocity = new Vector2(0, 0);
            Acceleration = 0f;
            Rotation = 0f;
            RotationDelta = 0f;
            Dead = false;
            Visible = true;
        }

        public void Die()
        {
            if (Vulnerable)
            {
                Dead = true;
                Visible = false;
                Vulnerable = false;
            }
        }
    }
}
