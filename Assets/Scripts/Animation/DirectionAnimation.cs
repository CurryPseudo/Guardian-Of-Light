using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DirectionAnimation
{
    public static string DirectionOfVelocity(Vector2 velocity)
    {
        if (velocity != Vector2.zero)
        {
            string[] dirAnimate = { "Back", "Right", "Front", "Left" };
            int dirIndex = 0;
            float max = velocity.y;
            if (Mathf.Abs(velocity.x) >= Mathf.Abs(velocity.y))
            {
                dirIndex++;
                max = velocity.x;
            }
            if (Mathf.Abs(max) < 0.1f)
            {
                return null;
            }
            if (max < 0)
            {
                dirIndex += 2;
            }
            return dirAnimate[dirIndex];
        }
        return null;
    }
}