using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using ShapeUtils;

namespace GravitySnail
{
    

    public class Game1 : Game
    {
        

        //===========FIELDS==========
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private List<string> levels;

        private Button button;
        private TextButton winButton;
        private TextButton controlsButton;
        private TextButton menuFromControls;
        private SpriteFont arial30;

        private Level LoadedLevel;
        private Level TestLevel;

        private int monitorWidth ;
        private int monitorHeight;
        private float aspectRatio;
        private const int textureResolutions = 560;

        private List<TextButton> levelSelectButtons;

        private bool doDebugDraw = false;

        //========PROPERTIES=========

        public Dictionary<string, Texture2D> Textures;

        public int LevelCount
        {
            get { return levels.Count; }
        }

        //======CONSTRUCTOR=======
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            monitorWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            monitorHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            aspectRatio = (float)monitorWidth / (float)monitorHeight;
            _graphics.PreferredBackBufferWidth = (int)(monitorHeight * aspectRatio);
            _graphics.PreferredBackBufferHeight = monitorHeight;
            _graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        //======METHHODS========
        protected override void Initialize()
        {
            //offset math!
            int offsetX = (_graphics.PreferredBackBufferWidth - textureResolutions) / 2;
            int offsetY = (_graphics.PreferredBackBufferHeight - textureResolutions) / 2;

            Reference.transformationMatrix = Matrix.CreateTranslation(offsetX, offsetY, 0);

            Debug.WriteLine(Matrix.CreateRotationX(MathF.PI * 1.5f));


            // TODO: Add your initialization logic here
            Reference.FrameOneSetup(this);
            
            Textures = new Dictionary<string, Texture2D>();

            base.Initialize();

            //create levels and populate it *in the correct order*
            levels = new List<string>();

            // tutorial-like levels
            levels.Add("Level1Basic.level");
            levels.Add("Level2Platformer.level");
            levels.Add("Level3Hazards.level");
            levels.Add("WatchYourStep.level");
            levels.Add("Level4Switches.level");
            levels.Add("UsingCrates.level"); 


            // some real puzzle stuff
            levels.Add("LockedOut.level");
            levels.Add("Gated.level"); 
            levels.Add("NewSlide.level");
            levels.Add("Heist.level");
            

            



            // spawns the player in the center of the window
            //player = new Player(playerTexture, new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2 - 200));

            button = new Button(new Vector2(180 + Reference.transformationMatrix.M41,280 + Reference.transformationMatrix.M42), Textures["PlayButton"], Textures["PlayButton2"]);
            button.OnDefaultClick += ChangeState;

            winButton = new TextButton(new Vector2(180 + Reference.transformationMatrix.M41, 510 + Reference.transformationMatrix.M42), Textures["blankButton"], Textures["blankButton"], "Play Again", arial30, Color.MediumPurple, Color.DarkBlue);
            winButton.OnDefaultClick += ChangeState;
            winButton.OnDefaultClick += UnloadLevel;

            controlsButton = new TextButton(new Vector2(360 + Reference.transformationMatrix.M41, 510 + Reference.transformationMatrix.M42), Textures["blankButton"], Textures["blankButton"], "controls", arial30, Color.DarkCyan, Color.WhiteSmoke);
            controlsButton.OnDefaultClick += ToControls;

            menuFromControls = new TextButton(new Vector2(180 + Reference.transformationMatrix.M41, 0 + Reference.transformationMatrix.M42), Textures["blankButton"], Textures["blankButton"], "back", arial30, Color.Black, Color.WhiteSmoke);
            menuFromControls.OnDefaultClick += ChangeState;


            //populates the level select screen with buttons
            levelSelectButtons = new List<TextButton>();
            int countInRow = 7;
            for(int i=0; i<levels.Count; i++)
            {
                levelSelectButtons.Add(new TextButton(new Vector2((i%countInRow)*80 + Reference.transformationMatrix.M41, (i/countInRow)*80 + Reference.transformationMatrix.M42),
                    Textures["LevelButton1"], Textures["LevelButton2"], (i + 1).ToString(), arial30,Color.White,Color.WhiteSmoke,1));

                levelSelectButtons[i].IntClickArgument = i;
                levelSelectButtons[i].OnIntClick += SelectLevel;
            }

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //====LOAD TEXTURES====

            Textures["Player"] = Content.Load<Texture2D>("Player");
            Textures["PlatformBox"] = Content.Load<Texture2D>("PlatformBox");
            Textures["PlatformCorner"] = Content.Load<Texture2D>("PlatformCorner");
            Textures["PlatformCap"] = Content.Load<Texture2D>("PlatformCap");
            Textures["PlatformSides"] = Content.Load<Texture2D>("PlatformSides");
            Textures["PlatformSingle"] = Content.Load<Texture2D>("PlatformSingle");
            Textures["DeathOverlay"] = Content.Load<Texture2D>("deathOverlay");
            Textures["TitleScreen"] = Content.Load<Texture2D>("titleScreen");
            Textures["WinScreen"] = Content.Load<Texture2D>("winScreen");
            Textures["ControlsScreen"] = Content.Load<Texture2D>("controls");
            Textures["PlayButton"] = Content.Load<Texture2D>("button1");
            Textures["PlayButton2"] = Content.Load<Texture2D>("button2");
            Textures["blankButton"] = Content.Load<Texture2D>("blankButton");
            Textures["LevelButton1"] = Content.Load<Texture2D>("levelButton1");
            Textures["LevelButton2"] = Content.Load<Texture2D>("levelButton2");
            Textures["LaserFrame1"] = Content.Load<Texture2D>("TestingAnimations/Laser1");

            Textures["Platform3Wide"] = Content.Load<Texture2D>("Platform3Wide");
            Textures["Platform5Wide"] = Content.Load<Texture2D>("Platform5Wide");

            Textures["RedButton"] = Content.Load<Texture2D>("RedButton");
            Textures["BlueButton"] = Content.Load<Texture2D>("BlueButton");

            Textures["Spike"] = Content.Load<Texture2D>("Spike");
            Textures["SpikesHalf"] = Content.Load<Texture2D>("SpikesHalf");
            Textures["Box"] = Content.Load<Texture2D>("Box");
            Textures["Goal"] = Content.Load<Texture2D>("Goal");

            arial30 = Content.Load<SpriteFont>("Arial30");

            for (int i = 1; i<= 6; i++)
            {
                Texture2D laserTexture = Content.Load<Texture2D>("TestingAnimations/Laser" + i);
                //multiple instances for a slower animation
                Reference.Animator.AddFrameToAnimation("Laser", laserTexture);
                Reference.Animator.AddFrameToAnimation("Laser", laserTexture);
                Reference.Animator.AddFrameToAnimation("Laser", laserTexture);
                Reference.Animator.AddFrameToAnimation("Laser", laserTexture);
            }


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Reference.GatherInput();

            //check debug 

            if(Reference.KBState.IsKeyDown(Keys.M) && !Reference.PrevKBState.IsKeyDown(Keys.M))
            {
                doDebugDraw = !doDebugDraw;
            }

            
            
            //Game Logic!
            switch(Reference.GameState)
            {
                case GameState.Menu:
                    {
                        // goes to main
                        button.Update(gameTime);
                        controlsButton.Update(gameTime);
                        // access to test has been commented out
                        // goes to test
                        //if (Reference.KBState.IsKeyDown(Keys.T))
                        //{
                        //    //LoadedLevel = TestLevel;
                        //    TestLevel = LevelEditor.LoadLevel("Platforms.level", 1);
                        //    Reference.GameState = GameState.Test;
                        //}

                        break;
                    }
                case GameState.Game:
                    {   
                        //Frame1 of level efforts (i.e., load level in)
                        if(Reference.PrevGameState != GameState.Game)
                        {
                            Debug.WriteLine("loaded Level");

                            if(LoadedLevel != null)
                            {
                                int levelNumber = LoadedLevel.LevelNumber;
                                LoadedLevel.ClearObjects();
                                LoadedLevel.SetObjects();
                            }


                            _graphics.PreferredBackBufferWidth = 560;
                            _graphics.PreferredBackBufferHeight = 560;

                        }

                        //update all the level Logic as long as the level is loaded
                        if(LoadedLevel != null)
                        {
                            LoadedLevel.Update(gameTime);
                        }

                        break;
                    }
                case GameState.Test:
                    {
                        TestLevel.Update(gameTime);
                        break;
                    }
                case GameState.GameOver:
                    {
                        if (Reference.KBState.IsKeyDown(Keys.R))
                        {
                            Reference.GameState = GameState.Game;
                            //player.SpawnPlayer();
                            LoadedLevel.Player.SpawnPlayer();
                        }
                        break;
                    }
                case GameState.Controls:
                    {
                        menuFromControls.Update(gameTime);
                        break;
                    }
                case GameState.LevelSelect:
                    {
                        foreach (TextButton b in levelSelectButtons)
                        {
                            b.Update(gameTime);
                        }
                        break;
                    }
                case GameState.Win:
                    {
                        winButton.Update(gameTime);
                        break;
                    }
            }


            Reference.EndOfFrame();
            
            base.Update(gameTime);
            //Debug.WriteLine("Update Done");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.Deferred,null,null,null,null,null,Reference.transformationMatrix);

            // spawns him the correct way up!!!
            

            switch (Reference.GameState)
            {
                case GameState.Menu:
                    {
                        _spriteBatch.Draw(Textures["TitleScreen"], new Vector2(0), Color.White);
                        button.Draw(_spriteBatch, Color.White);
                        controlsButton.Draw(_spriteBatch, Color.White);
                        break;
                    }
                case GameState.Game:
                    {
                        if(LoadedLevel != null)
                        {
                           LoadedLevel.Draw(_spriteBatch, _graphics);
                        }
                        break;
                    }
                case GameState.Test:
                    {
                        TestLevel.Draw(_spriteBatch, _graphics);
                        break;
                    }
                case GameState.GameOver:
                    {
                        //once we have Level.Draw working we'd just call that here but with the color gray
                        //draws all the game objects on screen but stops updating them so they're frozen on the death screen
                        LoadedLevel.DrawWithTint(_spriteBatch, Color.DarkGray);

                        _spriteBatch.Draw(Textures["DeathOverlay"], new Vector2(0), Color.White);
                        break;
                    }
                case GameState.Controls:
                    {
                        _spriteBatch.Draw(Textures["ControlsScreen"], new Vector2(0), Color.White);
                        menuFromControls.Draw(_spriteBatch, Color.PaleGoldenrod);
                        break;
                    }
                case GameState.LevelSelect:
                    {
                        foreach(TextButton b in levelSelectButtons)
                        {
                            b.Draw(_spriteBatch, Color.White);
                        }
                        break;
                    }
                case GameState.Win:
                    {
                        _spriteBatch.Draw(Textures["WinScreen"], new Vector2(0), Color.White);
                        winButton.Draw(_spriteBatch, Color.White);
                        break;
                    }
            }

            

            

            _spriteBatch.End();

            //Shapebatch for debugging
            ShapeBatch.Begin(GraphicsDevice);
            
            if(doDebugDraw && LoadedLevel != null)
            {
                foreach (GameObject obj in LoadedLevel.LevelObjects)
                {
                    if (obj != null)
                    {
                        Color col;
                        switch(obj.Direction)
                        {
                            case Facing.Up:
                                col = Color.LimeGreen;
                                break;

                            case Facing.Right:
                                col = Color.White;
                                break;

                            case Facing.Left:
                                col = Color.Yellow;
                                break;


                            default:
                                col = Color.Magenta;
                                break;
                        }
                        Rectangle objBounds = obj.Bounds;
                        Rectangle RectShifted = new Rectangle(objBounds.X + (int)Reference.transformationMatrix.M41, objBounds.Y + (int)Reference.transformationMatrix.M42, 
                            objBounds.Width, objBounds.Height);
                        ShapeBatch.BoxOutline(RectShifted, col);
                        if(obj.DrawOffset != Vector2.Zero)
                        {
                            RectShifted = new Rectangle(objBounds.X + (int)Reference.transformationMatrix.M41 - (int)obj.DrawOffset.X, objBounds.Y + (int)Reference.transformationMatrix.M42 - (int)obj.DrawOffset.Y,
                                (int)obj.DrawOffset.X, (int)obj.DrawOffset.Y);
                            
                            ShapeBatch.Box(RectShifted, col);
                        }

                        ShapeBatch.Circle(obj.Position + new Vector2((int)Reference.transformationMatrix.M41, (int)Reference.transformationMatrix.M42), 3, Color.White);

                        ShapeBatch.Line(obj.Position + new Vector2((int)Reference.transformationMatrix.M41, (int)Reference.transformationMatrix.M42), 
                            obj.Position + Vector2.Transform(obj.Offset, obj.GetRotationMatrix()) + new Vector2((int)Reference.transformationMatrix.M41, (int)Reference.transformationMatrix.M42), 
                            col);



                    } //end if !null
                    
                }//end foreach
            } //end debug


            ShapeBatch.End();


            base.Draw(gameTime);
            //Debug.WriteLine("Frame Drawn");
        } //end Draw

