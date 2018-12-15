using System;
using System.Collections.Generic;
using System.Linq;

namespace InsanityCubeSolver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Solver solver = new Solver(
                new Cube[]
                    {
                        new Cube(new []{'G', 'W', 'R', 'R', 'B', 'W' }),
                        new Cube(new []{'B', 'W', 'W', 'G', 'R', 'B' }),
                        new Cube(new []{'R', 'R', 'R', 'B', 'W', 'G' }),
                        new Cube(new []{'G', 'W', 'R', 'G', 'B', 'B' }),
                    });

            var watch = System.Diagnostics.Stopwatch.StartNew();

            (int, int)[] solution = solver.Run();

            watch.Stop();

            if (solution == null)
            {
                Console.WriteLine("Solution not found.");
            }
            else
            {
                Console.WriteLine("Solution found:");
                for (int i = 0; i < solution.Length; i++)
                {
                    Console.WriteLine($"Cube #{i + 1}: {solution[i].Item1} rotations to the right, {solution[i].Item2} flips to the back.");
                }
            }

            Console.WriteLine($"Program execution time: {watch.ElapsedMilliseconds} ms");

            Console.ReadKey();
        }
    }

    internal class Cube
    {
        // color of sides in format [front, right, back, left, bottom, top]
        private char[] originalSides;

        public Cube(char[] _sides)
        {
            if (_sides.Count() != 6)
            {
                throw new Exception("Cube must have six sides!");
            }

            originalSides = _sides;
        }

        /// <summary>
        /// Returns sides of cube after it has been rotated to the left for rotateSteps times and flipped to the back for flipSteps times
        /// </summary>
        /// <param name="rotateSteps"></param>
        /// <param name="flipSteps"></param>
        /// <returns></returns>
        public char[] Transform(int rotateSteps, int flipSteps)
        {
            if (rotateSteps > 3 || flipSteps > 3)
            {
                throw new Exception("Can't rotate or flip cube more than three times.");
            }

            List<char> transformedSides = new List<char>(originalSides);

            // rotate cube to the left for rotateSteps times
            if (rotateSteps > 0)
            {
                int endingIndex = 3;
                int startingIndex = endingIndex - rotateSteps + 1;
                int elementsToBeMoved = endingIndex - startingIndex + 1;

                char[] temp = transformedSides.Skip(endingIndex + 1 - elementsToBeMoved).Take(elementsToBeMoved).ToArray();
                transformedSides.RemoveRange(startingIndex, elementsToBeMoved);
                transformedSides.InsertRange(0, temp);
            }

            // flip cube to the back for flipSteps times
            if (flipSteps > 0)
            {
                for (int i = 0; i < flipSteps; i++)
                {
                    char[] rotatedSides = new char[]
                    {
                        // Bottom -> Front
                        transformedSides[4],
                        // Right -> Right
                        transformedSides[1],
                        // Top -> Back
                        transformedSides[5],
                        // Left -> Left
                        transformedSides[3],
                        // Back -> Bottom
                        transformedSides[2],
                        // Front -> Top
                        transformedSides[0]
                    };

                    transformedSides = rotatedSides.ToList();
                }
            }

            return transformedSides.ToArray();
        }
    }

    /// <summary>
    /// Finds a solution for the insanity cubes
    /// </summary>
    internal class Solver
    {
        private readonly int MAX_FLIPPING = 3;
        private readonly int MAX_ROTATIONS = 3;

        private Cube[] cubes;

        public Solver(Cube[] _cubes)
        {
            cubes = _cubes;
        }

        public (int, int)[] Run()
        {
            if (cubes.Count() != 4)
            {
                throw new Exception("There should be four cubes to find a solution.");
            }

            for (int cube1Rotation = 0; cube1Rotation <= MAX_ROTATIONS; cube1Rotation++)
            {
                for (int cube1Flipping = 0; cube1Flipping <= MAX_FLIPPING; cube1Flipping++)
                {
                    for (int cube2Rotation = 0; cube2Rotation <= MAX_ROTATIONS; cube2Rotation++)
                    {
                        for (int cube2Flipping = 0; cube2Flipping <= MAX_FLIPPING; cube2Flipping++)
                        {
                            for (int cube3Rotation = 0; cube3Rotation <= MAX_ROTATIONS; cube3Rotation++)
                            {
                                for (int cube3Flipping = 0; cube3Flipping <= MAX_FLIPPING; cube3Flipping++)
                                {
                                    for (int cube4Rotation = 0; cube4Rotation <= MAX_ROTATIONS; cube4Rotation++)
                                    {
                                        for (int cube4Flipping = 0; cube4Flipping <= MAX_FLIPPING; cube4Flipping++)
                                        {
                                            (int, int)[] testingParameters = new(int, int)[]{
                                                                                            (cube1Rotation, cube1Flipping),
                                                                                            (cube2Rotation, cube2Flipping),
                                                                                            (cube3Rotation, cube3Flipping),
                                                                                            (cube4Rotation, cube4Flipping)
                                                                                        };

                                            if (CheckSolution(testingParameters))
                                            {
                                                return testingParameters;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private bool CheckSolution((int, int)[] testingParameters)
        {
            Console.WriteLine("Testing: " + String.Join(", ", testingParameters.Select(x => $"({x.Item1}, {x.Item2})")));
            List<char[]> cubeColors = new List<char[]>();

            for (int i = 0; i < testingParameters.Length; i++)
            {
                cubeColors.Add(cubes[i].Transform(testingParameters[i].Item1, testingParameters[i].Item2));
            }

            for (int i = 0; i < 4; i++)
            {
                if (cubeColors.Select(x => x[i]).Distinct().Count() < 4)
                {
                    return false;
                }
            }

            return true;
        }
    }
}