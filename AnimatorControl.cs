using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GravitySnail
{
    /// <summary>
    /// The Animatior Control Item is a thing that holds all of the animations for various objects in the game
    /// It has a Dictionary, keyed with the name of specific objects, that holds a linked list-type thing of Textures that make up a given animation
    /// we will update Level to hold a list of Linked List-type things (I guess they'll wind up being closer to graphs? we'll make the tail loop to head)
    /// that it will iterate on each frame.
    /// 
    /// Could also make a GameObject hold the list and then call a method in each GameObject that advances it? depends on what seems better tbh
    /// I'm leaning toward GameObject implementation but still.
    /// 
    /// Might make this class static eventually? or just have a Static Reference of it in Reference
    /// </summary>
    internal class AnimatorControl
    {
        private List<GameObject> animationList;


        public Dictionary<string, AnimationNode<Texture2D>> AnimationDictionary;

        
        /// <summary>
        /// Creates a new Animator Control module with an empty dictionary
        /// </summary>
        public AnimatorControl()
        {
            AnimationDictionary = new Dictionary<string, AnimationNode<Texture2D>>();
            animationList = new List<GameObject>();
        }

        /// <summary>
        /// Adds the given frame to the animation specified, creates an entry if it does not already exist.
        /// </summary>
        /// <param name="animationName">The animation to add to or create</param>
        /// <param name="frame">the frame to add to the animation</param>
        /// <returns>false if a new animation is created, true if the frame is added to an animation that already exists</returns>
        public bool AddFrameToAnimation(string animationName, Texture2D frame)
        {
            bool foundAnimation = AnimationDictionary.ContainsKey(animationName);


            //if do not find the animation, just add a new Node to the Dictionary with the given key
            if(!foundAnimation)
            {
                //Create node
                AnimationDictionary[animationName] = new AnimationNode<Texture2D>();
                //Add Data
                AnimationDictionary[animationName].Data = frame;
                //Link node to self (head) for looping
                AnimationDictionary[animationName].NextNode = AnimationDictionary[animationName];
            } else
            {
                //Create node with the frame and already linked to the head node
                AnimationNode<Texture2D> newNode = new AnimationNode<Texture2D>(frame, AnimationDictionary[animationName]);
                
                //Find the last node in the list (it will be the one linked to the first node) starting at the head node
                AnimationNode<Texture2D> lastNode = AnimationDictionary[animationName];
                while(lastNode.NextNode != AnimationDictionary[animationName])
                {
                    lastNode = lastNode.NextNode;
                }

                //relink the last node to the new node (add to list)
                lastNode.NextNode = newNode;
            }

            //return what we found
            return foundAnimation;
        } //end AddFrameToAnimation

        /// <summary>
        /// Clears the list of objects to animate
        /// </summary>
        public void ClearAnimationList()
        {
            animationList.Clear();
        }

        /// <summary>
        /// Add an object to the animation list. If we are already animating, return false and do nothing. otherwise, add it and return true
        /// </summary>
        /// <param name="objectToAdd"></param>
        /// <returns></returns>
        public bool AddObjectToAnimationList(GameObject objectToAdd)
        {
            //if the object is already being animated, just don't add it.
            if(animationList.Contains(objectToAdd)) 
            {
                return false; 
            }

            animationList.Add(objectToAdd);

            return true;
        }

        /// <summary>
        /// If the gameObject is in AnimationList, removes it and returns true. Returns false otherwise
        /// </summary>
        /// <param name="objectToRemove"></param>
        /// <returns></returns>
        public bool RemoveObjectFromAnimationList(GameObject objectToRemove)
        {
            if(animationList.Contains(objectToRemove))
            {
                animationList.Remove(objectToRemove);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Advances all animated objects frame by 1
        /// </summary>
        public void AnimateAllObjects()
        {
            foreach (GameObject obj in animationList)
            {
                obj.AdvanceFrame();
            }
        }



    }
}