        /// <summary>
        /// writes a line to the debug console for testing
        /// </summary>
        public void DebugMethod()
        {
            Debug.WriteLine("The debugging method has been called!");
        }

        /// <summary>
        /// switches between level select state and menu state
        /// </summary>
        public void ChangeState()
        {
            if(Reference.GameState==GameState.Menu)
            {
                Reference.GameState = GameState.LevelSelect;
            }
            else
            {
                Reference.GameState = GameState.Menu;
            }
        }

        /// <summary>
        /// Sets the currently loaded level to the level at the position in the levels array
        /// </summary>
        /// <param name="levelNumber"></param>
        public void LoadLevel(int levelNumber)
        {
            //check if level is out of bounds of our array - if it is, don't let it run and throw an exception
            if(levelNumber < 0 || levelNumber >= levels.Count)
            {

                throw new ArgumentOutOfRangeException($"LoadLevel method failed, levelNumber {levelNumber} is not a valid index in level list");
            }

            //load the level from file
            LoadedLevel = LevelEditor.LoadLevel(levels[levelNumber], levelNumber);

            //create all the objects
            LoadedLevel.SetObjects();
            

            //update the reference level number for advancing
            Reference.LoadedLevelIndex = levelNumber;
        }

        /// <summary>
        /// changes state to controls
        /// </summary>
        public void ToControls()
        {
            Reference.GameState = GameState.Controls;
        }

        /// <summary>
        /// starts a given level from the level select screen
        /// </summary>
        /// <param name="level"></param>
        public void SelectLevel(int level)
        {
            LoadLevel(level);
            Reference.GameState = GameState.Game;
        }

        /// <summary>
        /// unloads the currently loaded level
        /// </summary>
        public void UnloadLevel()
        {
            LoadedLevel = null;
        }
    }
}