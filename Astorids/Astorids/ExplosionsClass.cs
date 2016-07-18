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
    class ExplosionsClass
    {
        public float Timer;
        public float Interval;

        public int FrameCount;
        public int CurrentFrame;

        public int FrameWidth;
        public int FrameHeight;

        public Rectangle CurrentSprite;
        public Vector2 Size;
        public Vector2 Position;
        public ExplosionType MyType;
    }
    public enum ExplosionType
    {
        ASTEROID,
        SHIP,
    }
}
