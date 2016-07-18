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
    class AsteroidClass
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        public float RotationDelta;

        public Vector2 Size;

        public Vector2 MaxLimit;
        public Vector2 MinLimit;
    }
}
