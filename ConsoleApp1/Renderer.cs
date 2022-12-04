using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

//Bind buffer simply means that we're selecting a buffer in the buffer array, so that any calls
//done to the bufferarray will be 

//GL.BUfferData basically means that we're copying some data into the buffer
//StaticDraw: the data will most likely not change at all or very rarely.
//DynamicDraw: the data is likely to change a lot.
//StreamDraw: the data will change every time it is drawn
namespace ConsoleApp1
{
    public class Renderer : GameWindow
    {
        Shader shader;
        Shader sunshader;
        Shader skyshader;
        //just until I learn how to use OpenTK
        Stopwatch stopWatch;

        int skyboxVBO;
        int skyboxVAO;
        float[] skyboxVertices = {
    // positions          
    -40.0f,  40.0f, -40.0f,
    -40.0f, -40.0f, -40.0f,
     40.0f, -40.0f, -40.0f,
     40.0f, -40.0f, -40.0f,
     40.0f,  40.0f, -40.0f,
    -40.0f,  40.0f, -40.0f,

    -40.0f, -40.0f,  40.0f,
    -40.0f, -40.0f, -40.0f,
    -40.0f,  40.0f, -40.0f,
    -40.0f,  40.0f, -40.0f,
    -40.0f,  40.0f,  40.0f,
    -40.0f, -40.0f,  40.0f,

     40.0f, -40.0f, -40.0f,
     40.0f, -40.0f,  40.0f,
     40.0f,  40.0f,  40.0f,
     40.0f,  40.0f,  40.0f,
     40.0f,  40.0f, -40.0f,
     40.0f, -40.0f, -40.0f,

    -40.0f, -40.0f,  40.0f,
    -40.0f,  40.0f,  40.0f,
     40.0f,  40.0f,  40.0f,
     40.0f,  40.0f,  40.0f,
     40.0f, -40.0f,  40.0f,
    -40.0f, -40.0f,  40.0f,

    -40.0f,  40.0f, -40.0f,
     40.0f,  40.0f, -40.0f,
     40.0f,  40.0f,  40.0f,
     40.0f,  40.0f,  40.0f,
    -40.0f,  40.0f,  40.0f,
    -40.0f,  40.0f, -40.0f,

    -40.0f, -40.0f, -40.0f,
    -40.0f, -40.0f,  40.0f,
     40.0f, -40.0f, -40.0f,
     40.0f, -40.0f, -40.0f,
    -40.0f, -40.0f,  40.0f,
     40.0f, -40.0f,  40.0f
};
        int quadVBO;
        int quadVAO;
        int quadEBO;

        float[] quad =
  {
    //Position          Texture coordinates
     1.0f,  1.0f, 0.0f, 1.0f, 1.0f, // top right
     1.0f, -1.0f, 0.0f, 1.0f, 0.0f, // bottom right
    -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, // bottom left
    -1.0f,  1.0f, 0.0f, 0.0f, 1.0f  // top left
 };

        uint[] quadindices = {  // note that we start from 0!
    0, 1, 3,   // first triangle
    1, 2, 3    // second triangle
};
        //Repeat: The default behavior for textures.Repeats the texture image.
        //MirroredRepeat: Same as GL_REPEAT but mirrors the image with each repeat.
        //ClampToEdge: Clamps the coordinates between 0 and 1. The result is that higher
        //coordinates become clamped to the edge, resulting in a stretched edge pattern.
        //ClampToBorder: Coordinates outside the range are now given a user-specified border color.

        //VBO, used to store a large number of vertices that can then be sent to the GPU in a single batch
        int VertexBufferObject;
       // int VBOnormals;

        //Vertex array object
        int VertexArrayObject;

        int VAOsun;

        Texture texture;
        Texture texture2;
        Texture texture3;

        Object classroom;

        Vector2 lastPos;

        Vector2 yawpitch;

        Vector3 position = new Vector3(0.0f, 0.0f, 3.0f);
        Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

        int EBO;
        int depthmap;

        int colorbuff;
        int colortex;
        int colordepthtex;
        int colornormtex;
        int colorpostex;
        int colorrbo;
        Shader tonemappingshader;
        public Renderer(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //simple keyboard input(todo: learn how to send keyboard input to shader)
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape)) {
                Exit();
            }

