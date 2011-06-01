using System;
using Microsoft.Xna.Framework;

namespace Bomber
{
    /// <summary>
    /// Defines method for component that could be destoried.
    /// 
    /// IDestroyable component may be destroyed by some internal or external impuls
    /// For exmaple barrel is destoryed by a bomb, but bomb is destoryed itself after
    /// defined period of time elapsed.
    /// 
    /// When Destroy() on a component is called, it becomes "destroing". At this time
    /// an animation could be played. After "DestroingTime" has been elapsed, it becomes
    /// "destroyed" and Remove() is called. This method removes the component from the game
    /// </summary>
    interface IDestroyable
    {
        /// <summary>
        /// (Get) Time when destroy on this component was called
        /// </summary>
        double DestroyTime { get; }

        /// <summary>
        /// (Get) How long this component will be destroing. After this time status of this component
        /// should change to "destroyed"
        /// </summary>
        double DestroingTime { get; }

        /// <summary>
        /// Initialize variables for manipulating an destoyable component
        /// </summary>
        void DestroyInitialize();

        /// <summary>
        /// Destoroy the component. In this method set destroing to true and initialize an animation
        /// Return true if destroy was succesfull, otherwise false (usually if it is already destroing)
        /// </summary>
        bool Destroy(GameTime gameTime);

        /// <summary>
        /// This method shall be called when component is "destroyed" and may be removed from the game
        /// </summary>
        void Remove();

        /// <summary>
        /// (Get) True when component is being destroing. An animation may be played
        /// </summary>
        bool Destroing
        {
            get;
        }
        /// <summary>
        /// (Get) True when component is destroyed (must not be updated or drawn)
        /// </summary>
        bool Destroyed
        {
            get;
        }
    }
}
