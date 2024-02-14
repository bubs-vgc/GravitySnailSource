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
    /// A child of MoveableObject, this object has its position restricted by clamp values in a certain direction
    /// </summary>
    internal class ClampedMoveableObject : MoveableObject
    {
        private Vector2 positionMin;
        private Vector2 positionMax;

        // === Constructor ===

        /// <summary>
        /// Creates a new ClampedMoveableObject
        /// </summary>
        /// <param name="directionToMove">The direction the platform goes in, use chars 'x' and 'y'</param>
        /// <param name="minPosition">Its minimum position in the chosen direction</param>
        /// <param name="maxPosition">Its maximum position in the chosen direction</param>
        /// <exception cref="ArgumentException"></exception>
        public ClampedMoveableObject(Vector2 position, int width, int height, Texture2D texture, int offsetX, int offsetY, Vector2 minPosition, Vector2 maxPosition)
            : base(position, width, height, texture, offsetX, offsetY)
        {
            this.position = position;
            positionMin = minPosition;
            positionMax = maxPosition;
        }



        // === Methods ===

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);



            position = Vector2.Clamp(position, positionMin, positionMax);

            if (position == positionMin || position == positionMax)
            {
                velocity = Vector2.Zero;
            }

            Debug.WriteLine($"{Bounds.X} {position.X} {texture.Height} {position.X - Bounds.X} {(width)/Reference.TileSize}");

            //System.Diagnostics.Debug.WriteLine("Current position x: " + position.X + "    Current position y: " + position.Y);
        }

        


    }
}
