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
    /// Any GameObject that needs movement
    /// </summary>
    internal class MoveableObject : GameObject
    {
        // === Fields ===

        protected Vector2 velocity;
        protected Vector2 previousVelocity;
        protected float maxSpeed;
        protected bool doGravity;
        protected bool doDrag;
        protected Facing direction;


        // === Properties ===

        /// <summary>
        /// The Velocity that this moveable object currently has
        /// </summary>
        public Vector2 Velocity { get { return velocity; } }
        /// <summary>
        /// The Max Speed this movable object can move
        /// </summary>
        public float MaxSpeed { get { return maxSpeed; } }
        /// <summary>
        /// If this movable object responds to gravity or not
        /// </summary>
        public bool DoGravity { 
            get { return doGravity; } 
            set { doGravity = value; }
        }
        /// <summary>
        /// If this movable object applies drag every frame
        /// </summary>
        public bool DoDrag
        {
            get { return doDrag; }
            set { doDrag = value; }
        }


        //==============Constructor=================

        /// <summary>
        /// Creates an MoveableObject, a GameObject that can be moved with a velocity, 
        /// with the given position, texture, maxSpeed, and initial facing
        /// </summary>
        public MoveableObject(Vector2 position, int width, int height, Texture2D texture, int offsetX, int offsetY, float maxSpeed, Facing direction)
            : base(position, width, height, texture, offsetX, offsetY)
        {
            velocity = Vector2.Zero;
            this.maxSpeed = maxSpeed;
            previousVelocity = Vector2.Zero;
            doDrag = true;
            doGravity = true;
            this.direction = direction;
        }


        public MoveableObject(Vector2 position, Texture2D texture, float maxSpeed, Facing direction) : this(position, texture.Width, texture.Height, texture, 0, 0, maxSpeed, direction)
        {

        }

        public MoveableObject(Vector2 position, int width, int height, Texture2D texture, int offsetX, int offsetY) : this(position, width, height, texture, offsetX, offsetY, Reference.BaseMaxSpeed, Facing.Down)
        {

        }


        /// <summary>
        /// Creates an MoveableObject, a GameObject that can be moved with a velocity, 
        /// with the given position, texture, and maxSpeed
        /// </summary>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="texture"></param>
        /// <param name="maxSpeed"></param>
        public MoveableObject(Vector2 position, Texture2D texture, float maxSpeed) : this(position, texture, maxSpeed, Facing.Down)
        {

        }

        /// <summary>
        /// Creates an MoveableObject, a GameObject that can be moved with a velocity, 
        /// with the given position and texture, but the base Max Speed
        /// </summary>
        public MoveableObject(Vector2 position, Texture2D texture) : this(position, texture, Reference.BaseMaxSpeed)
        {

        }
        /// <summary>
        /// Creates an MoveableObject, a GameObject that can be moved with a velocity, 
        /// with the given texture, but at 0,0 and with the base Max Speed
        /// </summary>
        /// <param name="texture"></param>
        public MoveableObject(Texture2D texture) : this(Vector2.Zero, texture, Reference.BaseMaxSpeed)
        {

        }




        // === Methods ===

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.Milliseconds / (float)1000;



            //Apply forces to the player
            ApplyGravity(deltaTime);
            ApplyDrag(deltaTime);

            //Apply movement
            ApplyVelocity(deltaTime);




            EndOfFrame();
        }

        /// <summary>
        /// applies drag relative to the time passed if doDrag is true for this gameobject
        /// </summary>
        public virtual void ApplyDrag(float deltaTime)
        {
            if (doDrag == false)
            {
                return;
            }

            

            //Check to make sure we AREN'T accelerating on that axis
            if (Math.Abs(velocity.X) <= Math.Abs(previousVelocity.X))
            {
                velocity.X *= MathF.Pow(Reference.DragFactor, deltaTime);
            }
            if (Math.Abs(velocity.Y) <= Math.Abs(previousVelocity.Y))
            {
                velocity.Y *= MathF.Pow(Reference.DragFactor, deltaTime);
            }
            
        }

        /// <summary>
        /// End of frame cleanup method
        /// </summary>
        public virtual void EndOfFrame()
        {
            previousVelocity = velocity;
        }



        /// <summary>
		/// Applies gravity to the object based on the current gravity direction and strength
		/// </summary>
		public void ApplyGravity(float deltaTime)
        {
            //if time hasn't passed between frames, IGNORE this function. for Frame1 divide by 0 issues
            //if doGravity is false, we don't want to apply any gravity to this object
            if (doGravity == false || deltaTime == 0)
            {
                return;
            }

            Vector2 gravityVector = Vector2.Zero;



            switch(Reference.CurrentGravity)
            {
                case Facing.Up:
                    gravityVector = new Vector2(0, -1) * Reference.GravityStrength;
                    break;

                case Facing.Down:
                    gravityVector = new Vector2(0, 1) * Reference.GravityStrength;
                    break;

                case Facing.Left:
                    gravityVector = new Vector2(-1, 0) * Reference.GravityStrength;
                    break;

                case Facing.Right:
                    gravityVector = new Vector2(1, 0) * Reference.GravityStrength;
                    break;
            }

            
            
            velocity += gravityVector * deltaTime;
        }

        /// <summary>
        /// applies the player's current velocity to the position of the player, adjusting for maxSpeed
        /// </summary>
        public void ApplyVelocity(float deltaTime)
        {
            //if time hasn't passed between frames, IGNORE this function. for Frame1 divide by 0 issues
            if (deltaTime == 0)
            {
                return;
            }

            //adjust for maxSpeed
            if (velocity.Length() > maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
            }

            //apply velocity
            position += velocity * deltaTime;
        }


        /// <summary>
        /// Checks whether or not the current GameObject is colliding
        /// with another GameObject
        /// </summary>
        /// <param name="check">The other GameObject</param>
        /// <returns>True if intersects,false otherwise</returns>
        public virtual bool CheckCollision(GameObject check)
        {
            return Bounds.Intersects(check.Bounds);
        }

        // ======================================
        // NOTE: Check and Resolve Collisions can
        // be later combined if necessary
        // ======================================

        public virtual void ResolveCollision(GameObject otherObject, Facing gravity)
        {

            // if gravity is up or down, horizontal collisions must be done first
            if (gravity == Facing.Up ||
                gravity == Facing.Down)
            {
                ResolveHoriztonalCollisions(otherObject, gravity);
                ResolveVerticalCollisions(otherObject, gravity);
            }
            // else, vertical collisions must be done first
            else
            {
                ResolveVerticalCollisions(otherObject, gravity);
                ResolveHoriztonalCollisions(otherObject, gravity);
            }

            

        }

        private void ResolveHoriztonalCollisions(GameObject otherObject, Facing gravity)
        {
            Rectangle overlap = Rectangle.Intersect(this.Bounds, otherObject.Bounds);

            if (overlap.Width > 0 || overlap.Height > 0)
            {
                // dumb little breakpoint to see if we're overlapping
            }

            //horizontal collisions
            if (overlap.Height > overlap.Width)
            {
                if(this is Player)
                {
                    (this as Player).ContactFramesReset();
                }

                this.velocity.X = 0.0f;
                // this means the player is approaching from the left
                if (this.Bounds.Left < otherObject.Bounds.Left)
                {
                    this.position.X -= overlap.Width;
                    this.position.X = MathF.Ceiling(this.position.X);
                }
                // else, the player is approaching from the right
                else
                {
                    this.position.X += overlap.Width;
                    this.position.X = MathF.Floor(this.position.X);
                }
            }
        }

        private void ResolveVerticalCollisions(GameObject otherObject, Facing gravity)
        {
            Rectangle overlap = Rectangle.Intersect(this.Bounds, otherObject.Bounds);

            // vertical collisions
            if (overlap.Width > overlap.Height)
            {
                if (this is Player)
                {
                    (this as Player).ContactFramesReset();
                }

                this.velocity.Y = 0.0f;
                // this means the player is approaching from the Bottom
                if (this.Bounds.Top > otherObject.Bounds.Top)
                {
                    this.position.Y += overlap.Height;
                    this.position.Y = MathF.Floor(this.position.Y);
                }
                // else, the player is approaching from the Top
                else
                {

                    this.position.Y -= overlap.Height;
                    this.position.Y = MathF.Ceiling(this.position.Y);                        
                }
            }
        }
    }
}
