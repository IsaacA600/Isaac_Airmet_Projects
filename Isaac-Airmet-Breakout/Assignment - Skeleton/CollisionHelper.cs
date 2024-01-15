using Microsoft.Xna.Framework;
using System;

// Code adapted from https://community.monogame.net/t/trouble-with-solid-collision-response/17008/2
namespace CS5410
{
    /// <summary>An enumeration of the possible sides at which 2D collision occurred.</summary>
    [Flags]
    public enum CollisionSide
    {
        /// <summary>No collision occurred.</summary>
        None = 0,
        /// <summary>Collision occurred at the top side.</summary>
        Top = 1,
        /// <summary>Collision occurred at the bottom side.</summary>
        Bottom = 2,
        /// <summary>Collision occurred at the left side.</summary>
        Left = 4,
        /// <summary>Collision occurred at the right side.</summary>
        Right = 8
    }

    /// <summary>A collection of helper methods for 2D collision detection and response.</summary>
    public static class CollisionHelperAABB
    {
        /// <summary>Calculates which side of a stationary object 
        /// a moving object has collided with.</summary>
        /// <param name="movingObjectPreviousHitbox">The moving object's previous hitbox,
        /// from the frame prior to when collision occurred.</param>
        /// <param name="stationaryObjectHitbox">The stationary object's hitbox.</param>
        /// <param name="movingObjectVelocity">The moving object's velocity 
        /// during the frame in which the collision occurred.</param>
        /// <returns>The side of the stationary object the moving object has collided with.</returns>
        public static CollisionSide GetCollisionSide(
            HitBox movingObjectPreviousHitbox,
            HitBox stationaryObjectHitbox,
            Vector2 movingObjectVelocity)
        {
            double cornerSlopeRise = 0;
            double cornerSlopeRun = 0;

            double velocitySlope = movingObjectVelocity.Y / movingObjectVelocity.X;

            //Stores what sides might have been collided with
            CollisionSide potentialCollisionSide = CollisionSide.None;

            if (movingObjectPreviousHitbox.right <= stationaryObjectHitbox.left)
            {
                //Did not collide with right side; might have collided with left side
                potentialCollisionSide |= CollisionSide.Left;

                cornerSlopeRun = stationaryObjectHitbox.left - movingObjectPreviousHitbox.right;

                if (movingObjectPreviousHitbox.bottom <= stationaryObjectHitbox.top)
                {
                    //Might have collided with top side
                    potentialCollisionSide |= CollisionSide.Top;
                    cornerSlopeRise = stationaryObjectHitbox.top - movingObjectPreviousHitbox.bottom;
                }
                else if (movingObjectPreviousHitbox.top >= stationaryObjectHitbox.bottom)
                {
                    //Might have collided with bottom side
                    potentialCollisionSide |= CollisionSide.Bottom;
                    cornerSlopeRise = stationaryObjectHitbox.bottom - movingObjectPreviousHitbox.top;
                }
                else
                {
                    //Did not collide with top side or bottom side or right side
                    return CollisionSide.Left;
                }
            }
            else if (movingObjectPreviousHitbox.left >= stationaryObjectHitbox.right)
            {
                //Did not collide with left side; might have collided with right side
                potentialCollisionSide |= CollisionSide.Right;

                cornerSlopeRun = movingObjectPreviousHitbox.left - stationaryObjectHitbox.right;

                if (movingObjectPreviousHitbox.bottom <= stationaryObjectHitbox.top)
                {
                    //Might have collided with top side
                    potentialCollisionSide |= CollisionSide.Top;
                    cornerSlopeRise = movingObjectPreviousHitbox.bottom - stationaryObjectHitbox.top;
                }
                else if (movingObjectPreviousHitbox.top >= stationaryObjectHitbox.bottom)
                {
                    //Might have collided with bottom side
                    potentialCollisionSide |= CollisionSide.Bottom;
                    cornerSlopeRise = movingObjectPreviousHitbox.top - stationaryObjectHitbox.bottom;
                }
                else
                {
                    //Did not collide with top side or bottom side or left side;
                    return CollisionSide.Right;
                }
            }
            else
            {
                //Did not collide with either left or right side; 
                //must be top side, bottom side, or none
                if (movingObjectPreviousHitbox.bottom <= stationaryObjectHitbox.top)
                    return CollisionSide.Top;
                else if (movingObjectPreviousHitbox.top >= stationaryObjectHitbox.bottom)
                    return CollisionSide.Bottom;
                else
                    //Previous hitbox of moving object was already colliding with stationary object
                    return CollisionSide.None;
            }

            //Corner case; might have collided with more than one side
            //Compare slopes to see which side was collided with
            return GetCollisionSideFromSlopeComparison(potentialCollisionSide,
                velocitySlope, cornerSlopeRise / cornerSlopeRun);
        }

        /// <summary>Gets which side of a stationary object was collided with by a moving object
        /// by comparing the slope of the moving object's velocity and the slope of the velocity 
        /// that would have caused the moving object to be touching corners with the stationary
        /// object.</summary>
        /// <param name="potentialSides">The potential two sides that the moving object might have
        /// collided with.</param>
        /// <param name="velocitySlope">The slope of the moving object's velocity.</param>
        /// <param name="nearestCornerSlope">The slope of the path from the closest corner of the
        /// moving object's previous hitbox to the closest corner of the stationary object's
        /// hitbox.</param>
        /// <returns>The side of the stationary object with which the moving object collided.
        /// </returns>
        static CollisionSide GetCollisionSideFromSlopeComparison(
            CollisionSide potentialSides, double velocitySlope, double nearestCornerSlope)
        {
            if ((potentialSides & CollisionSide.Top) == CollisionSide.Top)
            {
                if ((potentialSides & CollisionSide.Left) == CollisionSide.Left)
                    return velocitySlope < nearestCornerSlope ?
                        CollisionSide.Top : CollisionSide.Left;
                else if ((potentialSides & CollisionSide.Right) == CollisionSide.Right)
                    return velocitySlope > nearestCornerSlope ?
                        CollisionSide.Top : CollisionSide.Right;
            }
            else if ((potentialSides & CollisionSide.Bottom) == CollisionSide.Bottom)
            {
                if ((potentialSides & CollisionSide.Left) == CollisionSide.Left)
                    return velocitySlope > nearestCornerSlope ?
                        CollisionSide.Bottom : CollisionSide.Left;
                else if ((potentialSides & CollisionSide.Right) == CollisionSide.Right)
                    return velocitySlope < nearestCornerSlope ?
                        CollisionSide.Bottom : CollisionSide.Right;
            }
            return CollisionSide.None;
        }
    }
}
