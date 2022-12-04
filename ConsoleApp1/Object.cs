using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ConsoleApp1
{
    public class Object
    {
        private float[] vertices;
        private float[] texCoords;
        public uint[] indices;
        public uint[] indicest;
        public uint[] indicesn;

        public float[] normals;
        public float[] combined;

        public Object(string filename) {
            string[] lines = File.ReadAllLines(filename);
            var verts = new List<float>();
            var tex = new List<float>();
            var ind = new List<uint>();
            var indt = new List<uint>();
            var indn = new List<uint>();

            var comb = new List<float>();
            var test = new List<uint>();

            var norms = new List<float>();

            foreach (string s in lines) {
                string[] elements = s.Split(' ');
                switch (elements[0]) {
                    case "v":
                        verts.Add(float.Parse(elements[1], CultureInfo.InvariantCulture.NumberFormat));
                        verts.Add(float.Parse(elements[2], CultureInfo.InvariantCulture.NumberFormat));
                        verts.Add(float.Parse(elements[3], CultureInfo.InvariantCulture.NumberFormat));
                        break;

                    case "vt":
                        tex.Add(float.Parse(elements[1], CultureInfo.InvariantCulture.NumberFormat));
                        tex.Add(float.Parse(elements[2], CultureInfo.InvariantCulture.NumberFormat));
                        break;

                    case "vn":
                        norms.Add(float.Parse(elements[1], CultureInfo.InvariantCulture.NumberFormat));
                        norms.Add(float.Parse(elements[2], CultureInfo.InvariantCulture.NumberFormat));
                        norms.Add(float.Parse(elements[3], CultureInfo.InvariantCulture.NumberFormat));
                        break;

                    case "f":
                        string[] elem0 = elements[1].Split('/');
                        string[] elem1 = elements[2].Split('/');
                        string[] elem2 = elements[3].Split('/');
                        ind.Add(uint.Parse(elem0[0], CultureInfo.InvariantCulture.NumberFormat)-1);
                        indt.Add(uint.Parse(elem0[1], CultureInfo.InvariantCulture.NumberFormat)-1);
                        indn.Add(uint.Parse(elem0[2], CultureInfo.InvariantCulture.NumberFormat)-1);
                        

                        ind.Add(uint.Parse(elem1[0], CultureInfo.InvariantCulture.NumberFormat)-1);
                        indt.Add(uint.Parse(elem1[1], CultureInfo.InvariantCulture.NumberFormat)-1);
                        indn.Add(uint.Parse(elem1[2], CultureInfo.InvariantCulture.NumberFormat)-1);

                        ind.Add(uint.Parse(elem2[0], CultureInfo.InvariantCulture.NumberFormat)-1);
                        indt.Add(uint.Parse(elem2[1], CultureInfo.InvariantCulture.NumberFormat)-1);
                        indn.Add(uint.Parse(elem2[2], CultureInfo.InvariantCulture.NumberFormat)-1);
                        break;
                    
                }
            }
            vertices = verts.ToArray();
            texCoords = tex.ToArray();
            normals = norms.ToArray();
            indices = ind.ToArray();
            indicest = indt.ToArray();
            indicesn = indn.ToArray();


            for (int i = 0; i < indices.Length; i++) {
                comb.Add(vertices[indices[i] * 3]);
                comb.Add(vertices[indices[i] * 3 + 1]);
                comb.Add(vertices[indices[i] * 3 + 2]);
                
                
                comb.Add(texCoords[indicest[i] * 2]);
                comb.Add(texCoords[indicest[i] * 2 + 1]);

                comb.Add(normals[indicesn[i] * 3]);
                comb.Add(normals[indicesn[i] * 3 + 1]);
                comb.Add(normals[indicesn[i] * 3 + 2]);

                test.Add((uint)i);
                //if (i > vertices.Length-450) Console.WriteLine(indt[i]);
            
            }
            combined = comb.ToArray();
            indices = test.ToArray();

        }
    }
}
