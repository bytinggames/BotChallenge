using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace JuliHelper
{
    public struct CollisionResult
    {
        // EXPERIMENTAL
        internal Vector2 colPos;

        public bool collision;
        public float? distance;
        public float? distanceReversed;
        public Vector2 axisCol, axisColReversed;
        internal int colCornerPoly;
        internal int colCornerIndex;

        //public bool inDistance
        //{
        //    get { return distance.HasValue; }
        //}

        //public CollisionResult()
        //{
        //    collision = false;
        //    distance = distanceReversed = null;
        //    nearestDir = axisCol = Vector2.Zero;
        //}

        //public void DistanceInvert()
        //{
        //    if (distance.HasValue)
        //        distance = -distance;
        //    if (distanceReversed.HasValue)
        //        distanceReversed = -distanceReversed;
        //}

        public void AxisInvert()
        {
            axisCol = -axisCol;
            axisColReversed = -axisColReversed;
        }

        //public CollisionResult GetDistanceInvert()
        //{
        //    DistanceInvert();
        //    return this;
        //}

        public CollisionResult GetAxisInvert()
        {
            AxisInvert();
            return this;
        }

        //public CollisionResult GetNormalized()
        //{
        //    if (distance.HasValue && distance.Value < 0)
        //    {
        //        distance = -distance;
        //        axisCol = -axisCol;
        //    }

        //    if (distanceReversed.HasValue && distanceReversed.Value < 0)
        //    {
        //        distanceReversed = -distanceReversed;
        //        axisColReversed = -axisColReversed;
        //    }
        //    return this;
        //}

        public override string ToString()
        {
            return "collision: " + collision
                 + "\ndistance: " + distance
                 + "\ndistanceReversed: " + distanceReversed
                 + "\naxisCol: " + axisCol
                 + "\naxisColReversed: " + axisColReversed;
        }

        public void SetCollisionFromDist()
        {
            collision = Math.Sign(distance.Value * distanceReversed.Value) == -1;
        }

        public bool MinResult(CollisionResult cr)
        {
            if (cr.collision)
                collision = true;
            
            if (!distanceReversed.HasValue || (cr.distanceReversed.HasValue && cr.distanceReversed < distanceReversed))
            {
                distanceReversed = cr.distanceReversed;
                axisColReversed = cr.axisColReversed;
            }

            if (!distance.HasValue || (cr.distance.HasValue && cr.distance < distance))
            {
                distance = cr.distance;
                axisCol = cr.axisCol;

                return true;
            }
            return false;
        }

        public void MaxResult(CollisionResult cr)
        {
            if (cr.collision)
                collision = true;

            if (!distance.HasValue || (cr.distance.HasValue && cr.distance > distance))
            {
                distance = cr.distance;
                axisCol = cr.axisCol;
            }

            if (!distanceReversed.HasValue || (cr.distanceReversed.HasValue && cr.distanceReversed > distanceReversed))
            {
                distanceReversed = cr.distanceReversed;
                axisColReversed = cr.axisColReversed;
            }
        }
        /// <summary>
        /// for testing
        /// </summary>
        /// <param name="cr"></param>
        public void MaxMinResult(CollisionResult cr)
        {
            if (!cr.collision)
                collision = false;

            if (!cr.distance.HasValue)
            {
                distance = null;
                axisCol = Vector2.Zero;
            }
            else if (!distance.HasValue || (cr.distance.HasValue && cr.distance > distance))
            {
                distance = cr.distance;
                axisCol = cr.axisCol;
            }

            if (!cr.distanceReversed.HasValue)
            {
                distanceReversed = null;
                axisColReversed = Vector2.Zero;
            }
            else if (!distanceReversed.HasValue || (cr.distanceReversed.HasValue && cr.distanceReversed < distanceReversed))
            {
                distanceReversed = cr.distanceReversed;
                axisColReversed = cr.axisColReversed;
            }
        }

        /// <summary>
        /// for testing
        /// dist min
        /// distRev max
        /// </summary>
        /// <param name="cr"></param>
        public void MinMaxResult(CollisionResult cr)
        {
            if (!cr.collision)
                collision = false;

            if (!cr.distance.HasValue)
            {
                distance = null;
                axisCol = Vector2.Zero;
            }
            else if (!distance.HasValue || (cr.distance.HasValue && cr.distance < distance))
            {
                distance = cr.distance;
                axisCol = cr.axisCol;
            }

            if (!cr.distanceReversed.HasValue)
            {
                distanceReversed = null;
                axisColReversed = Vector2.Zero;
            }
            else if (!distanceReversed.HasValue || (cr.distanceReversed.HasValue && cr.distanceReversed > distanceReversed))
            {
                distanceReversed = cr.distanceReversed;
                axisColReversed = cr.axisColReversed;
            }
        }


        public static float minDist = 0;

        /// <summary>
        /// testing
        /// </summary>
        public bool AddCollisionResult(CollisionResult cr, Vector2 move)
        {
            if (cr.distance.HasValue)
            {
                float pixelDist = cr.distance.Value * move.Length();

                //cr.distance -= 1f * move.Length();
                float dist = cr.distance.Value - 1f * move.Length();
                
                if (dist >= minDist || pixelDist >= -Collision.minDist)
                    return MinResult(cr);
            }
            return false;
		}
		/// <summary>
		/// testing
		/// </summary>
		public bool AddCollisionResult2(CollisionResult cr)
		{
			if(cr.distance.HasValue)
			{
				if(cr.distance.Value > 0f)
					return MinResult(cr);
			}
			return false;
		}

		public CollisionResult3D To3D(Vector3 xDir, Vector3 yDir)
        {
            CollisionResult3D cr = new CollisionResult3D();
            cr.collision = collision;
            cr.distance = distance;
            cr.distanceReversed = distanceReversed;
            cr.axisCol = xDir * axisCol.X + yDir * axisCol.Y;
            cr.axisColReversed = xDir * axisColReversed.X + yDir * axisColReversed.Y;
            return cr;
        }
    }
}
