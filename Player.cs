using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GravitySnail
{
   
    /// <summary>
    /// Creates the Player, a Moveable object that has the ability to change the direction of
    /// gravity for every other Moveable object in the game, including itself
    /// </summary>
    internal class Player : MoveableObject
    {
        //======FIELDS========
        private SpriteEffects movementDirection = SpriteEffects.None;

        private int movementAcceleration=150;
        

        private Vector2 spawnPosition;

        private const int ContactFramesRefill = 10;



        //======PROPERTIES=========
        /// <summary>
        /// How Many Frames it's been since the player made contact
        /// </summary>
        public int ContactFrames;

        public bool MadeContact
        { get { return ContactFrames >= 0; } }


        //=====CONSTRUCTORS======

        public Vector2 SpawnPosition { get { return spawnPosition; } set { spawnPosition = value; } } 


        //Creates a player with default values
        public Player(Texture2D texture) : this(texture, Vector2.Zero, Reference.BaseMaxSpeed)
        {

        }
        /// <summary>
        /// Creates a player with the given texture and starting position, but the default maxSpeed
        /// </summary>
        public Player(Texture2D texture, Vector2 startingPosition) : this(texture, startingPosition, Reference.BaseMaxSpeed)
        {

        }
        /// <summary>
        /// Creates a player with the given texture, starting position, and maxSpeed
        /// </summary>
        public Player(Texture2D texture, Vector2 startingPosition, float maxSpeed) : base(startingPosition, texture, maxSpeed)
        {
           
        }

        //=========PROPERTIES===========

        public override Rectangle Bounds
        {
            get
            {
                if (Reference.CurrentGravity == Facing.Up || Reference.CurrentGravity == Facing.Down)
                {
                    return new Rectangle((int)position.X - (width / 2), (int)position.Y - (height / 2), width, height);
                }
                else
                {
                    return new Rectangle((int)position.X - (height / 2), (int)position.Y - (width / 2), height, width);
                }
            }
        }




        //============METHODS===========


        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.Milliseconds/(float)1000;

            

            //Update direction
            UpdateGravityDirection();


            //Apply forces to the player
            ApplyGravity(deltaTime);
            ApplyDrag(deltaTime);
            CheckMovement(deltaTime);

            //Apply movement
            ApplyVelocity(deltaTime);


            
            EndOfFrame();
        }

        /// <summary>
        /// The Player's EndOfFrame method that runs MoveableObject's EndOfFrame plus player specific updates
        /// </summary>
        public override void EndOfFrame()
        {
            if(ContactFrames > 0)
            {
                ContactFrames--;
            }
            base.EndOfFrame();
        }



        public override void Draw(SpriteBatch sb, Color color)
        {
            float rotation = 0;






            switch (Reference.CurrentGravity)
            { 

                case Facing.Down:
                    rotation = 0;
                    break;

                case Facing.Left:
                    rotation = MathF.PI * .5f;
                    break;

                case Facing.Up:
                    rotation = MathF.PI;
                    break;

                case Facing.Right:
                    rotation = MathF.PI * 1.5f;
                    break;
            }


            sb.Draw(texture, new Vector2((int)position.X, (int)position.Y), null, color, rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1, movementDirection, 0);
        }






        /// <summary>
        /// Checks for if the user is pressing wasd and updates the direction of gravity accordingly
        /// </summary>
        private void UpdateGravityDirection()
        {
            if(Reference.KBState.IsKeyDown(Keys.W))
            {
                Reference.CurrentGravity = Facing.Up;
            } 
            else if(Reference.KBState.IsKeyDown(Keys.S))
            {
                Reference.CurrentGravity = Facing.Down;
            }
            else if(Reference.KBState.IsKeyDown(Keys.A))
            {
                Reference.CurrentGravity = Facing.Left;
            }
            else if(Reference.KBState.IsKeyDown(Keys.D))
            {
                Reference.CurrentGravity= Facing.Right;
            }
        }

        /// <summary>
        /// checks to see if the left or right arrow keys are held down and moves the player
        /// movement is relative to the current direction of gravity
        /// </summary>
        private void CheckMovement(float deltaTime)
        {
            if(Reference.KBState.IsKeyDown(Keys.Right))
            {
                if (Reference.CurrentGravity != Facing.Up)
                {
                    movementDirection = SpriteEffects.None;
                }
                else
                {
                    movementDirection = SpriteEffects.FlipHorizontally;
                }
                switch (Reference.CurrentGravity)
                {

                    case Facing.Left:
                        velocity.Y += movementAcceleration * deltaTime;
                        break;

                    case Facing.Right:
                        velocity.Y -= movementAcceleration * deltaTime;
                        break;
                    default:
                        {
                            velocity.X+= movementAcceleration * deltaTime;
                            break;
                        }
                }
            }
            if(Reference.KBState.IsKeyDown(Keys.Left))
            {
                if(Reference.CurrentGravity!=Facing.Up)
                {
                    movementDirection = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    movementDirection = SpriteEffects.None;
                }
                switch (Reference.CurrentGravity)
                {

                    case Facing.Left:
                        velocity.Y -= movementAcceleration * deltaTime;
                        break;

                    case Facing.Right:
                        velocity.Y += movementAcceleration * deltaTime;
                        break;
                    default:
                        {
                            velocity.X -= movementAcceleration * deltaTime;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// resets players values and moves them to their spawn position
        /// </summary>
        public void SpawnPlayer()
        {
            velocity = Vector2.Zero;
            position = spawnPosition;
            Reference.CurrentGravity = Facing.Down;
        }


        public override void ApplyDrag(float deltaTime)
        {
            if(doDrag)
            {
                if (MadeContact)
                {
                    //Check to make sure we AREN'T accelerating on that axis
                    if (Math.Abs(velocity.X) <= Math.Abs(previousVelocity.X))
                    {
                        
                        velocity.X *= MathF.Pow(Reference.GroundDragFactor, deltaTime);
                    }
                    if (Math.Abs(velocity.Y) <= Math.Abs(previousVelocity.Y))
                    {
                        velocity.Y *= MathF.Pow(Reference.GroundDragFactor, deltaTime);
                    }
                }
                else
                {
                    base.ApplyDrag(deltaTime);
                }
            }
        }




        /// <summary>
        /// Resolves if the player character is colliding with another object
        /// </summary>
        /// <returns>True if their bounds collide, false otherwise</returns>
        public bool IsCollidingWith(GameObject other)
        {
            if (this.Bounds.Intersects(other.Bounds))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resets the Contact Frames of the Player
        /// </summary>
        public void ContactFramesReset()
        {
            ContactFrames = ContactFramesRefill;
        }

    }
}
