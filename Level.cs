using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GravitySnail
{

    /// <summary>
    /// represents a single game level
    /// can be created by loading in a file made with the level editor
    /// </summary>
    internal class Level
    {
        // === Fields ===
        protected int levelNumber;
        protected int tileSize;
        protected string name;
        public string[,] levelData;
        protected GameObject[,] levelObjects;
        protected Player player;
        protected Vector2 spawnLocation;

        protected List<(MoveableObject, GameObject)> ResolveCollisions;

        protected int width;
        protected int height;

        // === Properties ===
        /// <summary>
        /// The Numeric Code Associated with this Level
        /// </summary>
        public int LevelNumber { get; }
        /// <summary>
        /// The Player Reference for the currently loaded player
        /// </summary>
        public Player Player { get { return player; } }

        public GameObject[,] LevelObjects
        {
            get { return levelObjects; }
        }

        // === CONSTRUCTOR === 
        public Level(int number, int width, int height)
        {
            levelNumber = number;
            levelData = new string[width,height];
            levelObjects = new GameObject[width,height];
            tileSize = 32;

            this.width = width;
            this.height = height;

            ResolveCollisions = new List<(MoveableObject, GameObject)>();

            Reference.TileSize = tileSize;

            //TO DO: Load in level data from text file
        }

        // === Methods ===
        /// <summary>
        /// Set Object creates all of the objects for the level and adds the needed tags to them. 
        /// It also resets certain values between levels
        /// </summary>
        public void SetObjects()
        {
            //Reset Needed Values
            Reference.ColorDictionary[Color.Red] = true;
            Reference.ColorDictionary[Color.Green] = true;
            Reference.ColorDictionary[Color.Blue] = true;
            Reference.CurrentGravity = Facing.Down;

            bool laserCheck = true;


            //Create objects
            for (int i = 0; i < levelData.GetLength(0); i++)
            {
                for (int j = 0; j < levelData.GetLength(1); j++)
                {
                    switch (levelData[i, j][0])
                    {
                        case '0': //Hazards
                            Texture2D textureType = Reference.Game1Reference.Textures["Spike"];
                            Vector2 Size = new Vector2(tileSize, tileSize);
                            Vector2 Offset = new Vector2(0, 0);

                            if (levelData[i,j].Length > 2)
                            {
                                switch (levelData[i, j][1])
                                {
                                    case 's':
                                        textureType = Reference.Game1Reference.Textures["SpikesHalf"];
                                        Size.Y = tileSize / 2;
                                        Offset.Y = tileSize / 2;
                                        break;

                                    case 't':
                                    default:
                                        //tall spikes are the defualt
                                        break;
                                }
                            }



                            levelObjects[i, j] = new GameObject(new Vector2(i * tileSize, j * tileSize), (int)Size.X, (int)Size.Y, textureType, (int)Offset.X, (int)Offset.Y);
                            levelObjects[i, j].DrawOffset = new Vector2(5, 5);
                            levelObjects[i, j].Tags.Add(Tags.Dangerous);

                            break;
                        case '1': //Tiles (Static platforms)

                            textureType = Reference.Game1Reference.Textures["PlatformBox"];
                            //read obj type
                            if (levelData[i, j][1] == 'b' || levelData[i, j][1] == 'e' || levelData[i, j][1] == 's' || levelData[i, j][1] == 'c' || levelData[i, j][1] == 'o')
                            {
                                switch (levelData[i, j][1])
                                {
                                    case 'b':
                                        //box is the default
                                        break;

                                    case 'e':
                                        textureType = Reference.Game1Reference.Textures["PlatformCap"];
                                        break;

                                    case 's':
                                        textureType = Reference.Game1Reference.Textures["PlatformSides"];
                                        break;

                                    case 'c':
                                        textureType = Reference.Game1Reference.Textures["PlatformCorner"];
                                        break;

                                    case 'o':
                                        textureType = Reference.Game1Reference.Textures["PlatformSingle"];
                                        break;
                                }
                            }




                            levelObjects[i, j] = new GameObject(new Vector2(i * tileSize, j * tileSize), tileSize, tileSize, textureType, 0, 0);
                            levelObjects[i, j].DrawOffset = new Vector2(5,5);
                            levelObjects[i, j].Tags.Add(Tags.Collidable);

                            break;
                        case '2': //Goal
                            levelObjects[i, j] = new GameObject(new Vector2(i * tileSize-5, j * tileSize-5), tileSize, tileSize, Reference.Game1Reference.Textures["Goal"], 0,0);
                            levelObjects[i, j].Tags.Add(Tags.Goal);
                            break;
                        case '3': //Player
                            spawnLocation = new Vector2(i * tileSize, j * tileSize);
                            levelObjects[i, j] = new Player(Reference.Game1Reference.Textures["Player"], spawnLocation);
                            this.player = (Player)levelObjects[i, j];
                            this.player.SpawnPosition = spawnLocation;
                            break;
                        case '4': //Immortal Snail
                            levelObjects[i, j] = new MoveableObject(new Vector2(i * tileSize, j * tileSize), Reference.Game1Reference.Textures["Player"]);
                            levelObjects[i, j].Tags.Add(Tags.Dangerous);
                            break;
                        case '5': //Puzzle Assets

                            //read color
                            Color col = Color.White;
                            string colName = "";
                            switch(levelData[i,j].Substring(1, 1))
                            {
                                case "r":
                                    col = Color.Red;
                                    colName = "red";
                                    break;

                                case "g":
                                    col = Color.Green;
                                    colName = "green";
                                    break;

                                case "b":
                                    col = Color.Blue;
                                    colName = "blue";
                                    break;
                            }

                            if (levelData[i,j].Length >=3)
                            {
                                switch (levelData[i, j].Substring(2, 1))
                                {
                                    //lasers
                                    case "l":
                                        Vector2 size = new Vector2(tileSize, 10);


                                        levelObjects[i, j] = new GameObject(new Vector2(i * tileSize - 5, j * tileSize - 5), (int)size.X, (int) size.Y, Reference.Game1Reference.Textures["LaserFrame1"], 0, 10 );

                                        switch (colName)
                                        {
                                            case "red":
                                                Reference.RedLaserOnOff += levelObjects[i, j].SetTagTo;
                                                break;

                                            case "green":
                                                //put events here
                                                break;

                                            case "blue":
                                                //put events here
                                                break;
                                        }

                                        levelObjects[i, j].AnimationTextures = Reference.Animator.AnimationDictionary["Laser"];
                                        Reference.Animator.AddObjectToAnimationList(levelObjects[i, j]);

                                        if (levelData[i, j][3] == 'i')
                                        {
                                            Reference.ColorDictionary[col] = false;
                                            laserCheck = false;
                                        }

                                        break;
                                    //buttons
                                    case "b":
                                        
                                        levelObjects[i, j] = new SwitchObject(new Vector2(i * tileSize - 5, j * tileSize - 5),
                                                                              tileSize,
                                                                              tileSize/4,
                                                                              Reference.Game1Reference.Textures["RedButton"],
                                                                              0,
                                                                              28,
                                                                              col);

                                        break;

                                    default:
                                        levelObjects[i, j] = new SwitchObject(new Vector2(i * tileSize, j * tileSize),
                                                                              tileSize,
                                                                              tileSize/4,
                                                                              Reference.Game1Reference.Textures["RedButton"],
                                                                              0,
                                                                              28,
                                                                              Color.White);



                                        break;
                                }
                            }
                            
                            break;
                        case '6': //Platform Assets (Moveable platforms)
                            if (levelData[i,j].Length > 2) //Moveable Platforms 
                            {
                                int size = int.Parse(levelData[i, j].Substring(2, 1));
                                Texture2D texture = null;
                                switch(size)
                                {
                                    case 3:
                                        texture = Reference.Game1Reference.Textures["Platform3Wide"];
                                        break;

                                    case 5:
                                        texture = Reference.Game1Reference.Textures["Platform5Wide"];
                                        break;
                                }

                                Vector2 position = new Vector2(i * tileSize, j * tileSize);
                                Vector2 maxPosition = position;
                                switch (levelData[i, j].Substring(1, 1))
                                {
                                    case "X":
                                        maxPosition += new Vector2(tileSize * int.Parse(levelData[i, j].Substring(3, 1)), 0);
                                        break;
                                      
                                    case "Y":
                                        maxPosition += new Vector2(0, tileSize * int.Parse(levelData[i, j].Substring(3, 1)));
                                        break;
                                }



                                levelObjects[i, j] = new ClampedMoveableObject(position, size * tileSize, tileSize,
                                            texture, 0,0, position, maxPosition);
                                levelObjects[i,j].DrawOffset = new Vector2(5, 5);
                                levelObjects[i, j].Tags.Add(Tags.Collidable);
                            }
                            else //Crate
                            {
                                levelObjects[i, j] = new MoveableObject(new Vector2(i * tileSize, j * tileSize), tileSize, tileSize, Reference.Game1Reference.Textures["PlatformBox"], 0, 0);
                                levelObjects[i,j].DrawOffset = new Vector2(5, 5);
                                levelObjects[i, j].Tags.Add(Tags.Collidable);
                            }
                            break;
                       
                        default:
                            break;
                    } //end switch

                    //if the object exists, read the rotation - rotation exists for ALL objects and will ALWAYS be the last character
                    if(levelObjects[i, j] != null)
                    {
                        //see if the obj has a rotation value
                        char lastChar = levelData[i, j][levelData[i, j].Length-1];
                        if (lastChar == 'U' || lastChar == 'D' || lastChar == 'L' || lastChar == 'R')
                        {
                            switch(lastChar)
                            {
                                case 'U':
                                    levelObjects[i, j].Direction = Facing.Up;
                                    break;

                                case 'D':
                                    levelObjects[i, j].Direction = Facing.Down;
                                    break;

                                case 'L':
                                    levelObjects[i, j].Direction = Facing.Left;
                                    break;

                                case 'R':
                                    levelObjects[i, j].Direction = Facing.Right;
                                    break;
                            }
                        }
                    }

                }
            }//end for loop creating things

            Reference.callRedLaser(Tags.Dangerous, laserCheck);

        } //end SetObjects

        /// <summary>
        /// Draws the level to the screen
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb, GraphicsDeviceManager graphics)
        {
            for (int i = 0; i < levelData.GetLength(0); i++)
            {
                for (int j = 0; j < levelData.GetLength(1); j++)
                {
                    switch (levelData[i, j][0])
                    {
                        case '0': //Hazards
                            levelObjects[i, j].Draw(sb, Color.Red);
                            break;
                        case '1': //Tiles
                            levelObjects[i, j].Draw(sb, Color.Orange);
                            break;
                        case '2': //Goal
                            levelObjects[i, j].Draw(sb, Color.Yellow);
                            break;
                        case '3': //Player
                            levelObjects[i, j].Draw(sb, Color.White);
                            break;
                        case '4': //Immortal Snail
                            levelObjects[i, j].Draw(sb, Color.Black);
                            break;
                        case '5': //Puzzle Assets
                            switch (levelData[i, j].Substring(1,2))
                            {
                                case "rl":
                                    if (Reference.ColorDictionary[Color.Red])
                                    {
                                        levelObjects[i, j].Draw(sb, Color.Red);
                                    }
                                    break;
                                case "gl":
                                    if (Reference.ColorDictionary[Color.Green])
                                    {
                                        levelObjects[i, j].Draw(sb, Color.Green);
                                    }
                                    break;
                                case "bl":
                                    if (Reference.ColorDictionary[Color.Blue])
                                    {
                                        levelObjects[i, j].Draw(sb, Color.Blue);
                                    }
                                    break;
                                case "rb":
                                    levelObjects[i, j].Draw(sb, Color.White);
                                    break;
                                case "gb":
                                    levelObjects[i, j].Draw(sb, Color.White);
                                    break;
                                case "bb":
                                    levelObjects[i, j].Draw(sb, Color.White);
                                    break;
                                default:
                                    levelObjects[i, j].Draw(sb, Color.White);
                                    break;
                            }
                            break;
                        case '6': //Platform Assets
                            levelObjects[i, j].Draw(sb, Color.Blue);
                            break;
                        case '7': // Clamped moveable object
                            levelObjects[i, j].Draw(sb, Color.BlueViolet);
                            break;
                        default:
                            break;
                    }
                }
            }
        } //end Draw

        /// <summary>
        /// Draws the entire level to the Screen with the given tint
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="_graphicsDevice"></param>
        /// <param name="tint"></param>
        public void DrawWithTint(SpriteBatch sb, Color tint)
        {
            foreach(GameObject obj in levelObjects)
            {
                if(obj != null)
                {
                    obj.Draw(sb, tint);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            Reference.Animator.AnimateAllObjects();

            ResolveCollisions.Clear();

            foreach (GameObject obj in levelObjects)
            {
                if (obj != null)
                {
                    obj.Update(gameTime);
                    
                    if (obj is MoveableObject)
                    {
                        MoveableObject moveableObject = (MoveableObject)obj;
                        

                        foreach (GameObject gameObject in levelObjects)
                        {
                            if (gameObject != null && gameObject != moveableObject)
                            {
                                if (gameObject.HasTag(Tags.Collidable))
                                {
                                    ResolveCollisions.Add((moveableObject, gameObject));
                                }

                                // turns the switch on/off
                                if (gameObject is SwitchObject)
                                {
                                    (gameObject as SwitchObject).CheckCollision(moveableObject);
                                }

                                //for each movable object, if it's the goal,
                                //and if the movable object is the player,
                                //and if they are colliding, load the next level
                                if(gameObject.HasTag(Tags.Goal) && obj is Player && (obj as Player).IsCollidingWith(gameObject) )
                                {
                                    Reference.CurrentGravity = Facing.Down;
                                    if (levelNumber + 1 >= Reference.Game1Reference.LevelCount)
                                    {
                                        Reference.GameState = GameState.Win;
                                    }
                                    else
                                    {
                                         try
                                         {
                                             Reference.Game1Reference.LoadLevel(levelNumber+1);
                                         }
                                         catch (Exception e)
                                         {
                                             Debug.Fail(e.Message);
                                             
                                         }
                                    }
                                }
                            }
                        }
                    }

                    if(player != null)
                    {
                        if (player.IsCollidingWith(obj) && obj.HasTag(Tags.Goal))
                        {
                            //Load next level
                        }

                        if (player.IsCollidingWith(obj) && obj.HasTag(Tags.Dangerous))
                        {
                            Reference.GameState = GameState.GameOver;
                        }
                    }
                    
                }
            }
            
            foreach((MoveableObject, GameObject) objs in ResolveCollisions)
            {
                objs.Item1.ResolveCollision(objs.Item2, Reference.CurrentGravity);
            }
        }//end Update


        /// <summary>
        /// adds all objects in this level to the animation list, meaning they will be actively animated
        /// </summary>
        public void AddObjectsToAnimationList()
        {
            //TODO WITH MORE ANIMATIONS
        }


        public void ClearObjects()
        {
            levelObjects = new GameObject[width,height];
        }


    }
}
