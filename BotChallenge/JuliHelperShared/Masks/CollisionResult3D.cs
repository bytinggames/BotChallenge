using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace JuliHelper
{
    public struct CollisionResult3D
    {
        public bool collision;
        public float? distance;
        public float? distanceReversed;
        public Vector3 axisCol, axisColReversed;

        public void AxisInvert()
        {
            axisCol = -axisCol;
            axisColReversed = -axisColReversed;
        }

        public CollisionResult3D GetAxisInvert()
        {
            AxisInvert();
            return this;
        }

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

        public void MinResult(CollisionResult3D cr)
        {
            if (cr.collision)
                collision = true;

            if (!distance.HasValue || (cr.distance.HasValue && cr.distance < distance))
            {
                distance = cr.distance;
                axisCol = cr.axisCol;
            }

            if (!distanceReversed.HasValue || (cr.distanceReversed.HasValue && cr.distanceReversed < distanceReversed))
            {
                distanceReversed = cr.distanceReversed;
                axisColReversed = cr.axisColReversed;
            }
        }

        public void MaxResult(CollisionResult3D cr)
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
        /// just for testing first (ray test)
        /// </summary>
        /// <param name="cr"></param>
        public void MaxMinResult(CollisionResult3D cr)
        {
            if (!cr.collision)
                collision = false;

            if (!cr.distance.HasValue)
            {
                distance = null;
                axisCol = Vector3.Zero;
            }
            else if (!distance.HasValue || (cr.distance.HasValue && cr.distance > distance))
            {
                distance = cr.distance;
                axisCol = cr.axisCol;
            }

            if (!cr.distanceReversed.HasValue)
            {
                distanceReversed = null;
                axisColReversed = Vector3.Zero;
            }
            else if (!distanceReversed.HasValue || (cr.distanceReversed.HasValue && cr.distanceReversed < distanceReversed))
            {
                distanceReversed = cr.distanceReversed;
                axisColReversed = cr.axisColReversed;
            }
        }
    }
}
