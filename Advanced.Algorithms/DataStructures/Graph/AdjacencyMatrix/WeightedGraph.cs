﻿using System;
using System.Collections.Generic;
using Advanced.Algorithms.DataStructures.Graph.AdjacencyList;

namespace Advanced.Algorithms.DataStructures.Graph.AdjacencyMatrix
{

    //TODO implement IEnumerable & make sure duplicates are handled correctly if its not already
    /// <summary>
    /// A weighted graph implementation using dynamically growinng/shrinking adjacency matrix array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TW"></typeparam>
    public class WeightedGraph<T, TW> where TW : IComparable
    {
        public int VerticesCount => usedSize;

        private System.Collections.Generic.Dictionary<T, int> vertexIndices;
        private System.Collections.Generic.Dictionary<int, T> reverseVertexIndices;

        private TW[,] matrix;

        private int maxSize;
        private int usedSize;
        private int nextAvailableIndex;

        /// <summary>
        /// Constructor
        /// </summary>
        public WeightedGraph()
        {
            maxSize = 1;
            vertexIndices = new System.Collections.Generic.Dictionary<T, int>();
            reverseVertexIndices = new System.Collections.Generic.Dictionary<int, T>();
            matrix = new TW[maxSize, maxSize];
        }

      
        /// <summary>
        /// Add a new vertex to this graph
        /// O(1) complexity
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void AddVertex(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            if (vertexIndices.ContainsKey(value))
            {
                throw new Exception("Vertex exists.");
            }

            if (usedSize < maxSize / 2)
            {
                halfMatrixSize();
            }

            if (nextAvailableIndex == maxSize)
            {
                doubleMatrixSize();
            }

            vertexIndices.Add(value, nextAvailableIndex);
            reverseVertexIndices.Add(nextAvailableIndex, value);
            nextAvailableIndex++;
            usedSize++;
        }

        /// <summary>
        /// remove given vertex from this graph
        /// O(V) complexity
        /// </summary>
        /// <param name="value"></param>
        public void RemoveVertex(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            if (!vertexIndices.ContainsKey(value))
            {
                throw new Exception("Vertex does'nt exist.");
            }

            if (usedSize <= maxSize / 2)
            {
                halfMatrixSize();
            }

            var index = vertexIndices[value];

            //clear edges
            for (int i = 0; i < maxSize; i++)
            {
                matrix[i, index] = default(TW);
                matrix[index, i] = default(TW);
            }

            reverseVertexIndices.Remove(index);
            vertexIndices.Remove(value);


            usedSize--;
        }

        /// <summary>
        /// Add a new edge to this graph with given weight 
        /// and between given source and destination vertex
        /// O(1) complexity
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="weight"></param>
        public void AddEdge(T source, T dest, TW weight)
        {
            if (weight.Equals(default(TW)))
            {
                throw new Exception("Cannot add default edge weight.");
            }

            if (source == null || dest == null)
            {
                throw new ArgumentException();
            }

            if (!vertexIndices.ContainsKey(source) || !vertexIndices.ContainsKey(dest))
            {
                throw new Exception("Source or destination vertex does'nt exist.");
            }

            var sourceIndex = vertexIndices[source];
            var destIndex = vertexIndices[dest];
            if (!matrix[sourceIndex, destIndex].Equals(default(TW))
                && !matrix[destIndex, sourceIndex].Equals(default(TW)))
            {
                throw new Exception("Edge already exists.");
            }

            matrix[sourceIndex, destIndex] = weight;
            matrix[destIndex, sourceIndex] = weight;

        }

        /// <summary>
        /// Remove given edge
        /// O(1) complexity
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public void RemoveEdge(T source, T dest)
        {
            if (source == null || dest == null)
            {
                throw new ArgumentException();
            }

            if (!vertexIndices.ContainsKey(source) || !vertexIndices.ContainsKey(dest))
            {
                throw new Exception("Source or destination vertex does'nt exist.");
            }

            var sourceIndex = vertexIndices[source];
            var destIndex = vertexIndices[dest];
            if (matrix[sourceIndex, destIndex].Equals(default(TW))
                && matrix[destIndex, sourceIndex].Equals(default(TW)))
            {
                throw new Exception("Edge do not exists.");
            }

            matrix[sourceIndex, destIndex] = default(TW);
            matrix[destIndex, sourceIndex] = default(TW);
        }

