using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessing
{
    static class MatrixOperationsHelper
    {
        public static int[][] NegateValues(this int[][] input)
        {
            int n = input.Length;
            var result = new int[n][];
            for (int i = 0; i < n; i++)
            {
                result[i] = new int[n];
                for (int j = 0; j < n; j++)
                {
                    result[i][j] = -input[i][j];
                }
            }
            return result;
        }

        public static int[][] CombineSubmatrix(int[][] topleft, int[][] topright, int[][] bottomleft, int[][] bottomright)
        {
            int ycenter = topleft.Length;
            int h = topleft.Length + bottomleft.Length;
            var result = new int[h][];
            for (int i = 0; i < h; i++)
            {
                int xcenter = topleft[0].Length;
                int w = topleft[0].Length + topright[0].Length;
                result[i] = new int[w];
                for (int j = 0; j < w; j++)
                {
                    result[i][j] = i < ycenter ?
                        (j < xcenter ? topleft[i][j] : topright[i][j - xcenter]) :
                        (j < xcenter ? bottomleft[i - ycenter][j] : bottomright[i - ycenter][j - xcenter]);
                }
            }
            return result;
        }

    }
}
