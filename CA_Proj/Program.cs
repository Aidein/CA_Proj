using System;
using System.Collections.Generic;

namespace CA_Project
{
    internal class Program
    {
        // struct value type data type for matrix node definition
        class Vertex
        {
            public string name = "";
            private List<Path>? adjV = new List<Path>();

            public Vertex()
            {
                name = "NA";
                adjV = new List<Path>();
            }

            // method to insert adjacent vertex
            public void addAdjV(Vertex? vertex, int cost)
            {
                Path path = new Path();
                path.cost = cost;
                path.destination = vertex;
                if (adjV != null)
                {
                    adjV.Add(path);
                }
            }
        }

        // struct value type data type for matrix node adj vertexes paths definition
        class Path
        {
            public int cost;
            public Vertex? destination;

            public Path()
            {
                cost = 0;
            }
        }

        // struct value type data type for route from one vertex to other verteces definition
        class Route
        {
            public bool isSet;
            public Vertex initPoint = new Vertex();
            private List<Path> route = new List<Path>();

            public void insertRoadPoint(Vertex? vertex, int cost, int index)
            {
                Path instance = new Path();
                instance.destination = vertex;
                instance.cost = cost;
                route.Insert(index, instance);
            }

            public void removeRoadPoint(Vertex vertex, int cost)
            {
                Path instance = new Path();
                instance.destination = vertex;
                instance.cost = cost;
                route.Remove(instance);
            }

            public int overallCost()
            {
                int cost = 0;
                foreach (Path path in route)
                {
                    cost += path.cost;
                }
                return cost;
            }
        }

        class Matrix // class for weight matrix
        {
            int size; // size of matrix
            List<Vertex> vertices = new List<Vertex>();
            Route[,] matrix; // declaration of two-dimensional matrix
            Random random = new Random(); // for randomizing
            public Matrix(int size = 5) // parametrized constructor. By default 5 vertices
            {
                matrix = new Route[size, size];
                Console.Write("Verteces created: ");
                // creates number of vertices
                for (int j = 65; j < size + 65; j++)
                {
                    Vertex instance = new Vertex();
                    instance.name = ((char)j).ToString();
                    vertices.Add(instance);
                    Console.Write(instance.name + " ");
                }
                Console.Write("\n");

                for (int j = 0; j < size; j++) // columns
                {
                    for (int k = 0; k < size; k++) // rows
                    {
                        matrix[j, k].isSet = false; // make diagonals all elements initially zero
                    }
                }
                this.size = size;
            }
            public void createPaths() // create rendomized array
            {
                int index;

                for (int j = 0; j < size; j++) // columns
                {
                    for (int k = 0; k < size; k++) // rows
                    {
                        index = vertices.FindIndex(x => x.name == ((char)(k + 65)).ToString());
                        if (j == k)
                        {
                            matrix[k, j].isSet = false; // make diagonals 0' = no loops allowed
                        }
                        else
                        {
                            matrix[k, j].insertRoadPoint(vertices.Find(x => x.name == ((char)(j + 65)).ToString()), random.Next(1, 100), 0); // assign values from 1 to 99 (function for roads)
                            vertices[index].addAdjV(vertices.Find(x => x.name == ((char)(j + 65)).ToString()), matrix[k, j].overallCost());
                        }
                    }
                }
            }

            public Route getElement(int j, int k)
            {
                return matrix[j, k]; //get element of matrix in j column, k row
            }
            public void displayArray() // display matrix
            {
                char letter = 'A'; // declare name of vertex
                for (int j = 0; j < size; j++) // for every column
                {
                    if (j == 0) // for the first row to be "A  B  C  D ...."
                    {
                        Console.Write("    ");
                        for (int l = 0; l < size; l++)
                        {
                            Console.Write(vertices[l].name + "   ");
                        }
                        Console.WriteLine("\n");
                        letter = 'A';
                    }
                    Console.Write(letter + "  "); // for letters in column
                    for (int k = 0; k < size; k++) // display matrix elements
                    {
                        Console.Write((matrix[j, k].overallCost() < 10 ? " " : "") + matrix[j, k] + "  "); // to display single digits 
                        // at two spaces/ EX: 0 would be " 0", 7 = " 7"...
                    }
                    Console.WriteLine("\n");
                    letter++;
                }
            }

            public void recount(int i, Matrix prev) // Floyd Warshall Algorithm
            {
                for (int j = 0; j < size; j++) // for every column
                {
                    for (int k = 0; k < size; k++) // in every row
                    {
                        if (j == i || k == i || j == k) // copy diagonal + column and row of vertex, through which
                                                        // algorithm is looking for better path
                        {
                            matrix[j, k] = prev.getElement(j, k);
                        }
                        else
                        {
                            if (prev.getElement(j, k).overallCost() != Math.Min(prev.getElement(j, k).overallCost(), matrix[j, i].overallCost() + matrix[i, k].overallCost()))
                            {
                                Console.WriteLine("Yeah, It is working probably)))");
                            }
                            // choose minimum between path through vertex and previos path
                        }
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            int size = 5; // number of vertex
            Matrix matrix = new Matrix(size); // create initial weight matrix
            Matrix[] matrices = new Matrix[size + 1]; // collection of matrixes for every vertex
            matrix.createPaths();
            matrix.displayArray();

            matrices[0] = matrix; // let matrices[0] be initial matrix
            for (int i = 1; i <= size; i++)
            {
                matrices[i] = matrices[i - 1]; // copy entire matrix
                matrices[i].recount(i - 1, matrices[i - 1]);
            }
            matrices[size].displayArray(); // display final array
        }
    }
}