        /// <summary>
        /// Do we have an edge between given source and destination
        /// O(1) complexity
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool HasEdge(T source, T dest)
        {
            if (source == null || dest == null)
            {
                throw new ArgumentException();
            }

            if (!vertexIndices.ContainsKey(source) || !vertexIndices.ContainsKey(dest))
            {
                throw new Exception("Source or destination vertex does'nt exist.");
            }

            var sourceIndex = vertexIndices[source];
            var destIndex = vertexIndices[dest];

            if (!matrix[sourceIndex, destIndex].Equals(default(TW))
                && !matrix[destIndex, sourceIndex].Equals(default(TW)))
            {
                return true;
            }

            return false;
        }

        public List<Tuple<T,TW>> GetAllEdges(T vertex)
        {
            if (!vertexIndices.ContainsKey(vertex))
            {
                throw new ArgumentException("vertex is not in this graph.");
            }

            var index = vertexIndices[vertex];

            var result = new List<Tuple<T, TW>>();

            for (int i = 0; i < maxSize; i++)
            {
                if (!matrix[i,index].Equals(default(TW)))
                {
                    result.Add(new Tuple<T, TW>(reverseVertexIndices[i], matrix[i, index]));
                }
            }

            return result;
        }

        private void doubleMatrixSize()
        {
            var newMatrix = new TW[maxSize * 2, maxSize * 2];

            var newVertexIndices = new System.Collections.Generic.Dictionary<T, int>();
            var newReverseIndices = new System.Collections.Generic.Dictionary<int, T>();

            int k = 0;
            foreach (var vertex in vertexIndices)
            {
                newVertexIndices.Add(vertex.Key, k);
                newReverseIndices.Add(k, vertex.Key);
                k++;
            }
            nextAvailableIndex = k;

            for (int i = 0; i < maxSize; i++)
            {

                for (int j = i; j < maxSize; j++)
                {
                    if (!matrix[i, j].Equals(default(TW)) && !matrix[j, i].Equals(default(TW))
                        && reverseVertexIndices.ContainsKey(i)
                        && reverseVertexIndices.ContainsKey(j))
                    {
                        var newI = newVertexIndices[reverseVertexIndices[i]];
                        var newJ = newVertexIndices[reverseVertexIndices[j]];

                        newMatrix[newI, newJ] = matrix[i, j];
                        newMatrix[newJ, newI] = matrix[j, i];
                    }
                }
            }

            matrix = newMatrix;
            vertexIndices = newVertexIndices;
            reverseVertexIndices = newReverseIndices;
            maxSize *= 2;
        }

        private void halfMatrixSize()
        {
            var newMatrix = new TW[maxSize * 2, maxSize * 2];

            var newVertexIndices = new System.Collections.Generic.Dictionary<T, int>();
            var newReverseIndices = new System.Collections.Generic.Dictionary<int, T>();

            int k = 0;
            foreach (var vertex in vertexIndices)
            {
                newVertexIndices.Add(vertex.Key, k);
                newReverseIndices.Add(k, vertex.Key);
                k++;
            }
            nextAvailableIndex = k;

            for (int i = 0; i < maxSize; i++)
            {
                for (int j = i; j < maxSize; j++)
                {
                    if (!matrix[i, j].Equals(default(TW)) && !matrix[j,i].Equals(default(TW))
                        && reverseVertexIndices.ContainsKey(i)
                        && reverseVertexIndices.ContainsKey(j))
                    {
                        var newI = newVertexIndices[reverseVertexIndices[i]];
                        var newJ = newVertexIndices[reverseVertexIndices[j]];

                        newMatrix[newI, newJ] = matrix[i, j];
                        newMatrix[newJ, newI] = matrix[j, i];
                    }
                }
            }

            matrix = newMatrix;
            vertexIndices = newVertexIndices;
            reverseVertexIndices = newReverseIndices;
            maxSize /= 2;
        }
    }
}
