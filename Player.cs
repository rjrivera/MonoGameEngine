using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    class Player : Character
    {
        
        public float rotation, fireTimer, spriteTimer, spriteInterval, energy, energyCapacity, health, healthCapacity, maxSpeed, progressionTimer,
             hitTimer, hitInterval;//motion done in radians if i remember. multiple variables used for special effects. 
        public int currentFrame, numSprites, spriteWidth, spriteHeight;//determines which sprite sheet to use based on position. updated via user input.
        public bool alive, fire, hit, fireCannonBall, openDoor;
        public Rectangle sourceRect, destRect;//determines size of base sprite. should stay consistent but is flexible. [flag2]



        /*
       * Player constructor, simple initializer - placement defaults to 0,0.
       * Requires further initializtion based on sprite sheet and other game variables. 
       * parameters: None
       * */
        //TO DO, INCLUDE ANOTHER CONSTRUCTOR VARIABLE FOR numSprites.
        public Player()
        {
            
        }

        /*
         * initializer. can be used as a reset for a player. great for manipulating all character variables for a variety of purposes. (think loading state). 
         * parameters: spritesheet, index
         * */
        public void initPlayerData(int numposes, float initX, float initY, float initRotation, float initFireTimer, int initSpriteWidth, int initSpriteHeight){
            mainSpriteSheet = new Texture2D[numposes];
            position.X = initX;
            position.Y = initY;
            rotation = initRotation;
            fireTimer = initFireTimer;
            velocity.X = 0f;
            velocity.Y = 0f;
            pose = 0;
            alive = true;
            numPoses = numposes;
            airborne = true;
            currentFrame = 0;
            spriteInterval = 3000f / 25f;
            spriteTimer = 0f;
            hitTimer = 0f;
            hitInterval = 6000f / 25f;
            this.numSprites = 1;
            spriteWidth = initSpriteWidth; // I want to reduce noise by removing these two vars and using the actual sprite width/height. 
            spriteHeight = initSpriteHeight; //only reason to keep if you want to mutate the sprite ingame
            energy = 200f; //25f is about half a second
            energyCapacity = 600f;
            health = 100f;
            healthCapacity = 100f;
            maxSpeed = 6f;

        }

        /*
         * The update method, handles character manipulation PER CYCLE given key variables (aka, the formal parameters. 
         * parameters: GameTime gameTime, KeyboardState previousKeyboardState, KeyboardState keyboardState, float backgroundX, float backgroundY
         * */
        public void update(GameTime gameTime, KeyboardState previousKeyboardState, KeyboardState keyboardState, float backgroundX, float backgroundY)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            progressionTimer += deltaTime;
            //this section updates the character based on the keyboard state. 
            //allRangeMode = true; 
            //===============================================
            destRect = new Rectangle(
                        (int)(position.X), //+ backgroundX),
                        (int)(position.Y), // + backgroundY),
                        mainSpriteSheet[pose].Width,
                        mainSpriteSheet[pose].Height);
           
            
            
            if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right))
            {
                if (velocity.X < 0)
                {
                    velocity.X += 2f;
                    if (velocity.X > 0) { velocity.X = 0f; }
                }
                else if (velocity.X > 0)
                {
                    velocity.X -= 2f;
                    if (velocity.X < 0) { velocity.X = 0f; }
                }

            }
            if (keyboardState.IsKeyDown(Keys.Up) && keyboardState.IsKeyDown(Keys.Left))
            {
                this.rotation = 3.9f;
                if (velocity.X > -6f)
                {
                    velocity.X -= 6f;//moves left at the per/second rate.
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Up) && keyboardState.IsKeyDown(Keys.Right))
            {
                this.rotation = 5.45f;
                if (velocity.X < 6f)
                {
                    this.velocity.X += 6f;
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Down) && keyboardState.IsKeyDown(Keys.Right))
            {
                this.rotation = (float)(Math.PI * .25f);
                if (velocity.X < 6f)
                {
                    this.velocity.X += 6f;
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Down) && keyboardState.IsKeyDown(Keys.Left))
            {
                this.rotation = (float)(Math.PI * .75f);
                if (velocity.X > -6f)
                {
                    velocity.X -= 6f;//moves left at the per/second rate.
                }
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                this.rotation = 1.57f;
            }
            else if (keyboardState.IsKeyDown(Keys.Left))
            {
                this.rotation = (float)Math.PI;
                if (velocity.X > -6f)
                {
                    velocity.X -= 6f;//moves left at the per/second rate.
                }

            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                this.rotation = 0f;
                if (velocity.X < 6f)
                {
                    this.velocity.X += 6f;
                }

            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                this.rotation = 4.71f;//4.7f;
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {

                fireCannonBall = true;
                //great test bed

            }
            
            float seconds = progressionTimer / 1000;
            
            if (keyboardState.IsKeyDown(Keys.Q) && !this.airborne)
            {
                this.velocity.Y = -20f;
                this.airborne = true;
            }

            // SPRITE UPDATE LOGIC ========================
            spriteTimer += deltaTime;
            if (spriteTimer > spriteInterval)
            {
                currentFrame++;
                if (!alive)
                {
                    currentFrame = 0;
                    //put deathcode here.
                }
                if (currentFrame > numSprites - 1)
                {
                    currentFrame = 0;
                }
                spriteTimer = 0f;
            }
            this.sourceRect = new Rectangle(currentFrame * this.spriteWidth,
                0, this.spriteWidth, this.mainSpriteSheet[this.pose].Height);

            // ENDEX SPRITE UPDATE LOGIC =================
            
            if (energy > energyCapacity) { energy = energyCapacity; }
            if (health > healthCapacity) { health = healthCapacity; }


            //determines his pose
            /* TODO - implement muliple poses after further game development milestones met. 
            if (!airborne && velocity.X > 0)
            {
                pose = 0;
            }
            else if (!airborne && velocity.X < 0)
            {
                pose = 1;

            }
            if (airborne && velocity.X > 0)//TODO: redefine duke's velocity and adjust this to read <0 || > 0
            {
                pose = 3;
            }
            else if (airborne && velocity.X < 0)//note velocity.X == 0 should NOT be a trigger for pose change, as Duke may face left or right when he stops moving. 
            {
                pose = 4;
            }
            */
            
            //determine if he is hit, and play the appropriate sequence of events. 
            /*
             *  what's happening here? 
             *  first, upon initial hit, the initial actions triggered by a 0f timer update duke for getting hit.
             *  He is sent slowly off to the right, and loses health or his shield. 
             *  the timer runs, providing a brief invicibility period. once the timer expires, hit and hitTimer is reset, ready for the next instance of Duke getting hit. 
             *  commented out code refers to legacy shield power-up...keep for future game dev purposes. 
             * 
             * */
            if (hit)
            {
                if (hitTimer == 0f)
                {
                    velocity.X = 3f;
                    velocity.Y = -10f;
                    //if (shield) { shield = false; }
                    //else { decrementHealth(10f); }
                    decrementHealth(10f);
                }
                hitTimer += deltaTime;
                if (hitTimer > hitInterval)
                {
                    hit = false;
                    hitTimer = 0f;
                }

            }
            this.position.X += this.velocity.X;
        }

        /*
         * Sprite loader
         * parameters: spritesheet, index
         * */
        public void loadSpriteSheet(Texture2D Sprite, int index)
        {
            if (index >= numPoses)
            {
                //do nothing prevents runtime error.
            }
            else
            {
                mainSpriteSheet[index] = Sprite;
            }
        }
        /*
        public void initDash() //[flag3]
        {
            if (this.dashReady)
            {
                this.dashing = true;
                this.dashReady = false;
            }
        }
        */
        public void increaseEnergy(float increment)
        {
            this.energy += increment;
        }

        public void decrementEnergy(float decrement)
        {
            this.energy -= decrement;
        }

        public void decrementHealth(float decrement)
        {
            this.health -= decrement;
        }

    }
}
