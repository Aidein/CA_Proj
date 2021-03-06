using System;
using System.Collections.Generic;

namespace CA_Project
{                                                
    internal class Program
    {
        // class for vertex
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

        // class for paths
        class Path
        {
            public int cost;
            public Vertex? destination;

            public Path()
            {
                cost = 0;
            }
        }
        
        // class for route
        class Route
        {
            public bool isSet;
            public List<Path> route = new List<Path>();

            public Route()
            {
                isSet = false;
            }

            public void insertRoadPoint(Vertex? vertex, int cost, int index)
            {
                Path instance = new Path();
                instance.destination = vertex;
                instance.cost = cost;
                route.Insert(index, instance);
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
            

            public Matrix(int size = 5) // parametrized constructor. By default 5 vertices
            {
                this.size = size;
                matrix = new Route[size, size];
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        matrix[i, j] = new Route();
                    }
                }
                Console.Write("Vertexes created: ");
                // creates number of vertices
                for (int j = 65; j < size + 65; j++)
                {
                    Vertex instance = new Vertex();
                    instance.name = ((char)j).ToString();
                    vertices.Add(instance);
                    Console.Write(instance.name + " ");
                }
                Console.Write("\n\n");
            }
            public int trafficUpdate() 
            {
                Random random = new Random(); // for randomizing
                return random.Next(1,100);
            }
            public void createPaths() // create randomized array
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
                            matrix[k, j].insertRoadPoint(vertices.Find(x => x.name == ((char)(j + 65)).ToString()), 
                            trafficUpdate(), 0); // assign values from 1 to 99 (function for roads)
                            vertices[index].addAdjV(vertices.Find(x => x.name == ((char)(j + 65)).ToString()), 
                            matrix[k, j].overallCost());
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
                        Console.Write((matrix[j, k].overallCost() < 10 ? " " : "") + matrix[j, k].overallCost() + "  "); // to display single digits 
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
                            if (prev.getElement(j, k).overallCost() != Math.Min(prev.getElement(j, k).overallCost(), prev.getElement(j, i).overallCost() + prev.getElement(i, k).overallCost()))
                            {
                                //matrix[j, k].route.RemoveAt(matrix[j, k].route.Count() - 1);
                                matrix[j, k].route.Clear();
                                foreach (Path instance in prev.getElement(j, i).route)
                                {
                                    matrix[j, k].route.Add(instance);
                                }
                                foreach (Path instance in prev.getElement(i, k).route)
                                {
                                    matrix[j, k].route.Add(instance);
                                }
                            }
                            else
                            {
                                matrix[j, k] = prev.getElement(j, k);
                            }
                            // choose minimum between path through vertex and previos path
                        }
                    }
                }
            }

            // displays route
            public string getTrip(string init, string dest)
            {
                int i, d;
                char initChar = init.ToCharArray()[0];
                char destChar = dest.ToCharArray()[0];
                i = (int)initChar - 65;
                d = (int)destChar - 65;
                string route = "Route: " + init.ToString();
                foreach (Path instance in matrix[i, d].route)
                {
                    route += " --> " + instance.destination.name + " (cost: " + instance.cost + 
                        " )";
                }
                route += " OVERALL: " + matrix[i, d].overallCost().ToString();
                return route;
            }
        }

        static void Main(string[] args)
        {
            int size; // number of vertex
            string? s; // to check the null
            Console.Write("Please enter amount of vertices (1-27): ");
            while((s = Console.ReadLine()) == null || int.Parse(s) < 1 || int.Parse(s) > 27) // Checks if user entered wrong amount or null
            {
                Console.WriteLine("Null Exception. Please re-enter the digit (1-27)");
            }
            size = int.Parse(s);
            Matrix matrix = new Matrix(size); // create initial weight matrix
            Matrix[] matrices = new Matrix[size + 1]; // collection of matrixes for every vertex
            matrix.createPaths();
            matrix.displayArray();

            Console.WriteLine("Optimizing ..........\n");

            matrices[0] = matrix; // let matrices[0] be initial matrix
            for (int i = 1; i <= size; i++)
            {
                matrices[i] = matrices[i - 1]; // copy entire matrix
                matrices[i].recount(i - 1, matrices[i - 1]);
            }
            matrices[size].displayArray(); // display final array

            // asks to choose initial point and destination
            Console.Write("Please enter the initial and destination points.\nInitial point: ");
            string? init;
            init = Console.ReadLine();
            Console.Write("Destination point: ");
            string? dest;
            dest = Console.ReadLine();
            Console.WriteLine(matrix.getTrip(init, dest));
        }
    }
}