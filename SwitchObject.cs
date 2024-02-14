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
    internal class SwitchObject : GameObject
    {
        // === Fields ===

        private Color color;
        // like SingleKeyPress() from its cousin the UI button, wasLastFrameColliding prevents
        // constant flickering from being inside the Bounds of the Switch
        private List<MoveableObject> listOfObjectsColliding;

        private int cooldownFrames;
        private const int cooldownFramesResetValue = 15;
        // === Properties ===

        /// <summary>
        /// Returns the switch's color
        /// </summary>
        public Color Color { get { return color; } }


        // === Constructor ===

        /// <summary>
        /// Creates a new SwitchObject
        /// Has the power to turn on/off the colored lasers that it is related to
        /// </summary>
        /// <param name="position">The x and y coordinates of the GameObject</param>
        /// <param name="texture">The sprite of the GameObject</param>
        public SwitchObject(Vector2 position, int width, int height, Texture2D texture, int offsetX, int offsetY, Color color)
            : base(position, width, height, texture, offsetX, offsetY)
        {
            this.color = color;
            listOfObjectsColliding = new List<MoveableObject>();
            
        }


        // === Methods ===

        /// <summary>
        /// Checks whether or not the current SwitchObject is colliding
        /// with another GameObject
        /// If this occurs, SwitchObject switches its isConnectionOn bool
        /// </summary>
        /// <param name="check">The other GameObject</param>
        /// <returns>True if the collision happened, false otherwise</returns>
        public void CheckCollision(MoveableObject check)
        {
            // like SingleKeyPress() from its cousin the UI button, wasLastFrameColliding prevents
            // constant flickering from being inside the Bounds of the Switch
            

            


            if (Bounds.Intersects(check.Bounds) && listOfObjectsColliding.Count == 0 && cooldownFrames == 0)
            {
                if (Reference.ColorDictionary[color])
                {
                    System.Diagnostics.Debug.WriteLine("Connection off");
                    Reference.ColorDictionary[color] = false;
                    Reference.callRedLaser(GravitySnail.Tags.Dangerous, false);
                }
                else
                {

                    System.Diagnostics.Debug.WriteLine("Connection on");
                    Reference.ColorDictionary[color] = true;
                    Reference.callRedLaser(GravitySnail.Tags.Dangerous, true);

                }

                // if the current object isn't already in the list, add it
                if (!listOfObjectsColliding.Contains(check))
                {
                    listOfObjectsColliding.Add(check);
                }
            }


            if (!Bounds.Intersects(check.Bounds))
            {
                // if the currrent object is in the list but no longer colliding, remove it
                if (listOfObjectsColliding.Contains(check))
                {
                    listOfObjectsColliding.Remove(check);
                }
            }

        }

        public override void Update(GameTime gameTime)
        {


            if (listOfObjectsColliding.Count != 0)
            {
                cooldownFrames = cooldownFramesResetValue;
            }
            else if (cooldownFrames > 0)
            {
                cooldownFrames--;
            }

            base.Update(gameTime);
        }

    }
}
