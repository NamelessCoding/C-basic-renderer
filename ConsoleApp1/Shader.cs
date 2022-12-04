using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApp1
{
    class Shader
    {
        public int Handle;
        public Shader(string vpath, string fpath) {
            int fragShader = 0;
            int vertShader = 0;

            //read from the file, using the vertex shader path ("../shader.v") with encoding set as utf8
            //THE BEST HACK WRITTEN IN A NAPKIN

            string VertexShaderSource;

            using (StreamReader reader = new StreamReader(vpath, Encoding.UTF8))
            {
                VertexShaderSource = reader.ReadToEnd();
            }

            string FragmentShaderSource;

            using (StreamReader reader = new StreamReader(fpath, Encoding.UTF8))
            {
                FragmentShaderSource = reader.ReadToEnd();
            }

            //create the shader using GL.CreateShader, of type vertex shader
            vertShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertShader, VertexShaderSource);

            fragShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragShader, FragmentShaderSource);

            //compile shader and if there are any errors infoLogVert will contain the error, todo:Add extra information about the shader when outputting
            //an error
            GL.CompileShader(vertShader);

            string infoLogVert = GL.GetShaderInfoLog(vertShader);
            if (infoLogVert != System.String.Empty)
                System.Console.WriteLine(infoLogVert);

            GL.CompileShader(fragShader);

            string infoLogFrag = GL.GetShaderInfoLog(fragShader);

            if (infoLogFrag != System.String.Empty)
                System.Console.WriteLine(infoLogFrag);

            //Handle holds the integer pointing to the shader program that will run on the GPU
            //^ I was close, it's a pointer pointing towards the memory position of the shader program that cannot be dereferenced
            //dereferenced meaning that you can't change to what it's pointing to, or make it stop pointing towards the shader program
            Handle = GL.CreateProgram();
            //Attach the shaders to the handle
            GL.AttachShader(Handle, vertShader);
            GL.AttachShader(Handle, fragShader);
            //link 
            GL.LinkProgram(Handle);
            //detatch the individual shader programs, since they have already been compiled and also to delete them as to free them from memory
            GL.DetachShader(Handle, vertShader);
            GL.DetachShader(Handle, fragShader);
            GL.DeleteShader(fragShader);
            GL.DeleteShader(vertShader);


        }

        public void Run() {
            GL.UseProgram(Handle);
        }

        public void setMatrix4(Matrix4 matrix, string name) {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        public void setFloat(float num, string name)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform1(location, num);
        }
        public void setVector3(Vector3 vec, string name)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform3(location, vec.X, vec.Y, vec.Z);
        }

        public void setVector2(Vector2 vec, string name)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform2(location, vec.X, vec.Y);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~Shader()// for when the program is being stopped(exited)
        {
            GL.DeleteProgram(Handle);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SetInt(string name, int value)
        {
            int location = GL.GetUniformLocation(Handle, name);

            GL.Uniform1(location, value);
        }
    }
}
