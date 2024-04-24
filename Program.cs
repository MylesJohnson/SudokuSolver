using Google.OrTools.Sat;

namespace SudokuSolver
{
    internal static class Program
    {
        private static T[] GetColumn<T>(this T[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        private static T[] GetRow<T>(this T[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }

        private static void PrintPuzzle(CpSolver solver, IntVar[,] grid)
        {
            for (int i = 0; i < 9; i++)
            {                
                if (i != 0 && i % 3 == 0)
                {
                    // Print a horizontal line every 3 rows
                    Console.WriteLine(new string('-', 21));
                }

                for (int j = 0; j < 9; j++)
                {
                    if (j != 0 && j % 3 == 0)
                    {
                        // Print a vertical line every 3 columns
                        Console.Write("| ");
                    }
                    Console.Write($"{solver.Value(grid[i, j])} ");
                }
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            int[,] puzzle =
                {
                    { 0, 0, 6, 5, 1, 0, 0, 0, 8 },
                    { 7, 0, 3, 8, 0, 0, 6, 9, 1 },
                    { 2, 0, 0, 0, 3, 0, 5, 4, 7 },
                    { 0, 0, 1, 7, 0, 0, 0, 8, 0 },
                    { 6, 0, 0, 3, 0, 1, 7, 5, 0 },
                    { 8, 0, 7, 4, 5, 0, 3, 1, 0 },
                    { 0, 8, 5, 6, 0, 0, 9, 0, 0 },
                    { 9, 0, 0, 0, 0, 5, 0, 7, 0 },
                    { 0, 7, 4, 0, 0, 0, 0, 0, 0 }
                };

            CpModel model = new CpModel();
            IntVar[,] grid = new IntVar[9, 9];

            // Load the puzzle into the model
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (puzzle[i, j] == 0)
                    {
                        grid[i, j] = model.NewIntVar(1, 9, $"cell_{i}_{j}");
                    }
                    else
                    {
                        grid[i, j] = model.NewConstant(puzzle[i, j]);
                    }
                }
            }

            // Add rows and columns must be unique constraints
            for (int i = 0; i < 9; i++)
            {
                model.AddAllDifferent(grid.GetRow(i));
                model.AddAllDifferent(grid.GetColumn(i));
            }

            // Add 3x3 square must be unique constraints
            for (int i = 0; i < 9; i += 3)
            {
                for (int j = 0; j < 9; j += 3)
                {
                    IntVar[] square = new IntVar[9];
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            square[k * 3 + l] = grid[i + k, j + l];
                        }
                    }
                    model.AddAllDifferent(square);
                }
            }

            CpSolver solver = new CpSolver();
            CpSolverStatus status = solver.Solve(model);

            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                PrintPuzzle(solver, grid);
            }
            else
            {
                Console.WriteLine("No solution found.");
            }
        }
    }
}
