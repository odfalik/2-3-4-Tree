using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace proj3 {
    public class Node234 {
        public List<int> Values { get; set; }
        public List<Node234> Children { get; set; }
        
        public Node234(int value) {
            Values = new List<int>();
            Children = new List<Node234>();
            Values.Add(value);
        }

        public bool HasSpace() { return Values.Count != 3; }
        public int GetDegree() { return Children.Count; }
        public bool IsLeaf() { return Children.Count == 0; }

        // Given a value to be inserted, return the child index to follow
        public int GetChildIndexToFollow(int val) {
            int numVals = Values.Count;

            if (GetDegree() == 0)
                return -1; // -1 means don't follow children; target belongs in current node

            int i;
            for (i = 0; i < Values.Count; i++) {
                if (val <= Values[i])
                    return i;
            }
            return i;
        }

        public int GetChildPredecessorIndex(int target) {
            return GetChildIndexToFollow(target - 1);
        }
        public int GetChildSuccessorIndex(int target) {
            return GetChildIndexToFollow(target + 1);
        }

        public void AddValue(int value) {
            if (Values.Contains(value))
                return;
            Values.Add(value);
            Values.Sort();
        }

        public void AddChild(Node234 child) {
            Debug.Assert(Children.Count <= Values.Count);
            Children.Add(child);
            Children = Children.OrderBy(c => c.Values[0]).ToList(); // not space efficient, but code-size efficient ;)
        }

        public void AddChildren(List<Node234> children) {
            foreach (Node234 child in children)
                Children.Add(child);
            Children = Children.OrderBy(c => c.Values[0]).ToList();
        }

        public int GetHeight() {
            int height = 0;

            foreach (Node234 child in Children) {
                int childHeight = child.GetHeight();
                if (childHeight > height) {
                    height = childHeight;
                }
            }

            return height + 1;
        }

        // Returns a string only denoting the values of the node
        public override string ToString() {
            string output = "[";

            for (int i = 0; i < Values.Count; i++) {
                int val = Values[i];
                string spaceAfter = "";

                if (i != Values.Count - 1) {
                    if (val < 10)
                        spaceAfter = "  ";
                    else
                        spaceAfter = " ";
                } else if (val < 10) {
                    spaceAfter = "";
                }

                output += val + spaceAfter;
            }

            return output + "] ";
        }

        // Pads node's ToString with spaces
        public string ToPaddedString() {
            int childrenStringWidth = 0;
            childrenStringWidth = Children.Sum(child => child.ToPaddedString().Length);

            string spacedString = ToString();

            if (childrenStringWidth != 0) {
                int spaces = childrenStringWidth - spacedString.Length;
                int padLeft = spaces / 2 + spacedString.Length;
                spacedString = spacedString.PadLeft(padLeft).PadRight(childrenStringWidth);
            }

            return spacedString;
        }
    }
}