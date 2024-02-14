using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GravitySnail
{
    //=====GLOBALLY USEFUL ENUMS========
    /// <summary>
    /// Used to represent a directional facing
    /// </summary>
    public enum Facing
    {
        Down,
        Up,
        Left,
        Right
    }

    /// <summary>
    /// The possible states the game could be in
    /// </summary>
    public enum GameState
    { 
        Menu, 
        LevelSelect, 
        Game, 
        GameOver, 
        Pause, 
        Win,
        Test,
        Controls
    };


    /// <summary>
    /// The Possible tags the game objects can have
    /// </summary>
    public enum Tags
    {
        Collidable,
        Dangerous,
        Goal,
        RedConnection
    }

    public delegate bool switchDelegate(Tags tag, bool value);


    /// <summary>
    /// A Static Class that holds a lot of game information and helpful universal methods
    /// Use to gather input and check control GameState across many objects.
    /// </summary>
    public static class Reference
    {
        //=========CONSTANTS========
        public const float BaseMaxSpeed = 275;
        public const float DragFactor = .5f;
        public const float GroundDragFactor = .2f;
        public const int TargetFramerate = 60;



        //=========FIELDS========= 
            //Values set in FrameOneSetup()
        private static Game1 game1Reference;
        private static GameState gameState;
        private static GameState prevGameState;
        private static GameState prevPrevGameState;
        private static Facing currentGravity;
        private static float gravityStrength;
        private static int tileSize;

        private static MouseState currentMouse;
        private static MouseState previousMouse;

        private static KeyboardState kbState;
        private static KeyboardState prevKBState;

        private static Dictionary<Color, bool> colorDictionary = new Dictionary<Color, bool>();

        private static AnimatorControl animator;
        public static event switchDelegate RedLaserOnOff;




        //=========PROPERTIES============
        /// <summary>
        /// The Current Keyboard state of the game
        /// </summary>
        public static KeyboardState KBState
        {
            get { return kbState; }
        }

        /// <summary>
        /// The Previous Frame's Keyboard State
        /// </summary>
        public static KeyboardState PrevKBState
        {
            get { return prevKBState; }
        }

        /// <summary>
        /// The Current Mouse State of the Game
        /// </summary>
        public static MouseState CurrentMouse
        {
            get { return currentMouse; }
        }

        /// <summary>
        /// The Previous Frame's Mouse State
        /// </summary>
        public static MouseState PreviousMouse
        {
            get { return previousMouse; }
        }

        /// <summary>
        /// The Current state of the Game
        /// </summary>
        public static GameState GameState
        {
            get { return gameState; }
            set { gameState = value; }
        }

        /// <summary>
        /// The Previous Frame's Game State (for One Time Transition effects, will actually be 2 frames behind)
        /// </summary>
        public static GameState PrevGameState
        {
            get { return prevPrevGameState; }
        }


        /// <summary>
        /// The Current Facing in the game
        /// </summary>
        public static Facing CurrentGravity
        {
            get { return currentGravity; }
            set { currentGravity = value; }
        }

        /// <summary>
        /// The Strength of the Gravity for the world 
        /// </summary>
        public static float GravityStrength
        {
            get { return gravityStrength; }
            set { gravityStrength = value; }
        }

        public static Dictionary<Color, bool> ColorDictionary
        {
            get { return colorDictionary; }
        }

        /// <summary>
        /// The Animator control for the level, will be where the animations are instatiated and kept 
        /// </summary>
        internal static AnimatorControl Animator
        {
            get { return animator; }
        }


        /// <summary>
        /// The Number of the currently loaded level
        /// </summary>
        public static int LoadedLevelIndex;

        /// <summary>
        /// The reference of the game that is run in FrameOneSetup
        /// </summary>
        public static Game1 Game1Reference
        {
            get { return game1Reference; }
        }

        public static Matrix transformationMatrix;

        public static Matrix rotationMatrix0;
        public static Matrix rotationMatrix90;
        public static Matrix rotationMatrix180;
        public static Matrix rotationMatrix270;

        public static int TileSize
        {
            get { return tileSize; }
            set { tileSize = value; }
        }


        //=====================METHDOS=========================
        /// <summary>
        /// Gathers the current Keyboard and Mouse States and updates fields appropriately 
        /// </summary>
        public static void GatherInput()
        {
            kbState = Keyboard.GetState();
            currentMouse = Mouse.GetState();
        }

        /// <summary>
        /// Runs end of Frame processes:
        ///     Updates prevKBState and previousMouse to equal kbState and currentMouse
        /// </summary>
        public static void EndOfFrame()
        {
            prevKBState = kbState;
            previousMouse = currentMouse;
            prevPrevGameState = prevGameState;
            prevGameState = GameState;
        }

        /// <summary>
        /// To prevent Frame1 Null Reference issues and initialize values
        /// </summary>
        public static void FrameOneSetup(Game1 game1Reference)
        {
            //this. does not work because static
            Reference.game1Reference = game1Reference;

            previousMouse = new MouseState(); 
            prevKBState = new KeyboardState();
            gameState = GameState.Menu;
            prevGameState = GameState.Menu;

            gravityStrength = 100;

            //setup matrix reference values
            rotationMatrix0   = Matrix.CreateRotationZ(0);
            rotationMatrix90  = Matrix.CreateRotationZ(MathF.PI / 2);
            rotationMatrix180 = Matrix.CreateRotationZ(MathF.PI );
            rotationMatrix270 = Matrix.CreateRotationZ(MathF.PI * 3 / 2);


            // manually set up the coloredConnections 
            colorDictionary.Add(Color.Red, true);
            colorDictionary.Add(Color.Blue, true);
            colorDictionary.Add(Color.Green, true);

            animator = new AnimatorControl();

        }

        /// <summary>
        /// checks for a single click of the Left Mouse Button
        /// </summary>
        /// <returns></returns>
        public static bool SingleClick()
        {
            return (currentMouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton == ButtonState.Released);
        }

        public static Rectangle RectangeFromCenter()
        {
            return new Rectangle();
        }

        public static void callRedLaser(Tags tag, bool value)
        {
            if(RedLaserOnOff != null)
            {
                RedLaserOnOff.Invoke(tag, value);
            }
            
        }

    }
}
