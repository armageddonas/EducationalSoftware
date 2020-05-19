﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalSoftware
{
    public class RandomProbability
    {
        /// <summary>
        /// Random choice given some probabilities.
        /// </summary>
        /// <param name="probabilities"></param>
        /// <returns></returns>
        public static string Choose(List<(string, float)> probabilities)
        {
            probabilities.Sort((p, q) => p.Item2.CompareTo(q.Item2));
            double prob = new Random().NextDouble();
            int item = -1;
            double counter = 0;
            while (prob > counter)
            {
                item++;
                counter += probabilities[item].Item2;

            }
            return probabilities[item].Item1;


        }
    }

}