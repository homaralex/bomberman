﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bomberman.Utils
{
    /// <summary>
    /// Helper class for parsing value literals.
    /// For deserialization purposes, etc.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses Int Tuple.
        /// Example form: 2,2
        /// </summary>
        /// <param name="text">
        /// String to parse
        /// </param>
        /// <returns>
        /// Parsed int Tuple
        /// </returns>
        public static Tuple<int, int> ParseIntTuple(string text)
        {
            int elem1, elem2;

            if (text.Length < 3)
            {
                throw new BombermanException("Could not parse int Tuple - string too short. \n" + text);
            }

            string tmp = text.Substring(1, text.Length - 2);
            int commaPosition = tmp.IndexOf(',');
            if (commaPosition == -1)
            {
                throw new BombermanException("Could not parse int Tuple - string does not contain a comma between first and last position. \n" + text);
            }
            elem1 = Int32.Parse(tmp.Substring(0, commaPosition));
            elem2 = Int32.Parse(tmp.Substring(commaPosition + 1)); 

            return new Tuple<int, int>(elem1, elem2);
        }
        /// <summary>
        /// Parses Vector2 from string.
        /// Example: {X:2.131 Y: 123.123}
        /// </summary>
        /// <param name="text">
        /// String to parse
        /// </param>
        /// <returns>
        /// Parsed Vector2
        /// </returns>
        public static Vector2 ParseVector2(string text)
        {
            float x, y;

            int xStart = text.IndexOf("X:") + 2;
            int xLen = text.IndexOf("Y") - xStart - 1;
            int yStart = text.IndexOf("Y:") + 2;
            int yLen = text.IndexOf("}") - yStart;

            string xText = text.Substring(xStart, xLen);
            x = float.Parse(xText);
            string yText = text.Substring(yStart, yLen);
            y = float.Parse(yText);

            return new Vector2(x, y);
        }
    }
}