            if (input.IsKeyDown(Key.W))
            {
                position += front * 2.4f * (float)e.Time ;
            }
            if (input.IsKeyDown(Key.S))
            {
                position -= front * 2.4f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.D))
            {
                position += Vector3.Normalize(Vector3.Cross(front,up)) * 2.4f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.A))
            {
                position -= Vector3.Normalize(Vector3.Cross(front, up)) * 2.4f * (float)e.Time;
            }

            Vector2 mouse = new Vector2((float)Mouse.GetCursorState().X, (float)Mouse.GetCursorState().Y);

            Vector2 delta = mouse - lastPos;

            yawpitch += delta / 5.0f;
            //basic spherical coordinates, that I've done before
            
            

            front.X = (float)Math.Sin(MathHelper.DegreesToRadians(yawpitch.Y)) * (float)Math.Cos(MathHelper.DegreesToRadians(yawpitch.X));
            front.Y = (float)Math.Cos(MathHelper.DegreesToRadians(yawpitch.Y));
            front.Z = (float)Math.Sin(MathHelper.DegreesToRadians(yawpitch.Y)) * (float)Math.Sin(MathHelper.DegreesToRadians(yawpitch.X));
            front = Vector3.Normalize(front);


            lastPos = mouse;
            base.OnUpdateFrame(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            
             //Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);

            base.OnMouseMove(e);
        }
        int depthtext;
        protected override void OnLoad(EventArgs e)
        {
            yawpitch = new Vector2(0.0f,0.0f);
            //shadows from a tutorial://///////////
            //For PostFX
            colorbuff = GL.GenFramebuffer();
            colortex = GL.GenTexture();
            colordepthtex = GL.GenTexture();
            colornormtex = GL.GenTexture();
            colorpostex = GL.GenTexture();
            //TEXTURE COLOR ///////////////////////////////////////////////
            GL.BindTexture(TextureTarget.Texture2D, colortex);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba32f,
                Width, Height, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, IntPtr.Zero);

            float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, colorbuff);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, colortex, 0
                );
            ////////////////////////////////////////////////////////////////
            ///
            GL.BindTexture(TextureTarget.Texture2D, colordepthtex);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba32f,
                Width, Height, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment1,
                TextureTarget.Texture2D, colordepthtex, 0
                );
            //////////////////////////////////////////////////////////////
            GL.BindTexture(TextureTarget.Texture2D, colornormtex);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba32f,
                Width, Height, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment2,
                TextureTarget.Texture2D, colornormtex, 0
                );
            //////////////////////////////////////////////////////
            ///
            GL.BindTexture(TextureTarget.Texture2D, colorpostex);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba32f,
                Width, Height, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment3,
                TextureTarget.Texture2D, colorpostex, 0
                );
            DrawBuffersEnum[] att = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2, DrawBuffersEnum.ColorAttachment3 };
            GL.DrawBuffers(4, att);
            ////////////////////////////////////////////////////////////////////
            colorrbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, colorrbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Width, Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, colorrbo);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            /////////////////////////////////////////////
            depthmap = GL.GenFramebuffer();
            depthtext = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, depthtext);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexImage2D(TextureTarget.Texture2D, 0, 
                PixelInternalFormat.DepthComponent, 
                2048, 2048, 0, PixelFormat.DepthComponent,
                PixelType.Float, IntPtr.Zero);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, depthmap);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D,depthtext,0 
                );
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            ///////////////////////////////////////////////
            sunshader = new Shader("sunshader.v", "sunshader.f");
            skyshader = new Shader("skyshader.v", "skyshader.f");
            tonemappingshader = new Shader("post.v", "post.f");
            //Set the background color when clearing(wonder if you can set it as an image)
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.9f,0.8f,0.7f,1.0f);
            shader = new Shader("shader.v", "shader.f");
            texture = new Texture("garden.png", TextureUnit.Texture0);
            texture2 = new Texture("spec.png", TextureUnit.Texture1);
            texture3 = new Texture("gardenn.png", TextureUnit.Texture3);
            classroom = new Object("garden.obj");
            stopWatch = new Stopwatch();
            stopWatch.Start();
            shader.Run();
            shader.SetInt("texture1", 0);
            shader.SetInt("texture2", 1);
            shader.SetInt("shadowMap", 2);
            shader.SetInt("norm", 3);

            //shader.SetInt("texture2", 1);
            VertexBufferObject = GL.GenBuffer();
           // VBOnormals = GL.GenBuffer();
            VertexArrayObject = GL.GenVertexArray();
            EBO = GL.GenBuffer();
            quadEBO = GL.GenBuffer();
            VAOsun = GL.GenVertexArray();

            GL.BindVertexArray(VertexArrayObject);
            //Seclect the VertexBufferObject
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            //Using the selected vertex buffer object, write to it the vertices data using static draw(meaning that data won't change)
            GL.BufferData(BufferTarget.ArrayBuffer, classroom.combined.Length * sizeof(float), classroom.combined, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, classroom.indices.Length * sizeof(uint), classroom.indices, BufferUsageHint.StaticDraw);

            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //int vertexPosition = GL.GetUniformLocation(shader.Handle, "aPosition");
            //GL.EnableVertexAttribArray(vertexPosition);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            //int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            int texCoordLocation = GL.GetAttribLocation(shader.Handle, "aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCoordLocation);

            int normsCoords = GL.GetAttribLocation(shader.Handle, "aNormal");
            GL.VertexAttribPointer(normsCoords, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
            GL.EnableVertexAttribArray(normsCoords);

            sunshader.Run();

            GL.BindVertexArray(VAOsun);
            //Seclect the VertexBufferObject
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            //Using the selected vertex buffer object, write to it the vertices data using static draw(meaning that data won't change)
            GL.BufferData(BufferTarget.ArrayBuffer, classroom.combined.Length * sizeof(float), classroom.combined, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, classroom.indices.Length * sizeof(uint), classroom.indices, BufferUsageHint.StaticDraw);

            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //int vertexPosition = GL.GetUniformLocation(shader.Handle, "aPosition");
            //GL.EnableVertexAttribArray(vertexPosition);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);



            skyboxVBO = GL.GenBuffer();
            skyboxVAO = GL.GenVertexArray();
            skyshader.Run();
            GL.BindVertexArray(skyboxVAO);
            //Seclect the VertexBufferObject
            GL.BindBuffer(BufferTarget.ArrayBuffer, skyboxVBO);
            //Using the selected vertex buffer object, write to it the vertices data using static draw(meaning that data won't change)
            GL.BufferData(BufferTarget.ArrayBuffer, skyboxVertices.Length * sizeof(float), skyboxVertices, BufferUsageHint.StaticDraw);

            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //int vertexPosition = GL.GetUniformLocation(shader.Handle, "aPosition");
            //GL.EnableVertexAttribArray(vertexPosition);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);


            //quad for postFX
            quadVBO = GL.GenBuffer();
            quadVAO = GL.GenVertexArray();
            tonemappingshader.Run();
            tonemappingshader.SetInt("color", 0);
            tonemappingshader.SetInt("depth", 1);
            tonemappingshader.SetInt("norm", 2);
            tonemappingshader.SetInt("posit", 3);

            GL.BindVertexArray(quadVAO);
            //Seclect the VertexBufferObject
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadEBO);

            //Using the selected vertex buffer object, write to it the vertices data using static draw(meaning that data won't change)
            GL.BufferData(BufferTarget.ArrayBuffer, quad.Length * sizeof(float), quad, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, quadindices.Length * sizeof(uint), quadindices, BufferUsageHint.StaticDraw);

            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //int vertexPosition = GL.GetUniformLocation(shader.Handle, "aPosition");
            //GL.EnableVertexAttribArray(vertexPosition);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            int texCoordLocation2 = GL.GetAttribLocation(tonemappingshader.Handle, "aTexCoord");
            GL.VertexAttribPointer(texCoordLocation2, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCoordLocation2);

            //CursorGrabbed = true;
            //CursorVisible = false;

            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            //set to null, not sure why
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            shader.Dispose();
            sunshader.Dispose();
            stopWatch.Stop();
            //free from memory whenever the program is closing
            GL.DeleteBuffer(VertexBufferObject);


            base.OnUnload(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {

            //clear the screen, this is done so that if you only draw a single object, the next frame doesn't just draw on top of it, creating a cool effect tho
            //shadows
           // GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.BindVertexArray(VertexBufferObject);
            Vector3 lpos = new Vector3(20.0f*(float)Math.Cos(stopWatch.Elapsed.TotalSeconds), 20.0f, 20.0f * (float)Math.Sin(stopWatch.Elapsed.TotalSeconds));
            Matrix4 lightview = Matrix4.LookAt(
                lpos,
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f)
                );

            //Matrix4 lightproj = Matrix4.CreateOrthographicOffCenter(
            //    -60.0f, 60.0f, -60.0f, 60.0f, 1.0f, 100.0f
            //    );
            Matrix4 lightproj = Matrix4.CreateOrthographic(40.0f, 40.0f, 0.01f, 100.5f);

            //Matrix4 lightproj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Width / Height, 0.01f, 100.0f);

            sunshader.Run();
            // Matrix4 model2 = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(0.0f));
           Matrix4 modell = Matrix4.Identity;
            // Matrix4 lightSpace = lightproj * lightview * modell ;
            //uniform mat4 lightproj;
            //uniform mat4 lightview;
            //uniform mat4 lightid;
            sunshader.setMatrix4(lightproj, "lightproj");
            sunshader.setMatrix4(lightview, "lightview");
            sunshader.setMatrix4(modell, "lightid");
            GL.CullFace(CullFaceMode.Front);
            //GL.BindVertexArray(VertexArrayObject);
            GL.Viewport(0, 0, 2048, 2048);
            //GL.BindVertexArray(VertexBufferObject);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, depthmap);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.DrawElements(PrimitiveType.Triangles, classroom.indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            //GL.BindVertexArray(VertexArrayObject);
            GL.CullFace(CullFaceMode.Back);


            //model view and projection matricies:
            Matrix4 model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(0.0f));

            Matrix4 view = Matrix4.LookAt(position, position + front, up);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(65.0f), Width / Height, 0.1f, 100.0f);


            Matrix4 view2 = view.ClearTranslation();
            //GL.Viewport(0, 0, Width, Height);
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // GL.Viewport(0, 0, Width, Height);
            // GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, colorbuff);

            ///
            GL.BindVertexArray(VertexArrayObject);

            GL.Viewport(0,0,Width,Height);
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Run();
            
            
            shader.setMatrix4(model, "model");
            shader.setMatrix4(view, "view");
            shader.setMatrix4(projection, "projection");
            shader.setMatrix4(lightproj, "lightproj");
            shader.setMatrix4(lightview, "lightview");
            shader.setMatrix4(modell, "lightid");
            shader.setVector3(position, "viewPos");
            shader.setVector3(lpos,"lpos");
            texture.Use(TextureUnit.Texture0);
            texture2.Use(TextureUnit.Texture1);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, depthtext);
            texture3.Use(TextureUnit.Texture3);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.DrawElements(PrimitiveType.Triangles, classroom.indices.Length, DrawElementsType.UnsignedInt, 0);
            //renders use double buffering, meaning that they have 2 buffers that they can draw the image to, one is being drawn to, while the other is
            //being shown, this is done to prevent drawing to be shown on the screen 

            //skybox:

            GL.DepthMask(false);
            GL.DepthFunc(DepthFunction.Lequal);
            skyshader.Run();

            skyshader.setMatrix4(view2, "view");
            skyshader.setMatrix4(projection, "projection");
            shader.setVector3(position, "viewPos");
            shader.setVector3(lpos, "lpos");
            // ... set view and projection matrix
            GL.BindVertexArray(skyboxVAO);
            GL.DrawArrays(BeginMode.Triangles, 0, 36);
            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Less);


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(1.0f,1.0f,1.0f,1.0f);
             GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.DepthTest);
            tonemappingshader.Run();
            GL.BindVertexArray(quadVAO);
            tonemappingshader.setMatrix4(model, "model");
            tonemappingshader.setMatrix4(view, "view");
            tonemappingshader.setMatrix4(projection, "projection");
            tonemappingshader.setMatrix4(view.Inverted(), "invview");
            tonemappingshader.setVector2(new Vector2((float)Width, (float)Height), "wh");
            tonemappingshader.setFloat(stopWatch.ElapsedMilliseconds, "time");
            tonemappingshader.setVector3(position, "viewPos");

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, colortex);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, colordepthtex);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, colornormtex);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, colorpostex);

            GL.DrawElements(PrimitiveType.Triangles, quadindices.Length, DrawElementsType.UnsignedInt, 0);

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            //reset on resize 
            GL.Viewport(0,0,Width,Height);
            base.OnResize(e);
        }
    }
}
