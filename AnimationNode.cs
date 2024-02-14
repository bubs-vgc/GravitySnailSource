using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GravitySnail
{
    public class AnimationNode<T>
    {
        /// <summary>
        /// The Data (often texture2D) of this animation node
        /// </summary>
        public T Data;


        /// <summary>
        /// The Next Node in the sequence
        /// </summary>
        public AnimationNode<T> NextNode;

        /// <summary>
        /// Creates an animation node that is already filled and linked
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nextNode"></param>
        public AnimationNode(T data, AnimationNode<T> nextNode)
        {
            Data = data;
            NextNode = nextNode;
        }

        /// <summary>
        /// Creates an empty animation node
        /// </summary>
        public AnimationNode()
        {

        }

    }
}
