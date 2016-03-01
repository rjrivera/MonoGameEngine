using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Game1
{
    class basicBlock
    {
        public int length, height;
        public Texture2D texture;
        public Rectangle destinationRect;
        public Vector2 position, velocity;

        public basicBlock()
        {
            position.X = position.Y = 0;
            length = height = 32;
        }

        public void loadTexture(Texture2D texture_){
            this.texture = texture_;
        }

        /*public virtual void Update(float backX, float backY, float gameTime)
        {

        }*/
        //TODO: replace this update method with above virtual function, and provide polymorphic classes steming from this basic block. 
        public void Update(float backX, float backY, GameTime gameTime)
        {
            //update the destination Rectangle here for all platforms. 
            this.destinationRect = new Rectangle((int)(position.X - backX), (int)(position.Y - backY), (int)(texture.Width), (int)(texture.Height));

            //update any sprites here. 
            GameTime deltaTime = gameTime;
            //float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds; // <<< consider using this in lieu of the GameTime object. 

            /* TODO: uncomment when activating sprite-animated blocks. 
            spriteTimer += deltaTime;


            if (spriteTimer > spriteInterval)//possiblity of updateing the interval itself for advanced behavior control.
            {
                spriteTimer = 0f;

                currentFrame++;
                //following if statement is for clock independent spritesheets
                //if (dead && deathFrame < 7)
                //{
                //    deathFrame++;
                //}
                if (currentFrame > numSprites - 1)
                {
                    currentFrame = 0;

                }
                sourceRect = new Rectangle(currentFrame * (int)Length, 0, (int)Length, this.Skin.Height);
            }
            */

        }

        // top collision == 1; right collision (from the right) == 2; bottom collision == 3; left collision == 4;
        // left and right collisions may behave the same [ ] TODO: verify this hypothesis. 
        public void collisionAction(int collisionDirection, Character target_) //see todo of line 285 in Game1.cs. 
        {
            //todo - add the platform's velocity to make this code work for moving platforms. platforming perfection is my NEXT goal. 
            switch(collisionDirection)
            {
                case 1:
                    target_.position.Y -= target_.velocity.Y;
                    target_.velocity.Y = 0;
                    //target_.position.Y = //this.position.Y - target_.mainSpriteSheet[target_.pose].Height + 1f;
                    target_.airborne = false;
                    break;
                case 2:
                    target_.velocity.X = 0;
                    target_.position.X = this.position.X + this.texture.Width; 
                    break;
                case 3:
                    target_.velocity.Y = 0;
                    //target_.position.Y = this.position.Y + this.texture.Height;
                    break;
                case 4:
                    target_.position.X -= target_.velocity.X;
                    target_.velocity.X = 0;
                    //target_.position.X = this.position.X - target_.mainSpriteSheet[target_.pose].Width;
                    break;
                default:
                    //do nothing
                    break;
            }

        }
    }
}
