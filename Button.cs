using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace GravitySnail
{
    /// <summary>
    /// Represents a button that can be clicked to trigger an event and changes image when hovered over
    /// </summary>
    internal class Button : GameObject
    {
        // === Fields ===
        protected Texture2D hoverTexture;
        public delegate void defaultClickDelegate();
        public event defaultClickDelegate OnDefaultClick;
        public delegate void intClickDelegate(int intArg);
        public event intClickDelegate OnIntClick;
        protected int intClickArgument;

        protected int buttonType;

        //Properties

        /// <summary>
        /// If this is an int button, the argument that will be passed
        /// </summary>
        public int IntClickArgument
        {
            get { return intClickArgument; }
            set { intClickArgument = value; }
        }


        public int ButtonType
        {
            get { return buttonType; }
            set { buttonType = value; }
        }


        // === Constructor ===
        public Button(Vector2 position, Texture2D texture, Texture2D hover) : base(position, texture)
        {
            hoverTexture = hover;
            buttonType = 0;
        }
        public Button(Vector2 position, Texture2D texture, Texture2D hover, int type) : base(position, texture)
        {
            hoverTexture = hover;
            buttonType = type;
        }

        // === Methods ===

        /// <summary>
        /// returns true if the mouse is over the button
        /// </summary>
        /// <returns></returns>
        protected bool MouseOver()
        {
            if (Reference.CurrentMouse.X >= position.X && Reference.CurrentMouse.X <= position.X + texture.Width
                && Reference.CurrentMouse.Y >= position.Y && Reference.CurrentMouse.Y <= position.Y + texture.Height)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// draws the hoverTexture if the mouse is over the button
        /// otherwise draws default texture
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="color"></param>
        public override void Draw(SpriteBatch sb, Color color)
        {
            if(MouseOver())
            {
                sb.Draw(
                hoverTexture,
                new Vector2(position.X - Reference.transformationMatrix.M41, position.Y - Reference.transformationMatrix.M42),
                color);
            }
            else
            {
                sb.Draw(
                texture,
                new Vector2(position.X - Reference.transformationMatrix.M41, position.Y - Reference.transformationMatrix.M42),
                color);
            }
        }

        /// <summary>
        /// checks to see if the button is being clicked and calls the event
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if(Reference.SingleClick() && MouseOver())
            {
                switch(buttonType)
                {
                    case 0:
                        OnDefaultClick();
                        break;

                    case 1:
                        OnIntClick(intClickArgument);
                        break;
                }
            }
        }
    }
}
