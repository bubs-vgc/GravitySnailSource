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
    /// GameObject handles the basic fields and methods 
    /// that every game entity will need to have.
    /// </summary>
    internal class GameObject
    {
        // === Fields ===
        protected int width;
        protected int height;
        protected int offsetX;
        protected int offsetY;
        protected Vector2 drawOffset;
        protected Vector2 position;
        protected Texture2D texture;
        protected List<Tags> tags;

        protected AnimationNode<Texture2D> animationTextures;

        protected Facing direction;



        // === Properties ===

        public Vector2 Offset
        {
            get { return new Vector2(offsetX, offsetY); }

        }

        /// <summary>
        /// Gets the position of the GameObject
        /// </summary>
        public Vector2 Position { get { return position; } }

        /// <summary>
        /// Gets the bounds of the GameObject
        /// </summary>
        virtual public Rectangle Bounds { 
            get {
                //default values are for direction DOWN
                int X = (int)MathF.Round(position.X) ;
                int Y = (int)MathF.Round(position.Y) ;

                Vector2 rotatedPosition = new Vector2(X - texture.Width / 2, Y - texture.Height / 2);
                

                int rectX;
                int rectY;
                int textX;
                int textY;

                switch(direction)
                {
                    

                    case Facing.Up:
                        
                        rectX = width;
                        rectY = height;
                        textX = texture.Width;
                        textY = texture.Height;
                        break;

                    case Facing.Right:
                        rotatedPosition += Vector2.Transform(new Vector2(offsetX, offsetY), GetRotationMatrix());
                        rectX = height;
                        rectY = width;
                        textX = texture.Height;
                        textY = texture.Width;
                        break;

                    case Facing.Left:
                        
                        rectX = height;
                        rectY = width;
                        textX = texture.Height;
                        textY = texture.Width;
                        break;

                    default:
                        rotatedPosition += Vector2.Transform(new Vector2(offsetX, offsetY), GetRotationMatrix());
                        rectX = width;
                        rectY = height;
                        textX = texture.Width;
                        textY = texture.Height;
                        break;
                }

                Tuple<int, int> finalCalculatedPosition = null;
                if (Offset != Vector2.Zero)
                {

                    finalCalculatedPosition = new Tuple<int, int>((int)(rotatedPosition.X), (int)(rotatedPosition.Y));

                }
                else
                {
                    finalCalculatedPosition = new Tuple<int, int>((int)(position.X-textX/2), (int)(position.Y-textY/2));
                }
                


                return new Rectangle(finalCalculatedPosition.Item1, finalCalculatedPosition.Item2, rectX, rectY);


            } 
        }

        /// <summary>
        /// Gets the texture of the GameObject
        /// </summary>
        public Texture2D Texture { get; }

        /// <summary>
        /// The Tag for the GameObject telling the game what type of Game Object this qualifies as
        /// </summary>
        public List<Tags> Tags { get { return tags; } }

        /// <summary>
        /// The Animation Node loop that this GameObject is set to
        /// </summary>
        public AnimationNode<Texture2D> AnimationTextures
        {
            get { return animationTextures; }
            set { animationTextures = value; }
        }

        /// <summary>
        /// The Direction the Game Object is currently facing
        /// </summary>
        public Facing Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        /// <summary>
        /// An amount to change the draw position by. **subtracted**, so positive values move more towards the topleft, negative towards bottomright
        /// </summary>
        public Vector2 DrawOffset
        {
            get { return drawOffset; }
            set { drawOffset = value; }
        }

        // === Constructor ===


        /// <summary>
        /// Creates a new GameObject, a base for all game entities, where the bounds are the bounds of the texture
        /// </summary>
        /// <param name="position">The x and y coordinates of the GameObject</param>
        /// <param name="texture">The sprite of the GameObject</param>
        public GameObject(Vector2 position, Texture2D texture) : this (position, texture.Width, texture.Height, texture)
        {

        }

        /// <summary>
        /// Creates a new GameObject, a base for all game entities, at 0,0 where the bounds are the bounds of the texture
        /// </summary>
        /// <param name="texture"></param>
        public GameObject(Texture2D texture) : this(Vector2.Zero, texture)
        {
            
        }

        /// <summary>
        /// Creates a Game Object with given position, width, height, & texture
        /// </summary>
        /// <param name="position"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="texture"></param>
        public GameObject (Vector2 position, int width, int height, Texture2D texture) : this (position, width, height, texture, 0, 0)
        {

        }

        public GameObject(Vector2 position, int width, int height, Texture2D texture, int offsetX, int offsetY) : this(position, width, height, texture, offsetX, offsetY, Facing.Down)
        {

        }

        /// <summary>
        /// Creates a new GameObject, a base for all game entities, at the position passed, with the passed width and height + offset for bounding box
        /// </summary>
        public GameObject(Vector2 position, int width, int height, Texture2D texture, int offsetX, int offsetY, Facing direction)
        {
            this.position = position;

            this.width = width;
            this.height = height;

            this.offsetX = offsetX;
            this.offsetY = offsetY;

            this.texture = texture;

            //A Game Object has no Tags to begin with
            tags = new List<Tags>();

            this.direction = direction;
        }


        // === Methods ===

        /// <summary>
        /// The update logic for the GameObject
        /// Overriden in child classes to provide specific behavior
        /// </summary>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// Draws the object in the current facing
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="color"></param>
        /// <param name="rotation"></param>
        public virtual void Draw(SpriteBatch sb, Color color)
        {
            float rotation = 0;
            switch (direction)
            {
                case Facing.Down:
                    break;

                case Facing.Up:
                    rotation = MathF.PI;
                    break;

                case Facing.Left:
                    rotation = MathF.PI * .5f;
                    break;

                case Facing.Right:
                    rotation = MathF.PI * 1.5f;
                    break;
            }
            sb.Draw(texture, new Vector2((int)position.X  - DrawOffset.X, (int)position.Y  - DrawOffset.Y), null, color, rotation, new Vector2(texture.Width / 2 , texture.Height / 2), 1, SpriteEffects.None, 0);
        }

        /// <summary>
        /// returns true if this game object's tag list contains the given tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasTag(Tags tag)
        {
            return tags.Contains(tag);
        }

        public bool SetTagTo(Tags tag, bool value)
        {
            if(value && !tags.Contains(tag))
            {
                tags.Add(tag);
                return true;
            } 
            else if (!value && tags.Contains(tag))
            {
                tags.Remove(tag);
                return true;
            }

            return false;
        }



        /// <summary>
        /// Sets the texture of this gameObject to the next texture in it's AnimationTextures 
        /// and advances the object's AnimationTextures. 
        /// Call to animate Object
        /// </summary>
        public void AdvanceFrame()
        {
            animationTextures = animationTextures.NextNode;
            texture = animationTextures.Data;
        }


        public Matrix GetRotationMatrix()
        {
            Matrix rotation = 
                ( direction == Facing.Down ? Reference.rotationMatrix0 : 
                ( direction == Facing.Left ? Reference.rotationMatrix90: 
                ( direction == Facing.Up ? Reference.rotationMatrix180 :
                Reference.rotationMatrix270)));



            return //Matrix.CreateTranslation(-(position.X), -(position.Y), 0f)
                rotation;
                //* Matrix.CreateTranslation(position.X, position.Y, 0f);
        }


    }
}
