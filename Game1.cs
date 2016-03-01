using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<List<Texture2D>> levelTextures = new List<List<Texture2D>>();
        List<basicBlock> blocks = new List<basicBlock>();
        List<Character> characters = new List<Character>();
        Player Player = new Player();
        int numLevelSkins = 3; 
        int mapHeightTiles, mapWidthTiles, tileHeight, tileWidth; //to include in custom level class with blocks. 
        int currentLevel = 7;
        bool loadlLevel = true;
        KeyboardState previousKeyboardState, KeyboardState;
        float backgroundX, backgroundY; 
        float gravityAccel = 1f;
        Rectangle viewportRect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            // TODO: designed a more effective way & naming convention to load textures 'better'
            // TODO eligible for improvement after world has been designed by game designer. 
            // [ ] Prerequisite AI: Design the Game World content. 

            for (int i = 0; i < numLevelSkins; i++){
                levelTextures.Add(new List<Texture2D>());

            }
            levelTextures[0].Add(Content.Load<Texture2D>("Graphics\\block1"));
            for (int i = 1; i <= 2; i++)
            {
                levelTextures[0].Add(Content.Load<Texture2D>("Graphics\\block" + i.ToString()));
            }
            
            levelTextures[1].Add(Content.Load<Texture2D>("Graphics\\marble1"));
            for (int i = 1; i <= 2; i++)
            {
                levelTextures[1].Add(Content.Load<Texture2D>("Graphics\\marble" + i.ToString()));
            }
            levelTextures[2].Add(Content.Load<Texture2D>("Graphics\\dirt1"));
            for (int i = 1; i <= 2; i++)
            {
                levelTextures[2].Add(Content.Load<Texture2D>("Graphics\\dirt" + i.ToString()));
            }
            // TODO: remove this testbed code and provide a proper game generated initialization. ==========
            this.Player.initPlayerData(1, 0f, 124f, 0f, 0f, 81, 64);

            // TODO: inject dummy sprite sheets before art AI's take priority. 
            this.Player.loadSpriteSheet(Content.Load<Texture2D>("Graphics\\TALOS1"), 0);

            viewportRect = new Rectangle(0, 0,
                graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height);

            // ==================== END TODO TERRITORY =====================================================
            this.backgroundX = this.backgroundY = 0f; 

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // process room traversal events. 
            if (loadlLevel)
            {
                loadLevel(currentLevel);
                loadlLevel = false; 
            }

            // handle user input. 
            this.previousKeyboardState = this.KeyboardState; 
            this.KeyboardState = Keyboard.GetState();

            // update game objects. 
            this.Player.update(gameTime, this.previousKeyboardState, this.KeyboardState, this.backgroundX, this.backgroundY);
            
            // handle physics interactions/collisions. 
            UpdatePhysics(gameTime); // not done yet. 

            // handle Viewport panning.
            UpdatePanning();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();
            // TODO: Add your drawing code here

            // =======  DRAW THE LEVEL ==============================
            Rectangle destRect = new Rectangle();
            foreach (basicBlock bblock in blocks) {
               
                destRect = new Rectangle((int)(bblock.position.X - backgroundX), (int)(bblock.position.Y - backgroundY), bblock.length, bblock.height);
                spriteBatch.Draw(bblock.texture, destRect, Color.White);
            }
            // ======================================================

            // ======= DRAW THE PLAYER ==============================
            spriteBatch.Draw(Player.mainSpriteSheet[0], Player.destRect, Color.White);
            // ======================================================

            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected void loadLevel(int roomNumber){
            // clear the current room list. 
            this.blocks = new List<basicBlock>();
            //read library into json object. 
            TextReader tr = new StreamReader(roomNumber.ToString() + ".json");
            string level = tr.ReadToEnd();
            JObject loadLevel = JObject.Parse(level);

            // get JSON result objects into a list. 
            IList<JToken> placements = loadLevel["data"].Children().ToList();
            JToken mapHeightTiles = loadLevel["height"];
            JToken mapWidthTiles = loadLevel["width"];
            JToken tileHeight = loadLevel["tileheight"];
            JToken tileWidth = loadLevel["tilewidth"];
            JToken skin = loadLevel["skin"];

            this.mapHeightTiles = int.Parse(JsonConvert.DeserializeObject<string>(mapHeightTiles.ToString()));
            this.mapWidthTiles = int.Parse(JsonConvert.DeserializeObject<string>(mapWidthTiles.ToString()));
            this.tileHeight = int.Parse(JsonConvert.DeserializeObject<string>(tileHeight.ToString()));
            this.tileWidth = int.Parse(JsonConvert.DeserializeObject<string>(tileWidth.ToString()));
            int textureKey = int.Parse(JsonConvert.DeserializeObject<string>(skin.ToString()));
            
            //populate the list from the JSON results.
            basicBlock tempBlock = new basicBlock();
            List<string> rawPlacementData = new List<string>();
            string tempString;
            foreach (JToken result in placements)
                {
                    tempString = JsonConvert.DeserializeObject<string>(result.ToString());
                    rawPlacementData.Add(tempString);
                }
            int count = 0;
            for (int i = 0; i < this.mapHeightTiles; i++)
            {
                for (int j = 0; j < this.mapWidthTiles; j++)
                {
                    
                    if (rawPlacementData[(i * this.mapWidthTiles) + j] != "0") // todo: create generic methods of loading multiple map tile types. 
                    {
                        tempBlock = new basicBlock();
                        tempBlock.length = this.tileWidth;
                        tempBlock.height = this.tileHeight;
                        tempBlock.position.X = j * this.tileWidth;
                        tempBlock.position.Y = i * this.tileHeight;
                        //below...reading the write texture based on the instructions from JSON AND the levelTextures global list of textures. 
                        tempBlock.loadTexture(this.levelTextures[textureKey].ElementAt<Texture2D>(int.Parse(rawPlacementData[(i * this.mapWidthTiles) + j])));
                        this.blocks.Add(tempBlock);
                    }
                    count++;
                }

            }
        }

        /*==============================================
         * UpdatePhysics(float gameTime)
         * Purpose: Keeps track of physics collision detection and updates.
         * 
         * Class variables used: 
         *============================================== */

        protected void UpdatePhysics(GameTime gameTime)
        {
            if (this.Player.airborne)
            {
                //update player position and velocity. 
                // constant acceleration of gravity
                if (this.Player.velocity.Y < 10f)
                {
                    this.Player.velocity.Y += this.gravityAccel;
                }
                //update position. 
                if (this.Player.position.Y > 150 && this.Player.velocity.Y < 0)
                {
                    this.Player.position.Y += this.Player.velocity.Y;
                }
                else if (this.Player.position.Y <= 150 && backgroundY > 0f && this.Player.velocity.Y < 0f)
                {
                    this.backgroundY += this.Player.velocity.Y;
                }
                else if (this.Player.position.Y + this.Player.velocity.Y > 275 && this.Player.velocity.Y > 0f)
                {
                    this.backgroundY += this.Player.velocity.Y;
                }
                else { this.Player.position.Y += this.Player.velocity.Y; }


            }
            //reinitializes freefall state after processing gravity updates. collision detection with platforms will toggle this to false
            //where appropriate. 
            this.Player.airborne = true;
            /*foreach (Enemy enemy in enemies) we will need this when applying enemies. 
            {
                enemy.airborne = true;
            }*/


            //========================================== todo, handle dynamic infrastructurs.
            foreach (basicBlock block in this.blocks)
            {
                block.Update(this.backgroundX, this.backgroundY, gameTime);
            }


            //===========================================
            //Duke.falling = true;//might be unnecessary.
            //jumpReady = false;
            //debug This never enters after I push the A button.
            //TODO: DEBUG why player is stuck with half of body in platform.
            // collision detection logic. 
            foreach (basicBlock platform in this.blocks)
            {
                //TODO: Include legacy code for collision detection of all characters below, don't differentiate. 
                //TODO: port the code, but refactor to make a collision detection function with the platform general object. 
                //determines if duke should land on this platform. 
                if (this.Player.position.Y <= (platform.destinationRect.Y) && //bottom of player checks. 
                    this.Player.position.Y > (platform.destinationRect.Y) - (this.Player.mainSpriteSheet[Player.pose].Height) &&
                    this.Player.position.X >= (platform.destinationRect.X - this.Player.mainSpriteSheet[Player.pose].Width + 1f) &&
                    (float)this.Player.position.X < platform.texture.Width + (platform.destinationRect.X - 2f))
                {
                    //do nothing, do no update the Y component, unless the platform is moving vertically.

                    //next line "auto-snaps" player to a platform.
                    //TODO - make an enum for the 1. 
                    //TODO - see http://stackoverflow.com/questions/1642868/why-wont-reference-of-derived-class-work-for-a-method-requiring-a-reference-of-b zaloj comment. 
                    Player.alive = false; //bool used as a breaker to observe mutation of player in collisionAction method is persistent.
                    platform.collisionAction(1, Player);
                    Player.alive = true; //bool used as a breaker to determine if I need to pass via reference above. 
                    // ==============BOOK MARK==============================
                    /*
                    if ((platform.position.Y - backgroundY) <= 100 && backgroundY > 0f && platform.velocity.Y < 0f) //if on platform and moving out of sight, updates panning
                    {
                        this.backgroundY += platform.velocity.Y;
                        this.backgroundX += platform.velocity.X;
                        Duke.velocity.X -= platform.velocity.X;
                    }
                    else if ((platform.position.Y - backgroundY) + platform.velocity.Y > 350 && platform.velocity.Y > 0f)//see above comment.
                    {
                        this.backgroundY += platform.velocity.Y;
                        this.backgroundX += platform.velocity.X;
                        Duke.velocity.X -= platform.velocity.X;
                    }
                    */

                }

                //TODO: port the code, but refactor to make a collision detection function with the platform general object.
                //I am happy with the current conditions of the collisionDetection & collisionHandling...proceeding towards other aspects of devel. 
                //checks if duke is on the left boundary of a wall. OR right boundary
                if ((this.Player.position.X <= (platform.destinationRect.X) &&
                    this.Player.position.X + this.Player.mainSpriteSheet[Player.pose].Width > (platform.destinationRect.X)&&
                    this.Player.position.Y + this.Player.mainSpriteSheet[Player.pose].Height >= (platform.destinationRect.Y + 5f) &&
                (float)this.Player.position.Y < (platform.texture.Height + platform.destinationRect.Y)
                    && this.Player.velocity.X > 0) ||
                    (this.Player.position.X >= (platform.destinationRect.X + platform.texture.Width - 6f) &&
                    this.Player.position.X < (platform.destinationRect.X + platform.texture.Width) && this.Player.velocity.X < 0 &&
                    this.Player.position.Y + this.Player.mainSpriteSheet[Player.pose].Height >= (platform.destinationRect.Y + 5f) &&
                (float)this.Player.position.Y < (platform.texture.Height + platform.destinationRect.Y)
                    && this.Player.velocity.X < 0))
                {
                    //do nothing, do no update the Y component, unless the platform is moving vertically.
                    //next line "auto-snaps" player to a platform.
                    platform.collisionAction(4, Player);
                    /*
                    if ((platform.position.Y - backgroundY) <= 100 && backgroundY > 0f && platform.velocity.Y < 0f) //if on platform and moving out of sight, updates panning
                    {
                        this.backgroundY += platform.velocity.Y;
                        this.backgroundX += platform.velocity.X;
                        Duke.velocity.X -= platform.velocity.X;
                    }
                    else if ((platform.position.Y - backgroundY) + platform.velocity.Y > 350 && platform.velocity.Y > 0f)//see above comment.
                    {
                        this.backgroundY += platform.velocity.Y;
                        this.backgroundX += platform.velocity.X;
                        Duke.velocity.X -= platform.velocity.X;
                    }

                    */
                }
                //determines if duke hit the bottom of this wall and provides appropriate feedback (ie. Duke hit the wallcube's ceiling
                if (this.Player.position.Y >= (platform.destinationRect.Y) &&
                    this.Player.position.Y < platform.destinationRect.Y + 65f &&
                    this.Player.position.X >= (platform.destinationRect.X - 25f) &&
                    (float)this.Player.position.X < platform.texture.Width + (platform.destinationRect.X - 25f) && this.Player.velocity.Y < 0)
                {
                    //do nothing, do no update the Y component, unless the platform is moving vertically.

                    //this.Player.airborne = true;
                    //TODO: use this code >> after satisfactory QA and testing. - platform.collisionAction(1, Player);

                    platform.collisionAction(3, Player);
                    //this.Player.velocity.Y = 0f;


                }
            }
        }

        /*==============================================
         * UpdatePanning()
         * Purpose: Keeps track of Screenpanning with respect to
         * character movement, platforming, and arena modes. 
         * consider turning into a function of the Player class. 
         * Class variables used: 
         *
         *============================================== */

        public void UpdatePanning()
        {   //first X panning is analyzed/established, then Y panning. TODO: annotate right and left in each if statement with && vel.x>/<0
            
            
            if (Player.position.X > 200 && Player.velocity.X < 0)//allows duke to move freely left. 
            {
                //Duke.position.X -= maxSpeed;//moves left at the per/second rate.
            }
            if (Player.position.X <= 200 && backgroundX > Math.Abs(Player.velocity.X) && Player.velocity.X < 0)//allows duke to ride up against the left map boundary. DOES NOT prevent. 
            {
                this.backgroundX += Player.velocity.X; //remember, velocity can be a negative value. 
                this.Player.position.X -= Player.velocity.X; //undos dukes position vector update so the panning gets updated instead.
            }
            else if (backgroundX <= Player.mainSpriteSheet[Player.pose].Width&& Player.velocity.X < 0 && Player.position.X <= Player.mainSpriteSheet[Player.pose].Width / 6)//if passes, the leftrunning Duke will not run off the SCREEN. uses else if because previous if-statement. 
            {
                this.backgroundX = 0f;
                Player.position.X = 0f;
            }

            if (Player.velocity.X < 0 && Player.position.X <= Player.mainSpriteSheet[Player.pose].Width / 6)//if passes, the leftrunning Duke will not run off the SCREEN. uses else if because previous if-statement. 
            {
                Player.position.X -= Player.velocity.X;//undo any velocity vector affecting duke 

            }
            if (Player.velocity.X > 0 && Player.position.X > viewportRect.Width - Player.mainSpriteSheet[Player.pose].Width)
            {
                Player.position.X -= Player.velocity.X;//undo any velocity vector affecting duke 
            }
       
            if (Player.position.X < 400 && Player.velocity.X > 0) //allows duke to move freely right. 
            {
                this.Player.position.X += 0;
            }
            if (Player.position.X >= 400 && Player.velocity.X > 0) // allows right panning. 
            {
                this.backgroundX += Player.velocity.X; //remember, velocity can be a negative value. 
                this.Player.position.X -= Player.velocity.X;// undos duke's position vector update at start of method. 
            }
            this.Player.position.X += 0;
            
            //upon exiting this method, position does not update. 
        }
    }
}
