using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public class Character
    {
        public Vector2 position, velocity;//global position.
        public Texture2D[] mainSpriteSheet;//should be three, but open. defined in main (size).
        public int pose, numPoses;
        public bool airborne;

        public Character()
        {

        }
        public virtual void Update(GameTime gametime)
        {

        }
    }
}

