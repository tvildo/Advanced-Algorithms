﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Advanced.Algorithms.DataStructures.Graph.AdjacencyMatrix
{


    //TODO implement IEnumerable & make sure duplicates are handled correctly if its not already
    /// <summary>
    /// A directed graph implementation using dynamically growinng/shrinking adjacency matrix array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DiGraph<T>
    {
        public int VerticesCount => usedSize;

        private System.Collections.Generic.Dictionary<T, int> vertexIndices;
        private System.Collections.Generic.Dictionary<int, T> reverseVertexIndices;

        private BitArray[] matrix;

        private int maxSize;
        private int usedSize;
        private int nextAvailableIndex;

        public DiGraph()
        {
            maxSize = 1;
            vertexIndices = new System.Collections.Generic.Dictionary<T, int>();
            reverseVertexIndices = new System.Collections.Generic.Dictionary<int, T>();
            matrix = new BitArray[maxSize];

            for (var i = 0; i < maxSize; i++)
            {
                matrix[i] = new BitArray(maxSize);
            }
        }

        /// <summary>
        /// add a new vertex to this graph
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
        /// remove an existing vertex from graph
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
            for (var i = 0; i < maxSize; i++)
            {
                matrix[i].Set(index, false);
                matrix[index].Set(i, false);
            }

            reverseVertexIndices.Remove(index);
            vertexIndices.Remove(value);

            usedSize--;

        }

        /// <summary>
        /// add an edge from source to destination vertex
        /// O(1) complexity
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public void AddEdge(T source, T dest)
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
            if (matrix[sourceIndex].Get(destIndex))
            {
                throw new Exception("Edge already exists.");
            }

            matrix[sourceIndex].Set(destIndex, true);
        }

        /// <summary>
        /// remove an existing edge between source & destination
        ///  O(1) complexity
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
            if (!matrix[sourceIndex].Get(destIndex))
            {
                throw new Exception("Edge do not exists.");
            }

            matrix[sourceIndex].Set(destIndex, false);

        }

        /// <summary>
        /// do we have an edge between the given source and destination?
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

            return matrix[sourceIndex].Get(destIndex);
        }

        public List<T> GetAllOutEdges(T vertex)
        {
            if (!vertexIndices.ContainsKey(vertex))
            {
                throw new ArgumentException("vertex is not in this graph.");
            }

            var index = vertexIndices[vertex];

            var result = new List<T>();

            for (var i = 0; i < maxSize; i++)
            {
                if(matrix[index].Get(i))
                {
                    result.Add(reverseVertexIndices[i]);
                }
            }

            return result;
        }

        public List<T> GetAllInEdges(T vertex)
        {
            if (!vertexIndices.ContainsKey(vertex))
            {
                throw new ArgumentException("vertex is not in this graph.");
            }

            var index = vertexIndices[vertex];

            var result = new List<T>();

            for (var i = 0; i < maxSize; i++)
            {
                if (matrix[i].Get(index))
                {
                    result.Add(reverseVertexIndices[i]);
                }
            }

            return result;
        }

        private void doubleMatrixSize()
        {
            var newMatrix = new BitArray[maxSize * 2];
            for (var i = 0; i < maxSize * 2; i++)
            {
                newMatrix[i] = new BitArray(maxSize * 2);
            }

            var newVertexIndices = new System.Collections.Generic.Dictionary<T, int>();
            var newReverseIndices = new System.Collections.Generic.Dictionary<int, T>();

            var k = 0;
            foreach (var vertex in vertexIndices)
            {
                newVertexIndices.Add(vertex.Key, k);
                newReverseIndices.Add(k, vertex.Key);
                k++;
            }
            nextAvailableIndex = k;

            for (var i = 0; i < maxSize; i++)
            {
                newMatrix[i] = new BitArray(maxSize * 2);
                for (var j = 0; j < maxSize; j++)
                {
                    if (!matrix[i].Get(j) || !reverseVertexIndices.ContainsKey(i) ||
                        !reverseVertexIndices.ContainsKey(j))
                    {
                        continue;
                    }

                    var newI = newVertexIndices[reverseVertexIndices[i]];
                    var newJ = newVertexIndices[reverseVertexIndices[j]];

                    newMatrix[newI].Set(newJ, true);
                }
            }

            matrix = newMatrix;
            vertexIndices = newVertexIndices;
            reverseVertexIndices = newReverseIndices;
            maxSize *= 2;
        }

        private void halfMatrixSize()
        {
            var newMatrix = new BitArray[maxSize / 2];
            for (var i = 0; i < maxSize / 2; i++)
            {
                newMatrix[i] = new BitArray(maxSize / 2);
            }

            var newVertexIndices = new System.Collections.Generic.Dictionary<T, int>();
            var newReverseIndices = new System.Collections.Generic.Dictionary<int, T>();

            var k = 0;
            foreach (var vertex in vertexIndices)
            {
                newVertexIndices.Add(vertex.Key, k);
                newReverseIndices.Add(k, vertex.Key);
                k++;
            }
            nextAvailableIndex = k;

            for (var i = 0; i < maxSize; i++)
            {
                for (var j = 0; j < maxSize; j++)
                {
                    if (!matrix[i].Get(j) || !reverseVertexIndices.ContainsKey(i) ||
                        !reverseVertexIndices.ContainsKey(j))
                    {
                        continue;
                    }

                    var newI = newVertexIndices[reverseVertexIndices[i]];
                    var newJ = newVertexIndices[reverseVertexIndices[j]];

                    newMatrix[newI].Set(newJ, true);
                }
            }

            matrix = newMatrix;
            vertexIndices = newVertexIndices;
            reverseVertexIndices = newReverseIndices;
            maxSize /= 2;
        }
    }
}
