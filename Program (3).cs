using System;
using System.Linq;

class Program
{
    static (int[][], int) NorthwestCornerMethod(int[] supply, int[] demand, int[][] costs)
    {
        int[][] allocation = new int[supply.Length][];
        for (int i = 0; i < supply.Length; i++)
        {
            allocation[i] = new int[demand.Length];
        }

        int[] supplyCopy = supply.ToArray();
        int[] demandCopy = demand.ToArray();
        int totalCost = 0;

        int row = 0, col = 0;
        while (row < supply.Length && col < demand.Length)
        {
            int x = Math.Min(supplyCopy[row], demandCopy[col]);
            allocation[row][col] = x;
            supplyCopy[row] -= x;
            demandCopy[col] -= x;
            totalCost += x * costs[row][col];

            if (supplyCopy[row] == 0)
            {
                row++;
            }
            else
            {
                col++;
            }
        }

        return (allocation, totalCost);
    }

    static (int[][], int) MinimumCostMethod(int[] supply, int[] demand, int[][] costs)
    {
        int[][] allocation = new int[supply.Length][];
        for (int i = 0; i < supply.Length; i++)
        {
            allocation[i] = new int[demand.Length];
        }

        int[] supplyCopy = supply.ToArray();
        int[] demandCopy = demand.ToArray();
        int totalCost = 0;

        while (true)
        {
            int minCost = int.MaxValue;
            int minRow = -1, minCol = -1;

            for (int row = 0; row < supply.Length; row++)
            {
                for (int col = 0; col < demand.Length; col++)
                {
                    if (supplyCopy[row] > 0 && demandCopy[col] > 0)
                    {
                        if (costs[row][col] < minCost)
                        {
                            minCost = costs[row][col];
                            minRow = row;
                            minCol = col;
                        }
                    }
                }
            }

            if (minRow == -1 || minCol == -1)
            {
                break;
            }

            int x = Math.Min(supplyCopy[minRow], demandCopy[minCol]);
            allocation[minRow][minCol] = x;
            supplyCopy[minRow] -= x;
            demandCopy[minCol] -= x;
            totalCost += x * minCost;
        }

        return (allocation, totalCost);
    }

    static (int[][], int) PotentialsMethod(int[] supply, int[] demand, int[][] costs)
    {
        int n = supply.Length;
        int m = demand.Length;

        int[][] allocation = new int[n][];
        for (int i = 0; i < n; i++)
        {
            allocation[i] = new int[m];
        }

        int[] u = new int[n];
        int[] v = new int[m];

        u[0] = 0;

        while (true)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (allocation[i][j] == 0)
                    {
                        continue;
                    }

                    if (u[i] + v[j] > costs[i][j])
                    {
                        u[i] = costs[i][j] - v[j];
                    }
                    else if (u[i] + v[j] < costs[i][j])
                    {
                        v[j] = costs[i][j] - u[i];
                    }
                }
            }

            int[][] delta = new int[n][];
            for (int i = 0; i < n; i++)
            {
                delta[i] = new int[m];
                for (int j = 0; j < m; j++)
                {
                    delta[i][j] = costs[i][j] - u[i] - v[j];
                }
            }

            int maxDelta = delta.SelectMany(row => row).Max();
            if (maxDelta <= 0)
            {
                break;
            }

            var maxIndices = Enumerable.Range(0, n)
                .SelectMany(i => Enumerable.Range(0, m).Select(j => (i, j)))
                .Where(idx => delta[idx.i][idx.j] == maxDelta)
                .OrderByDescending(idx => supply[idx.i] * demand[idx.j])
                .First();

            int maxRow = maxIndices.i;
            int maxCol = maxIndices.j;

            int minSupplyDemand = Math.Min(supply[maxRow], demand[maxCol]);
            allocation[maxRow][maxCol] = minSupplyDemand;
            supply[maxRow] -= minSupplyDemand;
            demand[maxCol] -= minSupplyDemand;
        }

        int totalCost = allocation.SelectMany((row, i) => row.Select((x, j) => x * costs[i][j])).Sum();
        return (allocation, totalCost);
    }

    static void Main()
    {
        // Исходные данные
        int[] supply = { 200, 250, 200 };
        int[] demand = { 190, 100, 120, 110, 130 };
        int[][] costs = new int[][]
        {
            new int[] { 28, 27, 18, 27, 24 },
            new int[] { 18, 26, 27, 32, 21 },
            new int[] { 27, 33, 23, 31, 34 }
        };

        // Опорный план северо-западного угла
        var (allocationNW, totalCostNW) = NorthwestCornerMethod(supply, demand, costs);

        // Опорный план методом минимальных элементов
        var (allocationMinCost, totalCostMinCost) = MinimumCostMethod(supply, demand, costs);

        // Оптимальный план методом потенциалов
        var (allocationPotentials, totalCostPotentials) = PotentialsMethod(supply, demand, costs);

        // Сравнение результатов и вывод наилучшего плана
        //if (totalCostNW <= totalCostMinCost && totalCostNW <= totalCostPotentials)
        //{
            Console.WriteLine("Наилучший план (северо-западный угол):");
            Console.WriteLine("Allocation:");
            foreach (var row in allocationNW)
            {
                Console.WriteLine(string.Join(", ", row));
            }
            Console.WriteLine("Total Cost: " + totalCostNW);
        //}
        //else if (totalCostMinCost <= totalCostNW && totalCostMinCost <= totalCostPotentials)
        //{
            Console.WriteLine("Наилучший план (минимальные элементы):");
            Console.WriteLine("Allocation:");
            foreach (var row in allocationMinCost)
            {
                Console.WriteLine(string.Join(", ", row));
            }
            Console.WriteLine("Total Cost: " + totalCostMinCost);
        //}
        //else
        //{
            Console.WriteLine("Наилучший план (метод потенциалов):");
            Console.WriteLine("Allocation:");
            foreach (var row in allocationPotentials)
            {
                Console.WriteLine(string.Join(", ", row));
            }
            Console.WriteLine("Total Cost: " + totalCostPotentials);
        //}
    }
}

